using SmartTVRemoteControl.Native.DLNA.UPnP;
using SmartTVRemoteControl.Native.HTTP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Timers;

namespace SmartTVRemoteControl.Native.DLNA.SSDP
{
	internal class SsdpServer : IDisposable
	{
		private const string SSDP_ADDR = "239.255.255.250";

		private const int SSDP_PORT = 1900;

		private const int DATAGRAMS_PER_MESSAGE = 2;

		private readonly UdpClient client = new UdpClient();

		private readonly AutoResetEvent datagramPosted = new AutoResetEvent(false);

		private readonly Dictionary<Guid, List<UPnPDevice>> devices = new Dictionary<Guid, List<UPnPDevice>>();

		private readonly ConcurrentQueue<Datagram> messageQueue = new ConcurrentQueue<Datagram>();

		private readonly System.Timers.Timer notificationTimer = new System.Timers.Timer(60000);

		private readonly System.Timers.Timer queueTimer = new System.Timers.Timer(1000);

		private readonly static Random random;

		private readonly static IPEndPoint SSDP_ENDP;

		private readonly static IPAddress SSDP_IP;

		private bool running = true;

		private UPnPDevice[] Devices
		{
			get
			{
				UPnPDevice[] array;
				lock (this.devices)
				{
					array = this.devices.Values.SelectMany<List<UPnPDevice>, UPnPDevice>((List<UPnPDevice> i) => i).ToArray<UPnPDevice>();
				}
				return array;
			}
		}

		static SsdpServer()
		{
			SsdpServer.random = new Random();
			SsdpServer.SSDP_ENDP = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);
			SsdpServer.SSDP_IP = IPAddress.Parse("239.255.255.250");
		}

		public SsdpServer()
		{
			this.notificationTimer.Elapsed += new ElapsedEventHandler(this.Tick);
			this.notificationTimer.Enabled = true;
			this.queueTimer.Elapsed += new ElapsedEventHandler(this.ProcessQueue);
			this.client.Client.UseOnlyOverlappedIO = true;
			this.client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			this.client.ExclusiveAddressUse = false;
			this.client.Client.Bind(new IPEndPoint(IPAddress.Any, 1900));
			this.client.JoinMulticastGroup(SsdpServer.SSDP_IP, 2);
			this.Receive();
		}

		public void Dispose()
		{
			this.running = false;
			while (this.messageQueue.Count != 0)
			{
				this.datagramPosted.WaitOne();
			}
			this.client.DropMulticastGroup(SsdpServer.SSDP_IP);
			this.queueTimer.Elapsed -= new ElapsedEventHandler(this.ProcessQueue);
			this.notificationTimer.Enabled = false;
			this.queueTimer.Enabled = false;
			this.notificationTimer.Dispose();
			this.queueTimer.Dispose();
			this.datagramPosted.Dispose();
		}

		internal void NotifyAll()
		{
			UPnPDevice[] devices = this.Devices;
			for (int i = 0; i < (int)devices.Length; i++)
			{
				this.NotifyDevice(devices[i], "alive", false);
			}
		}

		internal void NotifyDevice(UPnPDevice dev, string type, bool sticky)
		{
			Headers header = new Headers()
			{
				{ "HOST", "239.255.255.250:1900" },
				{ "CACHE-CONTROL", "max-age = 600" },
				{ "LOCATION", dev.Descriptor.ToString() },
				{ "SERVER", HttpServer.Signature },
				{ "NTS", string.Concat("ssdp:", type) },
				{ "NT", dev.Type },
				{ "USN", dev.USN }
			};
			this.SendDatagram(SsdpServer.SSDP_ENDP, dev.Address, string.Format("NOTIFY * HTTP/1.1\r\n{0}\r\n", header.HeaderBlock), sticky);
		}

		private void ProcessQueue(object sender, ElapsedEventArgs e)
		{
			while (this.messageQueue.Count != 0)
			{
				Datagram datagram = null;
				if (!this.messageQueue.TryPeek(out datagram))
				{
					continue;
				}
				if (datagram == null || !this.running && !datagram.Sticky)
				{
					this.messageQueue.TryDequeue(out datagram);
				}
				else
				{
					datagram.Send();
					if (datagram.SendCount <= 2)
					{
						break;
					}
					this.messageQueue.TryDequeue(out datagram);
					break;
				}
			}
			this.datagramPosted.Set();
			this.queueTimer.Enabled = this.messageQueue.Count != 0;
			this.queueTimer.Interval = (double)SsdpServer.random.Next(50, (this.running ? 300 : 100));
		}

