using SmartView2.Core;
using SmartView2.Devices;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Interop;

namespace SmartTVRemoteControl.Native.Player
{
	public class PlayerNotificationProvider : IPlayerNotificationProvider
	{
		//private HwndSource source;

		private System.Timers.Timer streamResolutionChangedTimer;

		private System.Timers.Timer streamAudioSampleRateTimer;

		private System.Timers.Timer streamVersionChangedTimer;

		private System.Timers.Timer streamPmtInfoChangedTimer;

		private bool turnOnStreamResolutionChanged;

		private bool turnOnStreamAudioSampleRate;

		private bool turnOnStreamVersionChanged;

		private bool turnOnStreamPmtInfoChanged;

		public PlayerNotificationProvider()
		{
			
		}

		private void HandleEvent(EventHandler<EventArgs> riseEvent, System.Timers.Timer timer, bool eventFlag)
		{
			if (!eventFlag)
			{
				timer.Start();
				return;
			}
			timer.Stop();
			if (riseEvent != null)
			{
				riseEvent(this, null);
				timer.Start();
				eventFlag = false;
			}
		}

		private IntPtr HandleMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case 1025:
				{
					Logger.Instance.LogErrorFormat(string.Format("STREAM_RESOLUTION_CHANGED - {0}", msg));
					this.OnStreamResolutionChanged(this, null);
					handled = true;
					break;
				}
				case 1026:
				{
					Logger.Instance.LogErrorFormat(string.Format("STREAM_AUDIO_SAMPLE_RATE_CHANGED - {0}", msg));
					this.OnStreamAudioSampleRateChanged(this, null);
					handled = true;
					break;
				}
				case 1027:
				{
					Logger.Instance.LogErrorFormat(string.Format("STREAM_VERSION_CHANGED - {0}", msg));
					this.OnStreamVersionChanged(this, null);
					handled = true;
					break;
				}
				case 1028:
				{
					Logger.Instance.LogErrorFormat(string.Format("STREAM_PMT_INFO_CHANGED - {0}", msg));
					this.OnStreamPmtInfoChanged(this, null);
					handled = true;
					break;
				}
				case 1029:
				{
					switch ((int)wParam)
					{
						case 1:
						{
							Logger.Instance.LogErrorFormat(string.Format("STREAM_MEDIA_AUDIO_VIDEO - {0}", msg));
							StreamMediaEventArgs streamMediaEventArg = new StreamMediaEventArgs()
							{
								IsOk = true,
								StreamMedia = StreamMedia.AudioVideo
							};
							this.OnStreamMediaChanged(this, streamMediaEventArg);
							break;
						}
						case 2:
						{
							Logger.Instance.LogErrorFormat(string.Format("STREAM_MEDIA_AUDIO_ONLY - {0}", msg));
							StreamMediaEventArgs streamMediaEventArg1 = new StreamMediaEventArgs()
							{
								IsOk = true,
								StreamMedia = StreamMedia.AudioOnly
							};
							this.OnStreamMediaChanged(this, streamMediaEventArg1);
							break;
						}
						case 3:
						{
							Logger.Instance.LogErrorFormat(string.Format("STREAM_MEDIA_INVALID_NO_SIGNAL - {0}", msg));
							StreamMediaEventArgs streamMediaEventArg2 = new StreamMediaEventArgs()
							{
								IsOk = false,
								StreamMedia = StreamMedia.NoSignal
							};
							this.OnStreamMediaChanged(this, streamMediaEventArg2);
							break;
						}
						case 4:
						{
							Logger.Instance.LogErrorFormat(string.Format("STREAM_MEDIA_INVALID_LOCKED - {0}", msg));
							StreamMediaEventArgs streamMediaEventArg3 = new StreamMediaEventArgs()
							{
								IsOk = false,
								StreamMedia = StreamMedia.Locked
							};
							this.OnStreamMediaChanged(this, streamMediaEventArg3);
							break;
						}
						case 5:
						{
							Logger.Instance.LogErrorFormat(string.Format("STREAM_MEDIA_INVALID_SERVICE_NOT_AVAILABLE - {0}", msg));
							StreamMediaEventArgs streamMediaEventArg4 = new StreamMediaEventArgs()
							{
								IsOk = false,
								StreamMedia = StreamMedia.ServiceNotAvailable
							};
							this.OnStreamMediaChanged(this, streamMediaEventArg4);
							break;
						}
						case 6:
						{
							Logger.Instance.LogErrorFormat(string.Format("STREAM_MEDIA_INVALID_3D_NOT_AVAILABLE - {0}", msg));
							StreamMediaEventArgs streamMediaEventArg5 = new StreamMediaEventArgs()
							{
								IsOk = false,
								StreamMedia = StreamMedia.NotAvailable3D
							};
							this.OnStreamMediaChanged(this, streamMediaEventArg5);
							break;
						}
						case 7:
						{
							Logger.Instance.LogErrorFormat(string.Format("STREAM_MEDIA_INVALID_SCRAMBLED_CHANNEL - {0}", msg));
							StreamMediaEventArgs streamMediaEventArg6 = new StreamMediaEventArgs()
							{
								IsOk = false,
								StreamMedia = StreamMedia.ScrambledChannel
							};
							this.OnStreamMediaChanged(this, streamMediaEventArg6);
							break;
						}
						case 8:
						{
							Logger.Instance.LogErrorFormat(string.Format("STREAM_MEDIA_INVALID_DATA_SERVICE - {0}", msg));
							StreamMediaEventArgs streamMediaEventArg7 = new StreamMediaEventArgs()
							{
								IsOk = false,
								StreamMedia = StreamMedia.DataService
							};
							this.OnStreamMediaChanged(this, streamMediaEventArg7);
							break;
						}
						case 9:
						{
							Logger.Instance.LogErrorFormat(string.Format("STREAM_MEDIA_INVALID_CHECK_CABLE - {0}", msg));
							StreamMediaEventArgs streamMediaEventArg8 = new StreamMediaEventArgs()
							{
								IsOk = false,
								StreamMedia = StreamMedia.CheckCable
							};
							this.OnStreamMediaChanged(this, streamMediaEventArg8);
							break;
						}
						case 10:
						{
							Logger.Instance.LogErrorFormat(string.Format("STREAM_MEDIA_INVALID_ADULT_SCENE_BLOCK - {0}", msg));
							StreamMediaEventArgs streamMediaEventArg9 = new StreamMediaEventArgs()
							{
								IsOk = false,
								StreamMedia = StreamMedia.AdultSceneBlock
							};
							this.OnStreamMediaChanged(this, streamMediaEventArg9);
							break;
						}
						case 11:
						{
							Logger.Instance.LogErrorFormat(string.Format("STREAM_MEDIA_INVALID_PARENTAL_LOCK - {0}", msg));
							StreamMediaEventArgs streamMediaEventArg10 = new StreamMediaEventArgs()
							{
								IsOk = false,
								StreamMedia = StreamMedia.ParentalLock
							};
							this.OnStreamMediaChanged(this, streamMediaEventArg10);
							break;
						}
						case 12:
						{
							Logger.Instance.LogErrorFormat(string.Format("STREAM_MEDIA_INVALID_NOT_AVAILABLE - {0}", msg));
							StreamMediaEventArgs streamMediaEventArg11 = new StreamMediaEventArgs()
							{
								IsOk = false,
								StreamMedia = StreamMedia.NotAvailable
							};
							this.OnStreamMediaChanged(this, streamMediaEventArg11);
							break;
						}
					}
					handled = true;
					break;
				}
				case 1030:
				{
					byte[] numArray = new byte[16];
					Marshal.Copy(wParam, numArray, 0, 16);
					Marshal.FreeCoTaskMem(wParam);
					this.OnCCDataReceived(this, new CCDataEventArgs()
					{
						Data = numArray
					});
					handled = true;
					break;
				}
				case 1031:
				{
					byte[] numArray1 = new byte[lParam.ToInt32()];
					Marshal.Copy(wParam, numArray1, 0, lParam.ToInt32());
					Marshal.FreeCoTaskMem(wParam);
					string str = Encoding.ASCII.GetString(numArray1, 0, lParam.ToInt32());
					Logger.Instance.LogErrorFormat(string.Format("VIDEO_URL_RECEIVED: {0}", str));
					handled = true;
					break;
				}
				case 1032:
				{
					Logger.Instance.LogErrorFormat("VIDEO_INVALID_FORMAT");
					handled = true;
					this.OnStreamVideoStatusChanged(this, "VIDEO_INVALID_FORMAT");
					break;
				}
				case 1033:
				{
					Logger.Instance.LogErrorFormat("VIDEO_PLAYBACK_STARTED");
					handled = true;
					this.OnStreamVideoStatusChanged(this, "VIDEO_PLAYBACK_STARTED");
					break;
				}
				case 1034:
				{
					Logger.Instance.LogErrorFormat("VIDEO_PLAYBACK_STOPPED");
					handled = true;
					this.OnStreamVideoStatusChanged(this, "VIDEO_PLAYBACK_STOPPED");
					break;
				}
				case 1035:
				{
					Logger.Instance.LogErrorFormat("VIDEO_PLAYBACK_SHUTDOWN");
					handled = true;
					this.OnStreamVideoStatusChanged(this, "VIDEO_PLAYBACK_SHUTDOWN");
					this.OnVideoShutDown(this, new EventArgs());
					break;
				}
				case 1036:
				{
					Logger.Instance.LogErrorFormat("VIDEO_STREAM_EOS");
					handled = true;
					break;
				}
			}
			if (!handled)
			{
				return IntPtr.Zero;
			}
			return new IntPtr(1);
		}

		private void OnCCDataReceived(object sender, CCDataEventArgs e)
		{
			if (this.CCDataReceived != null)
			{
				this.CCDataReceived(sender, e);
			}
		}

		private void OnStreamAudioSampleRateChanged(object sender, EventArgs e)
		{
			this.HandleEvent(this.StreamAudioSampleRateChanged, this.streamAudioSampleRateTimer, this.turnOnStreamAudioSampleRate);
		}

		private void OnStreamMediaChanged(object sender, StreamMediaEventArgs e)
		{
			EventHandler<StreamMediaEventArgs> eventHandler = this.StreamMediaChanged;
			if (eventHandler != null)
			{
				eventHandler(sender, e);
			}
		}

		private void OnStreamPmtInfoChanged(object sender, EventArgs e)
		{
			this.HandleEvent(this.StreamPmtInfoChanged, this.streamPmtInfoChangedTimer, this.turnOnStreamPmtInfoChanged);
		}

		private void OnStreamResolutionChanged(object sender, EventArgs e)
		{
			this.HandleEvent(this.StreamResolutionChanged, this.streamResolutionChangedTimer, this.turnOnStreamResolutionChanged);
		}

		private void OnStreamVersionChanged(object sender, EventArgs e)
		{
			this.HandleEvent(this.StreamVersionChanged, this.streamVersionChangedTimer, this.turnOnStreamVersionChanged);
		}

		private void OnStreamVideoStatusChanged(object sender, string args)
		{
			EventHandler<string> eventHandler = this.StreamVideoStatusChanged;
			if (eventHandler != null)
			{
				eventHandler(sender, args);
			}
		}

		private void OnVideoShutDown(object sender, EventArgs args)
		{
			EventHandler eventHandler = this.VideoShutDown;
			if (eventHandler != null)
			{
				eventHandler(sender, args);
			}
		}

		private void streamAudioSampleRateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.turnOnStreamAudioSampleRate = true;
		}

		private void streamPmtInfoChangedTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.turnOnStreamPmtInfoChanged = true;
		}

		private void streamResolutionChangedTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.turnOnStreamResolutionChanged = true;
		}

		private void streamVersionChangedTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.turnOnStreamVersionChanged = true;
		}

		public event EventHandler<CCDataEventArgs> CCDataReceived;

		public event EventHandler<EventArgs> StreamAudioSampleRateChanged;

		public event EventHandler<StreamMediaEventArgs> StreamMediaChanged;

		public event EventHandler<EventArgs> StreamPmtInfoChanged;

		public event EventHandler<EventArgs> StreamResolutionChanged;

		public event EventHandler<EventArgs> StreamVersionChanged;

		public event EventHandler<string> StreamVideoStatusChanged;

		public event EventHandler VideoShutDown;
	}
}