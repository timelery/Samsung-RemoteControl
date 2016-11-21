using Networking.Native;
using SmartView2.Core;
using SmartTVRemoteControl.Common;
using SmartView2.Devices;
using SmartView2.Devices.SecondTv;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UPnP;
using UPnP.DataContracts;
using Nito.AsyncEx;
using UIFoundation.Navigation;
using UIFoundation;
using System.Windows.Threading;

namespace SmartTVRemoteControl
{
	public class DeviceController : IDisposable
	{
		private IDeviceDiscovery deviceDiscovery;

		private readonly IBaseDispatcher dispatcher;

        private readonly IDevicePairing devicePairing;

		private readonly IDeviceSettingProvider settingProvider;

		private readonly CancellationTokenSource tokenSource;

		private readonly IPlayerNotificationProvider notificationProvider;

		private bool isRefreshing;

		public ITargetDevice CurrentDevice
		{
			get;
			private set;
		}

		public DeviceInfo CurrentDeviceInfo
		{
			get;
			private set;
		}

		public ObservableCollection<DeviceInfo> Devices
		{
			get;
			private set;
		}

		public IPlayerNotificationProvider NotificationProvider
		{
			get
			{
				return this.notificationProvider;
			}
		}

		public DeviceController(IDeviceDiscovery deviceDiscovery, IDevicePairing devicePairing, IPlayerNotificationProvider notificationProvider, IDeviceSettingProvider settingProvider)
		{
			if (deviceDiscovery == null)
			{
                Console.WriteLine("deviceDiscovery null.");
				throw new ArgumentNullException("deviceDiscovery");
			}
			if (devicePairing == null)
			{
                Console.WriteLine("devicePairing null.");
                throw new ArgumentNullException("devicePairing");
			}
			if (settingProvider == null)
			{
                Console.WriteLine("settingProvider null.");
                throw new ArgumentNullException("settingProvider");
			}
			if (notificationProvider == null)
			{
                Console.WriteLine("notificationProvider null");
                throw new ArgumentNullException("notificationProvider");
			}
			this.deviceDiscovery = deviceDiscovery;
			this.deviceDiscovery.DeviceConnected += new EventHandler<DeviceInfoEventArgs>(this.deviceDiscovery_DeviceConnected);
			this.deviceDiscovery.DeviceUpdated += new EventHandler<DeviceInfoEventArgs>(this.deviceDiscovery_DeviceUpdated);
			this.deviceDiscovery.DeviceDisconnected += new EventHandler<DeviceInfoEventArgs>(this.deviceDiscovery_DeviceDisconnected);
			this.devicePairing = devicePairing;
			this.notificationProvider = notificationProvider;
			this.settingProvider = settingProvider;
			this.Devices = new ObservableCollection<DeviceInfo>();
			this.tokenSource = new CancellationTokenSource();
		}

		public Task ClosePinPageOnTV()
		{
			return this.devicePairing.ClosePinPageAsync();
		}

		public async Task<DeviceController.ConnectResult> Connect(DeviceInfo device)
		{
			DeviceController.ConnectResult connectResult;
			Console.WriteLine("Connect started... ");
			try
			{
				if (!await this.devicePairing.StartPairingAsync(device.DeviceAddress))
				{
					connectResult = DeviceController.ConnectResult.PinPageAlreadyShown;
				}
				else
				{
					this.SetCurrentDeviceInfo(device);
					connectResult = DeviceController.ConnectResult.Ok;
				}
			}
			catch (SocketException socketException)
			{
				connectResult = DeviceController.ConnectResult.SocketException;
			}
			catch
			{
				connectResult = DeviceController.ConnectResult.OtherException;
			}
			return connectResult;
		}

		public async Task<bool> ConnectToPreviousDeviceAsync()
		{
			bool connect;
			await Task.Delay(TimeSpan.FromSeconds(3));
			string str = this.settingProvider.LoadLastIp();
			DeviceInfo deviceInfo = (
				from device in this.Devices
				where device.DeviceAddress.Host == str
				select device).FirstOrDefault<DeviceInfo>();
			if (deviceInfo != null)
			{
				connect = await this.TryToConnect(deviceInfo.DeviceAddress.Host);
			}
			else
			{
				connect = false;
			}
			return connect;
		}

		private void CurrentDevice_Disconnecting(object sender, EventArgs e)
		{
			Console.WriteLine("CurrentDevice_Disconnecting started... ");
			this.OnCurrentDeviceDisconnected(this, EventArgs.Empty);
		}