		private void Receive()
		{
			try
			{
				this.client.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
			}
			catch (ObjectDisposedException objectDisposedException)
			{
			}
		}

		private void ReceiveCallback(IAsyncResult result)
		{
			try
			{
				IPEndPoint pEndPoint = new IPEndPoint(IPAddress.None, 1900);
				byte[] numArray = this.client.EndReceive(result, ref pEndPoint);
				using (StreamReader streamReader = new StreamReader(new MemoryStream(numArray), Encoding.ASCII))
				{
					string str = streamReader.ReadLine().Trim();
					char[] chrArray = new char[] { ' ' };
					string str1 = str.Split(chrArray, 2)[0];
					Headers header = new Headers();
					for (string i = streamReader.ReadLine(); i != null; i = streamReader.ReadLine())
					{
						i = i.Trim();
						if (string.IsNullOrEmpty(i))
						{
							break;
						}
						char[] chrArray1 = new char[] { ':' };
						string[] strArrays = i.Split(chrArray1, 2);
						header[strArrays[0]] = strArrays[1].Trim();
					}
					if (str1 == "M-SEARCH")
					{
						this.RespondToSearch(pEndPoint, header["st"]);
					}
				}
			}
			catch (Exception exception)
			{
			}
			this.Receive();
		}

		internal void RegisterNotification(Guid UUID, Uri Descriptor, IPAddress address)
		{
			List<UPnPDevice> uPnPDevices;
			lock (this.devices)
			{
				if (!this.devices.TryGetValue(UUID, out uPnPDevices))
				{
					Dictionary<Guid, List<UPnPDevice>> guids = this.devices;
					List<UPnPDevice> uPnPDevices1 = new List<UPnPDevice>();
					uPnPDevices = uPnPDevices1;
					guids.Add(UUID, uPnPDevices1);
				}
			}
			string[] strArrays = new string[] { "upnp:rootdevice", "urn:schemas-upnp-org:device:MediaServer:1", "urn:schemas-upnp-org:service:ContentDirectory:1", string.Concat("uuid:", UUID) };
			string[] strArrays1 = strArrays;
			for (int i = 0; i < (int)strArrays1.Length; i++)
			{
				string str = strArrays1[i];
				uPnPDevices.Add(new UPnPDevice(UUID, str, Descriptor, address));
			}
			this.NotifyAll();
		}

		internal void RespondToSearch(IPEndPoint endpoint, string req)
		{
			if (req == "ssdp:all")
			{
				req = null;
			}
			UPnPDevice[] devices = this.Devices;
			for (int i = 0; i < (int)devices.Length; i++)
			{
				UPnPDevice uPnPDevice = devices[i];
				if (string.IsNullOrEmpty(req) || !(req != uPnPDevice.Type))
				{
					this.SendSearchResponse(endpoint, uPnPDevice);
				}
			}
		}

		private void SendDatagram(IPEndPoint endpoint, IPAddress address, string message, bool sticky)
		{
			if (!this.running)
			{
				return;
			}
			Datagram datagram = new Datagram(endpoint, address, message, sticky);
			if (this.messageQueue.Count == 0)
			{
				datagram.Send();
			}
			this.messageQueue.Enqueue(datagram);
			this.queueTimer.Enabled = true;
		}

		private void SendSearchResponse(IPEndPoint endpoint, UPnPDevice dev)
		{
			Headers header = new Headers()
			{
				{ "CACHE-CONTROL", "max-age = 600" },
				{ "DATE", DateTime.Now.ToString("R") },
				{ "EXT", "" },
				{ "LOCATION", dev.Descriptor.ToString() },
				{ "SERVER", HttpServer.Signature },
				{ "ST", dev.Type },
				{ "USN", dev.USN }
			};
			this.SendDatagram(endpoint, dev.Address, string.Format("HTTP/1.1 200 OK\r\n{0}\r\n", header.HeaderBlock), false);
		}

		private void Tick(object sender, ElapsedEventArgs e)
		{
			this.notificationTimer.Interval = (double)SsdpServer.random.Next(60000, 120000);
			this.NotifyAll();
		}

		internal void UnregisterNotification(Guid UUID)
		{
			List<UPnPDevice> uPnPDevices;
			lock (this.devices)
			{
				if (this.devices.TryGetValue(UUID, out uPnPDevices))
				{
					this.devices.Remove(UUID);
				}
				else
				{
					return;
				}
			}
			foreach (UPnPDevice uPnPDevice in uPnPDevices)
			{
				this.NotifyDevice(uPnPDevice, "byebye", true);
			}
		}
	}
}