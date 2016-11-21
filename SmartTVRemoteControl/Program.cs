using Networking;
using Networking.Native;
using Nito.AsyncEx;
using SmartTVRemoteControl.Native;
using SmartTVRemoteControl.Native.Player;
using SmartTVRemoteControl.Native.Wlan;
using SmartTVRemoteControl.Properties;
using SmartView2.Core;
using SmartView2.Core.Commands;
using SmartView2.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using UPnP;
using UPnP.DataContracts;

namespace SmartTVRemoteControl
{
    class Program
    {
        private static DeviceController deviceController;
        private static INetworkTransportFactory transportFactory;
        private static IDevicePairing devicePairing;
        private static IEnumerable<object> guidItems;
        private static IDataLibrary dataLibrary;
        private static ITargetDevice targetDevice;
        private static string networkNames;
        private static IEnumerable<DeviceInfo> _devices;
        private static int PinPageTimeOut = 120;
        private static int maxInvalidCount = 5;
        private static string[] pinNumbers = new string[4];
        private static int invalidPinCount;
        private static string pinErrorMessage = string.Empty;
        private static DeviceInfo device;
        private static DispatcherTimer pinTimeOutTimer;
        private static bool isPinFocused;
        private static bool connectEnabled = true;
        private static bool isDeviceAvailable;
        private static bool isPaired = false;
        private static string[] _args;
        public static ObservableCollection<DeviceInfo> Devices
        {
            get;
            private set;
        }

        private static void Main(string[] args)
        {
            _args = args;
            AsyncContext.Run(() => loadController());
        }

        static async void loadController()
        {
            Guid deviceId = Settings.Default.DeviceId;
            if (deviceId == Guid.Empty)
            {
                Console.WriteLine("No saved device ID found");
                Settings @default = Settings.Default;
                Guid guid = Guid.NewGuid();
                deviceId = guid;
                @default.DeviceId = guid;
                Settings.Default.Save();
            } else
            {
                //Console.WriteLine("Previously saved device ID found");
            }

            transportFactory = new TransportFactory();
            devicePairing = new DevicePairing(deviceId, transportFactory, new SpcApiWrapper());
            IPlayerNotificationProvider playerNotificationProvider = new PlayerNotificationProvider();
            IDeviceListener uPnPDeviceListener = new UPnPDeviceListener(new NetworkInfoProvider(), transportFactory);
            IDeviceDiscovery uPnPDeviceDiscovery = new UPnPDeviceDiscovery(transportFactory, uPnPDeviceListener);
            IDeviceDiscovery tvDiscovery = new TvDiscovery(uPnPDeviceDiscovery, new TcpWebTransport(TimeSpan.FromSeconds(5)));
            deviceController = new DeviceController(tvDiscovery, devicePairing, playerNotificationProvider, new DeviceSettingProvider());
            //Console.WriteLine("Device discovery starting");
            deviceController.StartDiscovery();

            bool previousDeviceAsync = await deviceController.ConnectToPreviousDeviceAsync();
            if (!previousDeviceAsync)
            {
                Console.WriteLine("No previously paired TV found.");
                loadDevice(deviceController);
                checkNetwork(deviceController);
            }
            else
            {
                Console.WriteLine("Previously paired TV found.");
                verifyNetwork();
            }
        }

