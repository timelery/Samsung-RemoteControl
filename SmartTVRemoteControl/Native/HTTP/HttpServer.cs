using SmartView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartTVRemoteControl.Native.HTTP
{
    internal class HttpServer : IHttpServer, IDisposable
    {
        public static readonly string Signature = HttpServer.GenerateServerSignature();
        private Dictionary<Regex, IHttpHandler> handlerMap = new Dictionary<Regex, IHttpHandler>();
        private TcpListener listener;
        private IPAddress deviceIP;

        public bool IsWorking { get; private set; }

        public string ServerUrl { get; private set; }

        public IPEndPoint LocalEndpoint
        {
            get
            {
                return this.listener.LocalEndpoint as IPEndPoint;
            }
        }

        public HttpServer()
        {
            this.IsWorking = false;
        }

        private async Task AcceptAsync()
        {
            while (this.IsWorking)
            {
                try
                {
                    TcpClient client = await this.listener.AcceptTcpClientAsync();
                    Task.Factory.StartNew<Task>((Func<Task>)(async () =>
                    {
                        try
                        {
                            HttpRequest request = await this.ReadRequestAsync(client);
                            HttpResponse response = this.HandleRequest(request);
                            await this.WriteResponseAsync(client, response);
                            if (!request.Headers.Contains(new KeyValuePair<string, string>("Connection", "keep-alive")))
                                client.Close();
                        }
                        catch (Exception ex)
                        {
                        }
                        finally
                        {
                            if (client != null)
                                client.Close();
                        }
                    }));
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void AddHandler(IHttpHandler handler)
        {
            this.handlerMap.Add(new Regex("^" + handler.Prefix.Replace("/**", "/.*").Replace("/*", "/[^/]+") + "$", RegexOptions.Compiled), handler);
        }

        private async Task<HttpRequest> ReadRequestAsync(TcpClient client)
        {
            HttpRequest request = new HttpRequest(client);
            StreamReader reader = new StreamReader((Stream)client.GetStream());
            StreamWriter writer = new StreamWriter(request.Body);
            string line = await reader.ReadLineAsync();
            string[] parts = line.Split(new char[1]
            {
        ' '
            }, 3);
            request.Method = parts[0].Trim().ToUpper();
            request.Path = parts[1].Trim();
            for (line = await reader.ReadLineAsync(); !string.IsNullOrEmpty(line); line = await reader.ReadLineAsync())
            {
                parts = line.Split(new char[1]
                {
          ':'
                }, 2);
                request.Headers[parts[0]] = Uri.UnescapeDataString(parts[1]).Trim();
            }
            string val;
            int length;
            if (request.Headers.TryGetValue("Content-Length", out val) && int.TryParse(val, out length) && length > 0)
            {
                char[] buf = new char[length];
                length = await reader.ReadAsync(buf, 0, buf.Length);
                await writer.WriteAsync(buf, 0, length);
                await writer.FlushAsync();
                request.Body.Seek(0L, SeekOrigin.Begin);
            }
            return request;
        }

        private async Task WriteResponseAsync(TcpClient client, HttpResponse response)
        {
            StreamWriter writer = new StreamWriter((Stream)client.GetStream());
            this.ProcessRanges(response);
            await writer.WriteLineAsync(string.Format("HTTP/1.1 {0} {1}", (object)(uint)response.Status, (object)HttpPhrases.Phrases[response.Status]));
            await writer.WriteLineAsync(response.Headers.HeaderBlock);
            await writer.FlushAsync();
            if (response.Request.Method != "HEAD")
                await response.Body.CopyToAsync(writer.BaseStream);
        }

        private void ProcessRanges(HttpResponse response)
        {
            if (!response.Request.Headers.ContainsKey("Range"))
                return;
            long length = response.Body.Length;
            string[] strArray = response.Request.Headers["Range"].Substring(6).Split('-');
            long result1;
            if (!long.TryParse(strArray[0], out result1) || result1 < 0L)
                return;
            long result2 = length - 1L;
            if (strArray.Length >= 2 || !long.TryParse(strArray[1], out result2) || (result2 <= result1 || result2 >= length))
                result2 = length - 1L;
            if (result1 >= result2)
            {
                response.Body.Close();
            }
            else
            {
                if (result1 > 0L)
                    response.Body.Seek(result1, SeekOrigin.Begin);
                response.Headers["Content-Length"] = (result2 - result1 + 1L).ToString();
                response.Headers["Content-Range"] = string.Format("bytes {0}-{1}/{2}", (object)result1, (object)result2, (object)length);
                response.Status = HttpCode.Partial;
            }
        }

        private HttpResponse HandleRequest(HttpRequest request)
        {
            foreach (KeyValuePair<Regex, IHttpHandler> keyValuePair in this.handlerMap)
            {
                if (keyValuePair.Key.IsMatch(request.Path))
                    return keyValuePair.Value.HandleRequest(request);
            }
            return new HttpResponse(request, HttpCode.NotFound, "text/html", "");
        }

        public Task InitializeAsync(string serverAddress, string deviceAddress)
        {
            return Task.Factory.StartNew((Action)(() =>
            {
                if (this.listener != null && this.listener.Server.IsBound)
                    return;
                this.deviceIP = IPAddress.Parse(deviceAddress);
                IPAddress ipAddress = IPAddress.Parse(serverAddress);
                int freePort = HttpServer.GetFreePort(50000, 65000, ipAddress);
                this.listener = new TcpListener(new IPEndPoint(ipAddress, freePort));
                this.listener.Server.Ttl = (short)32;
                this.listener.Server.UseOnlyOverlappedIO = true;
                this.listener.Start();
                this.IsWorking = true;
                this.ServerUrl = string.Format("http://{0}:{1}", (object)ipAddress, (object)freePort);
                this.AcceptAsync();
            }));
        }

        public void Dispose()
        {
            this.Close();
        }

        public void Close()
        {
            this.IsWorking = false;
            if (this.listener == null)
                return;
            this.listener.Stop();
        }

        private static int GetFreePort(int LowRange, int UpperRange, IPAddress OnThisIP)
        {
            Random random = new Random();
            int port;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                while (true)
                {
                    port = random.Next(LowRange, UpperRange);
                    IPEndPoint ipEndPoint = new IPEndPoint(OnThisIP, port);
                    try
                    {
                        socket.Bind((EndPoint)ipEndPoint);
                        break;
                    }
                    catch
                    {
                    }
                }
                socket.Close();
            }
            return port;
        }

        private static string GenerateServerSignature()
        {
            OperatingSystem osVersion = Environment.OSVersion;
            string str = osVersion.Platform.ToString();
            switch (osVersion.Platform)
            {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                    str = "WIN";
                    break;
            }
            return string.Format("{0}{1}/{2}.{3} UPnP/1.0 DLNADOC/1.5 SmartView2/{4}.{5}", (object)str, (object)(IntPtr.Size * 8), (object)osVersion.Version.Major, (object)osVersion.Version.Minor, (object)Assembly.GetExecutingAssembly().GetName().Version.Major, (object)Assembly.GetExecutingAssembly().GetName().Version.Minor);
        }
    }
}