		private void deviceDiscovery_DeviceConnected(object sender, DeviceInfoEventArgs e)
		{
            Console.WriteLine("UPnP connection extablished with TV - " + e.DeviceInfo.UniqueDeviceName);
            if (this.Devices.Any<DeviceInfo>((DeviceInfo device) => device.UniqueDeviceName == e.DeviceInfo.UniqueDeviceName))
				{
					return;
				}
				Console.WriteLine("DeviceConnected added TV: {0}", new object[] { e.DeviceInfo.DeviceAddress });
				this.Devices.Add(e.DeviceInfo);
		}

		private void deviceDiscovery_DeviceDisconnected(object sender, DeviceInfoEventArgs e)
		{
			bool flag;
			Console.WriteLine("DeviceDisconnected started... ");
			flag = (this.CurrentDeviceInfo == null ? false : this.CurrentDeviceInfo.UniqueDeviceName == e.DeviceInfo.UniqueDeviceName);
			if (this.isRefreshing && flag)
			{
				return;
			}
			//this.dispatcher.Invoke(() => {
				DeviceInfo deviceInfo = (
					from arg in this.Devices
					where arg.UniqueDeviceName == e.DeviceInfo.UniqueDeviceName
					select arg).FirstOrDefault<DeviceInfo>();
				if (deviceInfo != null)
				{
					Console.WriteLine("DeviceDisconnected remove TV: {0}", new object[] { deviceInfo.DeviceAddress });
					this.Devices.Remove(deviceInfo);
					if (this.CurrentDeviceInfo != null && this.CurrentDeviceInfo.UniqueDeviceName == deviceInfo.UniqueDeviceName)
					{
						Console.WriteLine("Disconnect current device.");
						this.OnCurrentDeviceDisconnected(this, EventArgs.Empty);
					}
				}
			//});
		}

		private void deviceDiscovery_DeviceUpdated(object sender, DeviceInfoEventArgs e)
		{
            //this.dispatcher.Invoke(() => {
            Console.WriteLine("Device Updated");
				DeviceInfo friendlyName = (
					from device in this.Devices
					where device.UniqueDeviceName == e.DeviceInfo.UniqueDeviceName
					select device).FirstOrDefault<DeviceInfo>();
				if (friendlyName != null)
				{
					friendlyName.FriendlyName = e.DeviceInfo.FriendlyName;
				}
			//});
		}

		public async Task DisconnectAsync(bool isFail = false)
		{
			Console.WriteLine("Disconnect started... ");
			Console.WriteLine(string.Concat("Disconnect CurrentDevice isNull: ", this.CurrentDevice == null));
			if (this.CurrentDevice != null)
			{
				await this.CurrentDevice.DisconnectAsync();
				this.CurrentDevice.Dispose();
				this.SetCurrentDevice(null);
				this.SetCurrentDeviceInfo(null);
			}
			if (isFail)
			{
				this.settingProvider.ResetLastConnectionAddress();
			}
		}

		public void Dispose()
		{
			this.tokenSource.Cancel();
			if (this.deviceDiscovery != null)
			{
				this.deviceDiscovery.DeviceConnected -= new EventHandler<DeviceInfoEventArgs>(this.deviceDiscovery_DeviceConnected);
				this.deviceDiscovery.DeviceDisconnected -= new EventHandler<DeviceInfoEventArgs>(this.deviceDiscovery_DeviceDisconnected);
				this.deviceDiscovery.Dispose();
				this.deviceDiscovery = null;
			}
		}

		private async Task InitTargetDevice(DeviceInfo device)
		{
			Console.WriteLine("InitTargetDevice - started... ");
			Logger instance = Logger.Instance;
			object[] encryptionEnabled = new object[] { this.devicePairing.EncryptionEnabled };
			instance.LogMessageFormat("InitTargetDevice - EncryptionEnabled : {0}", encryptionEnabled);
			ISecondTvSecurityProvider noSecurityProvider = null;
			if (!this.devicePairing.EncryptionEnabled)
			{
				noSecurityProvider = new NoSecurityProvider();
			}
			else
			{
				noSecurityProvider = new AesSecurityProvider(this.devicePairing.SpcApi.GetKey(), this.devicePairing.SessionId);
			}
			Console.WriteLine("InitTargetDevice - Created Tv device.");
            
            ITargetDevice targetDevice = SmartTVRemoteControl.DeviceFactory.CreateTvDevice(device, this.notificationProvider, new DispatcherWrapper(Dispatcher.CurrentDispatcher), noSecurityProvider);
            Console.WriteLine("InitTargetDevice - Set current device.");
			this.SetCurrentDevice(targetDevice);
			try
			{
				Console.WriteLine("InitTargetDevice - Initialize Tv device.");
                AsyncContext.Run(() => targetDevice.InitializeAsync());
                //await targetDevice.InitializeAsync();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Console.WriteLine("[ERROR]Catch exception when initialize TV.");
				Logger logger = Logger.Instance;
				logger.LogMessageFormat("Exception message: {0}", new object[] { exception.Message });
				Logger instance1 = Logger.Instance;
				instance1.LogMessageFormat("Exception stacktrace: {0}", new object[] { exception.StackTrace });
			}
            finally
            {
                Console.WriteLine("InitTargetDevice is complete");
            }
		}