        private static async void verifyNetwork()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                goToRemoteControl(deviceController);
            }
            else
            {
                Console.WriteLine("No wireless connection detected. Exiting now.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private static async void checkNetwork(DeviceController deviceController)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (!await deviceController.TryToConnect(""))
                {
                    Console.WriteLine("Error establishing connection with previously paired TV.");
                    goToDiscovery(deviceController);
                }
                else
                {
                    Console.WriteLine("Successfully established connection to previously paired TV. Initiating remote control receiver");
                    loadDevice(deviceController);
                    goToRemoteControl(deviceController);
                }
            }
            else
            {
                Console.WriteLine("No wireless connection detected. Exiting now.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private static async void goToRemoteControl(DeviceController deviceController)
        {
            if (deviceController == null)
            {
                throw new ArgumentNullException("deviceController");
            }
            deviceController.CurrentDeviceDisconnected += new EventHandler<EventArgs>(deviceController_CurrentDeviceDisconnected);
            deviceController.Devices.CollectionChanged -= new NotifyCollectionChangedEventHandler(Devices_CollectionChanged);
            targetDevice = deviceController.CurrentDevice;
            deviceController.StopDiscovery();

            if (_args.Length == 0)
            {
                targetDevice.CurrentSource.RemoteControl.Menu.ExecuteIfYouCan(null);
                Console.WriteLine("Menu command was sent to the TV as a test. Please verify it worked before proceeding");
                Console.WriteLine("Initial Setup Complete. Relaunch with command that you want sent to the TV");
                Console.WriteLine("A list of commands that can be sent to the TV are below.");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n");
                Console.WriteLine("ButtonA" + "  " + "ButtonB" + "  " + "ButtonC" + "  " + "ButtonD" + "  " + "ChannelDown");
                Console.WriteLine("ChannelList" + "  " + "ChannelUp" + "  " + "DiscMenu" + "  " + "Exit" + "  " + "FastForward");
                Console.WriteLine("FastRewind" + "  " + "Forward" + "  " + "Guide" + "  " + "Home" + "  " + "Info");
                Console.WriteLine("JoystickDown" + "  " + "JoystickLeft" + "  " + "JoystickOk" + "  " + "JoystickRight");
                Console.WriteLine("JoystickUp" + "  " + "Keypad" + "  " + "MbrPower" + "  " + "Menu" + "  " + "Minus" + "  " + "Mute");
                Console.WriteLine("Num0" + "  " + "Num1" + "  " + "Num2" + "  " + "Num3" + "  " + "Num4");
                Console.WriteLine("Num5" + "  " + "Num6" + "  " + "Num7" + "  " + "Num8" + "  " + "Num9");
                Console.WriteLine("Pause" + "  " + "Play" + "  " + "Power" + "  " + "PreviousChannel" + "  " + "Record");
                Console.WriteLine("Rewind" + "  " + "Search" + "  " + "SmartHub" + "  " + "StbMenu" + "  " + "Stop");
                Console.WriteLine("SubTitle" + "  " + "Tools" + "  " + "VolumeDown" + "  " + "VolumeUp");
                Console.WriteLine("\n");
                Console.WriteLine("Example: SmartTVRemoteControl.exe -c Power");
                Console.WriteLine("\n");
                Console.ResetColor();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {

                var options = new Options();
                if (CommandLine.Parser.Default.ParseArguments(_args, options))
                {
                    // Values are available here
                    if (options.cmd != "" && options.cmd != null)
                    {
                        switch (options.cmd)
                        {
                            case "ButtonA":
                                targetDevice.CurrentSource.RemoteControl.ButtonA.ExecuteIfYouCan(null);
                                break;
                            case "ButtonB":
                                targetDevice.CurrentSource.RemoteControl.ButtonB.ExecuteIfYouCan(null);
                                break;
                            case "ButtonC":
                                targetDevice.CurrentSource.RemoteControl.ButtonC.ExecuteIfYouCan(null);
                                break;
                            case "ButtonD":
                                targetDevice.CurrentSource.RemoteControl.ButtonD.ExecuteIfYouCan(null);
                                break;
                            case "ChannelDown":
                                targetDevice.CurrentSource.RemoteControl.ChannelDown.ExecuteIfYouCan(null);
                                break;
                            case "ChannelList":
                                targetDevice.CurrentSource.RemoteControl.ChannelList.ExecuteIfYouCan(null);
                                break;
                            case "ChannelUp":
                                targetDevice.CurrentSource.RemoteControl.ChannelUp.ExecuteIfYouCan(null);
                                break;
                            case "DiscMenu":
                                targetDevice.CurrentSource.RemoteControl.DiscMenu.ExecuteIfYouCan(null);
                                break;
                            case "Exit":
                                targetDevice.CurrentSource.RemoteControl.Exit.ExecuteIfYouCan(null);
                                break;
                            case "FastForward":
                                targetDevice.CurrentSource.RemoteControl.FastForward.ExecuteIfYouCan(null);
                                break;
                            case "FastRewind":
                                targetDevice.CurrentSource.RemoteControl.FastRewind.ExecuteIfYouCan(null);
                                break;
                            case "Forward":
                                targetDevice.CurrentSource.RemoteControl.Forward.ExecuteIfYouCan(null);
                                break;
                            case "Guide":
                                targetDevice.CurrentSource.RemoteControl.Guide.ExecuteIfYouCan(null);
                                break;
                            case "Home":
                                targetDevice.CurrentSource.RemoteControl.Home.ExecuteIfYouCan(null);
                                break;
                            case "Info":
                                targetDevice.CurrentSource.RemoteControl.Info.ExecuteIfYouCan(null);
                                break;
                            case "JoystickDown":
                                targetDevice.CurrentSource.RemoteControl.JoystickDown.ExecuteIfYouCan(null);
                                break;
                            case "JoystickLeft":
                                targetDevice.CurrentSource.RemoteControl.JoystickLeft.ExecuteIfYouCan(null);
                                break;
                            case "JoystickOk":
                                targetDevice.CurrentSource.RemoteControl.JoystickOk.ExecuteIfYouCan(null);
                                break;
                            case "JoystickRight":
                                targetDevice.CurrentSource.RemoteControl.JoystickRight.ExecuteIfYouCan(null);
                                break;
                            case "JoystickUp":
                                targetDevice.CurrentSource.RemoteControl.JoystickUp.ExecuteIfYouCan(null);
                                break;
                            case "Keypad":
                                targetDevice.CurrentSource.RemoteControl.Keypad.ExecuteIfYouCan(null);
                                break;
                            case "MbrPower":
                                targetDevice.CurrentSource.RemoteControl.MbrPower.ExecuteIfYouCan(null);
                                break;
                            case "Menu":
                                targetDevice.CurrentSource.RemoteControl.Menu.ExecuteIfYouCan(null);
                                break;
                            case "Minus":
                                targetDevice.CurrentSource.RemoteControl.Minus.ExecuteIfYouCan(null);
                                break;
                            case "Mute":
                                targetDevice.CurrentSource.RemoteControl.Mute.ExecuteIfYouCan(null);
                                break;
                            case "Num0":
                                targetDevice.CurrentSource.RemoteControl.Num0.ExecuteIfYouCan(null);
                                break;
                            case "Num1":
                                targetDevice.CurrentSource.RemoteControl.Num1.ExecuteIfYouCan(null);
                                break;
                            case "Num2":
                                targetDevice.CurrentSource.RemoteControl.Num2.ExecuteIfYouCan(null);
                                break;
                            case "Num3":
                                targetDevice.CurrentSource.RemoteControl.Num3.ExecuteIfYouCan(null);
                                break;
                            case "Num4":
                                targetDevice.CurrentSource.RemoteControl.Num4.ExecuteIfYouCan(null);
                                break;
                            case "Num5":
                                targetDevice.CurrentSource.RemoteControl.Num5.ExecuteIfYouCan(null);
                                break;
                            case "Num6":
                                targetDevice.CurrentSource.RemoteControl.Num6.ExecuteIfYouCan(null);
                                break;
                            case "Num7":
                                targetDevice.CurrentSource.RemoteControl.Num7.ExecuteIfYouCan(null);
                                break;
                            case "Num8":
                                targetDevice.CurrentSource.RemoteControl.Num8.ExecuteIfYouCan(null);
                                break;
                            case "Num9":
                                targetDevice.CurrentSource.RemoteControl.Num9.ExecuteIfYouCan(null);
                                break;
                            case "Pause":
                                targetDevice.CurrentSource.RemoteControl.Pause.ExecuteIfYouCan(null);
                                break;
                            case "Play":
                                targetDevice.CurrentSource.RemoteControl.Play.ExecuteIfYouCan(null);
                                break;
                            case "Power":
                                targetDevice.CurrentSource.RemoteControl.Power.ExecuteIfYouCan(null);
                                break;
                            case "PreviousChannel":
                                targetDevice.CurrentSource.RemoteControl.PreviousChannel.ExecuteIfYouCan(null);
                                break;
                            case "Record":
                                targetDevice.CurrentSource.RemoteControl.Record.ExecuteIfYouCan(null);
                                break;
                            case "Return":
                                targetDevice.CurrentSource.RemoteControl.Return.ExecuteIfYouCan(null);
                                break;
                            case "Rewind":
                                targetDevice.CurrentSource.RemoteControl.Rewind.ExecuteIfYouCan(null);
                                break;
                            case "Search":
                                targetDevice.CurrentSource.RemoteControl.Search.ExecuteIfYouCan(null);
                                break;
                            case "SmartHub":
                                targetDevice.CurrentSource.RemoteControl.SmartHub.ExecuteIfYouCan(null);
                                break;
                            case "StbMenu":
                                targetDevice.CurrentSource.RemoteControl.StbMenu.ExecuteIfYouCan(null);
                                break;
                            case "Stop":
                                targetDevice.CurrentSource.RemoteControl.Stop.ExecuteIfYouCan(null);
                                break;
                            case "SubTitle":
                                targetDevice.CurrentSource.RemoteControl.SubTitle.ExecuteIfYouCan(null);
                                break;
                            case "Tools":
                                targetDevice.CurrentSource.RemoteControl.Tools.ExecuteIfYouCan(null);
                                break;
                            case "VolumeDown":
                                targetDevice.CurrentSource.RemoteControl.VolumeDown.ExecuteIfYouCan(null);
                                break;
                            case "VolumeUp":
                                targetDevice.CurrentSource.RemoteControl.VolumeUp.ExecuteIfYouCan(null);
                                break;
                            default:
                                break;
                        }
                        Console.WriteLine("TV command execution complete. Exiting in 2 seconds");
                        await Task.Delay(2000);
                        Environment.Exit(0);
                    }
                }
            }
        }
        
        private static void DrawMenu(DeviceController deviceController)
        {

        }

        private static async void goToDiscovery(DeviceController deviceController)
        {
            deviceController.StopDiscovery();
            Console.WriteLine("Select the TV you want to connect to from the list below.");
            int num = 1;
            string selectedDeviceName = "";
            List<UPnP.DataContracts.DeviceInfo> list = Devices.ToList();
            foreach (DeviceInfo deviceInfo in list)
            {
                Console.WriteLine(num + "." + deviceInfo.FriendlyName);
                num++;
            }

            int selector = 0;
            bool good = false;
            //Console.Clear();
            DrawMenu(deviceController);
            good = int.TryParse(Console.ReadLine(), out selector);
            if (good)
            {
                selectedDeviceName = list[selector -1].FriendlyName;
                Console.WriteLine("Selected TV: " + selectedDeviceName);
            }
            else
            {
                Console.WriteLine("Error while parsing device selection");
            }
            await ConnectToDevice(list[selector - 1]);
        }
        
        private static async Task ConnectToDevice(object param)
        {
            try
            {
                try
                {
                    Console.WriteLine("Connecting to selected TV now");
                    DeviceInfo deviceInfo = param as DeviceInfo;
                    if (deviceInfo == null)
                    {
                        return;
                    }
                    else if (!TvDiscovery.IsTv2014(deviceInfo))
                    {
                        Console.WriteLine("TV model is not supported. Must be atleast 2014 model.");
                        return;
                    }
                    else if (await deviceController.TryToConnect(deviceInfo.DeviceAddress.Host))
                    {
                        Console.WriteLine("Selected TV connected successfully");
                        goToRemoteControl(deviceController);
                    }
                    else
                    {
                        switch (await deviceController.Connect(deviceInfo))
                        {
                            case DeviceController.ConnectResult.PinPageAlreadyShown:
                                {
                                    //goToRemoteControl(deviceController);
                                    //Console.WriteLine("Another device is connecting. Please try again.");
                                    //goToDiscovery(deviceController);
                                    return;
                                }
                            case DeviceController.ConnectResult.SocketException:
                                {
                                    Console.WriteLine("Socket exception during connection attempt.");
                                    deviceController.RefreshDiscovery();
                                    return;
                                }
                            case DeviceController.ConnectResult.OtherException:
                                {
                                    Console.WriteLine("Unknown exception during connection attempt.");
                                    goToDiscovery(deviceController);
                                    return;
                                }
                            default:
                                {
                                    Console.WriteLine("Enter PIN from TV within 30 seconds.");
                                    string pin = Console.ReadLine();

                                    if (await OnPair(pin))
                                    {
                                        Console.WriteLine("Pairing attempt succeeded. Launching remote control interface now.");
                                        goToRemoteControl(deviceController);
                                    } else
                                    {
                                        Console.WriteLine("Error during pairing attempt. Please try again.");
                                        goToDiscovery(deviceController);
                                    }
                                    break;
                                }
                        }
                    }
                }
                catch (Exception exception2)
                {
                    Console.WriteLine("Unknown exception occurred during pairing attempt. Please try again.");
                    goToDiscovery(deviceController);
                }
            }
            finally
            {
                Console.WriteLine("deviceController.Connect call complete");
            }
        }
        
        private static async void goToChangeTv(DeviceController deviceController)
        {
            Console.WriteLine("Checking for available networks");
            networkNames = GetNetworksNames();
            Console.WriteLine("Preparing for TV pairing");
            loadPinInfo(deviceController);
            _devices = (
                from o in deviceController.Devices
                where o != deviceController.CurrentDeviceInfo
                select o).ToArray<DeviceInfo>();
            Console.WriteLine("Device count after change TV discovery: " + Devices.Count().ToString());
            Console.WriteLine("TODO - I need to reference the device and request pin auth now");
            if (Devices.Count<DeviceInfo>() == 0)
            {
                Console.WriteLine("No devices found. Refreshing now");
                OnRefresh(null);
            }
            goToDiscovery(deviceController);
        }

        private static void OnRefresh(object obj)
        {
            deviceController.RefreshDiscovery();
            Console.WriteLine("Device refresh completed");
        }

        private static async void loadDevice(DeviceController deviceController)
        {
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
            Devices = deviceController.Devices;
            Devices.CollectionChanged += new NotifyCollectionChangedEventHandler(Devices_CollectionChanged);
            Console.WriteLine("uPnP device count after device discovery: " + Devices.Count().ToString());
            CheckDeviceAvailable();
        }

        private static void CheckDeviceAvailable()
        {
            isDeviceAvailable = Devices.Count<DeviceInfo>() > 0;
            Console.WriteLine("uPnP device found?: " + isDeviceAvailable);
            goToDiscovery(deviceController);
        }

        private static async void loadPinInfo(DeviceController deviceController)
        {
            pinTimeOutTimer = new DispatcherTimer();
            pinTimeOutTimer.Tick += new EventHandler(pinTimeOutTimer_Tick);
            invalidPinCount = 0;
            pinErrorMessage = string.Empty;
        }

        private static async Task<bool> OnPair(string pin)
        {
           if (await deviceController.SetPin(pin))
                        {
                            Console.WriteLine("TV pairing succeeded");
                            isPaired = true;
                            return true;
                        }
                        invalidPinCount = invalidPinCount + 1;
                        pinErrorMessage = "Incorrect pin entered. Please try again";
                        ResetPin();
                        if (invalidPinCount >= maxInvalidCount)
                        {
                            RestartTimer();
                            Console.WriteLine("Invalid pin entered " + invalidPinCount + " times. Please try again");
                            isPaired = false;
                            ResetPin();
                            return false;
                        }
            return false;
        }

        private static async void pinTimeOutTimer_Tick(object sender, EventArgs e)
        {
            await deviceController.ClosePinPageOnTV();
            StopTimer();
            Console.WriteLine("Try again.Pairing time limit expired");
        }

        private static void ResetPin()
        {
            pinNumbers = new string[4];
            isPinFocused = false;
            isPinFocused = true;
        }

        private static void RestartTimer()
        {
            pinTimeOutTimer.Stop();
            pinTimeOutTimer.Start();
        }

        private static void StartTimer()
        {
            pinTimeOutTimer.Interval = TimeSpan.FromSeconds(120);
            pinTimeOutTimer.Start();
        }

        private static void StopTimer()
        {
            pinTimeOutTimer.Stop();
        }

        private static async void deviceController_CurrentDeviceDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Device disconnected");
            bool flag = deviceController == null;
            await deviceController.DisconnectAsync(true);
            Console.WriteLine("TV is no longer connected. Initiating discovery now.");
            goToDiscovery(deviceController);
        }

        private static void Devices_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("Device collection changed");
            _devices = (
                from p in deviceController.Devices
                where p != deviceController.CurrentDeviceInfo
                select p).ToArray<DeviceInfo>();
            Console.WriteLine("Device count after refresh: " + Devices.Count().ToString());
        }

        private static void OnChangeTV(object obj)
        {
            goToChangeTv(deviceController);
        }

        private static string GetNetworksNames()
        {
            List<string> strs = new List<string>();
            IEnumerable<NetworkInterface> networkInterfaces = ((IEnumerable<NetworkInterface>)NetworkInterface.GetAllNetworkInterfaces()).Where<NetworkInterface>((NetworkInterface arg) => {
                if (arg.OperationalStatus != OperationalStatus.Up)
                {
                    return false;
                }
                return arg.NetworkInterfaceType == NetworkInterfaceType.Ethernet;
            });
            if (networkInterfaces.Count<NetworkInterface>() != 0)
            {
                strs.Add("Wired");
            }
            foreach (WlanInterfaceInfo wlanInterfaceInfo in WlanApiWrapper.EnumInterfaces())
            {
                WlanConnectionInfo currentConnection = WlanApiWrapper.GetCurrentConnection(wlanInterfaceInfo.Guid);
                if (currentConnection == null)
                {
                    continue;
                }
                strs.Add(string.Format("<<{0}>>", currentConnection.ProfileName));
            }
            return string.Join(", ", strs);
        }

        private static async void DisconnectAsync()
        {
            Console.WriteLine("WARNING --- No device to disconnected");
            if (deviceController != null)
            {
                Console.WriteLine("Device disconnect started");
                await deviceController.DisconnectAsync(false);
            }
            Console.WriteLine("Device disconnect completed");
        }

        private static void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Network address changed");
            networkNames = GetNetworksNames();
        }

        private static void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            Console.WriteLine("Network availability changed");
            networkNames = GetNetworksNames();
        }

    }
}
