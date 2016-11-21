using Networking.Native;
using SmartView2.Core;
using SmartView2.Devices;
using SmartView2.Devices.RemoteControls;
using SmartView2.Devices.SecondTv;
using SmartTVRemoteControl.Native.DLNA;
using SmartTVRemoteControl.Native.HTTP;
using SmartView2.Native.MediaLibrary;
using SmartTVRemoteControl.Native.MultiScreen;
using System;
using UPnP.DataContracts;
using System.Windows.Threading;
using System.Threading;

namespace SmartTVRemoteControl
{

    public static class DeviceFactory
    {
        internal static Thread CreateDispatcher()
        {
            ManualResetEvent dispatcherReadyEvent = new ManualResetEvent(false); var dispatcherThread = new Thread(() => {
                // This is here just to force the dispatcher infrastructure to be setup on this thread 
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => { }));
                // Run the dispatcher so it starts processing the message loop 
                dispatcherReadyEvent.Set(); Dispatcher.Run();
            });
            dispatcherThread.SetApartmentState(ApartmentState.STA);
            dispatcherThread.IsBackground = true;
            dispatcherThread.Start();
            dispatcherReadyEvent.WaitOne();
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
            return dispatcherThread;
        }

        private static IBaseDispatcher _dispatcher;
        public static ITargetDevice CreateTvDevice(DeviceInfo deviceInfo, IPlayerNotificationProvider playerNotification, IBaseDispatcher dispatcher)
        {
            return DeviceFactory.CreateTvDevice(deviceInfo, playerNotification, dispatcher, new NoSecurityProvider());
        }

        internal static ITargetDevice CreateTvDevice(DeviceInfo deviceInfo, IPlayerNotificationProvider playerNotification, IBaseDispatcher dispatcher, ISecondTvSecurityProvider securityProvider)
        {
            if (deviceInfo == null)
            {
                throw new ArgumentNullException("deviceInfo");
            }
            if (playerNotification == null)
            {
            }
            if (dispatcher == null)
            {
                Console.WriteLine("Dispatcher is null");
            }
            //Dispatcher backgroundSerialQeueue = new Dispatcher;
        SecondTvSyncTransport secondTvSyncTransport = new SecondTvSyncTransport(securityProvider);
            SecondTvAsyncTransport secondTvAsyncTransport = new SecondTvAsyncTransport(securityProvider);
            secondTvSyncTransport.Connect(deviceInfo.DeviceAddress);
            secondTvAsyncTransport.Connect(deviceInfo.DeviceAddress);
            SecondTv secondTv = new SecondTv(secondTvSyncTransport, deviceInfo.LocalAddress);
            SecondTvRemoteInput secondTvRemoteInput = new SecondTvRemoteInput(secondTvAsyncTransport, deviceInfo.LocalAddress);
            secondTvRemoteInput.Connect(secondTvSyncTransport);
            MbrKeySender mbrKeySender = new MbrKeySender(secondTvSyncTransport, deviceInfo.LocalAddress);
            RemoteControlFactory remoteControlFactory = new RemoteControlFactory(secondTvRemoteInput, mbrKeySender);

            HttpServer _https = new HttpServer();
            MultiScreenController _msc = new MultiScreenController();
            DlnaServer _dlnas = new DlnaServer();
            DataLibrary _dl = new DataLibrary(_dispatcher);

                return new TvDevice(deviceInfo, secondTv, playerNotification, secondTvRemoteInput, remoteControlFactory, dispatcher, _https, _msc, _dlnas, _dl);
        }

        static void DoWork(object startArg)
        {
            Dispatcher targetDispatcher = startArg as Dispatcher;
            if (targetDispatcher == null)
            {
                // Log error  
                return;
            }

            while (true)
            {
                //targetDispatcher.BeginInvoke(....);
            }

            // A safer strategy is to post a background item that decrements a counter  
            // then the targetDispatcher can shut itself down when there are no more workers  
            //targetDispatcher.BeginInvokeShutdown(...);
        }
    }
}