		private void OnCurrentDeviceDisconnected(object sender, EventArgs e)
		{
			EventHandler<EventArgs> eventHandler = this.CurrentDeviceDisconnected;
			if (eventHandler != null)
			{
				eventHandler(sender, e);
			}
		}

		public async Task<DeviceController.ConnectResult> Reconnect(DeviceInfo device)
		{
			Console.WriteLine("Reconnect started... ");
			await this.DisconnectAsync(false);
			return await this.Connect(device);
		}

		public void RefreshDiscovery()
		{
            Console.WriteLine("Refresh Discovery");
            this.isRefreshing = true;
            this.deviceDiscovery.Refresh();
			this.isRefreshing = false;
		}

		private void SetCurrentDevice(ITargetDevice device)
		{
			if (this.CurrentDevice != null)
			{
				this.CurrentDevice.Disconnecting -= new EventHandler<EventArgs>(this.CurrentDevice_Disconnecting);
			}
			this.CurrentDevice = device;
			if (this.CurrentDevice != null)
			{
				this.CurrentDevice.Disconnecting += new EventHandler<EventArgs>(this.CurrentDevice_Disconnecting);
			}
		}

		private void SetCurrentDeviceInfo(DeviceInfo device)
		{
			this.CurrentDeviceInfo = device;
		}

		public async Task<bool> SetPin(string pin)
		{
			bool flag;
            string str = AsyncContext.Run(() => this.devicePairing.EnterPinAsync(pin));
            //string str = await ;
			if (string.IsNullOrEmpty(str))
			{
				flag = false;
			}
			else
			{
                await this.InitTargetDevice(this.CurrentDeviceInfo);
				this.settingProvider.Save(this.CurrentDeviceInfo.DeviceAddress.Host, pin);
				flag = true;
			}
			return flag;
		}

		public void StartDiscovery()
		{
            Console.WriteLine("Discovery started... ");
            Task.Run(async () => {
                while (!this.tokenSource.IsCancellationRequested)
                {
                    this.deviceDiscovery.Scan();
                    await Task.Delay(2000);
                }
            }, this.tokenSource.Token);
        }
        
        public void StopDiscovery()
		{
			//Console.WriteLine("Stop discovery started... ");
			this.tokenSource.Cancel();
		}

		public async Task<bool> TryToConnect(string deviceAddress = "")
		{
			bool flag;
			string str = deviceAddress;
			if (str == "")
			{
				str = this.settingProvider.LoadLastIp();
			}
			if (!string.IsNullOrEmpty(str))
			{
				string str1 = this.settingProvider.LoadPin(str);
				if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(str1))
				{
					ObservableCollection<DeviceInfo> devices = this.Devices;
					DeviceInfo deviceInfo = devices.FirstOrDefault<DeviceInfo>((DeviceInfo d) => d.DeviceAddress.Host.Contains(str));
					if (deviceInfo != null)
					{
						try
						{
							string str2 = await this.devicePairing.TryPairAsync(deviceInfo.DeviceAddress, str1);
							if (string.IsNullOrEmpty(str2))
							{
								this.settingProvider.Save(str, string.Empty);
							}
							else
							{
								this.SetCurrentDeviceInfo(deviceInfo);
								this.settingProvider.Save(str, str1);
								await this.InitTargetDevice(deviceInfo);
								flag = true;
								return flag;
							}
						}
						catch
						{
							flag = false;
							return flag;
						}
					}
				}
				flag = false;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public event EventHandler<EventArgs> CurrentDeviceDisconnected;

		public enum ConnectResult
		{
			Ok,
			PinPageAlreadyShown,
			SocketException,
			OtherException
		}
	}
}