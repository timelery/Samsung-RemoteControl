using com.samsung.multiscreen.application;
using com.samsung.multiscreen.device;
using com.samsung.multiscreen.device.requests;
using com.samsung.multiscreen.device.requests.impl;
using MediaLibrary.DataModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartView2.Core;
using SmartTVRemoteControl.Native.HTTP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTVRemoteControl.Native.MultiScreen
{
    public class MultiScreenController : IMultiScreen, IDisposable, INotifyPropertyChanged
    {
        private const string applicationName = "Mediashare";
        private const string channelId = "multiscreen_media_share";
        private const string MESSAGE_TYPE_ADD_ITEMS = "mediaQueuePlayer.addItems";
        private const string MESSAGE_TYPE_REMOVE_ITEMS = "mediaQueuePlayer.removeItems";
        private const string MESSAGE_TYPE_MOVE_ITEMS = "mediaQueuePlayer.moveItem";
        private const string MESSAGE_TYPE_GET_QUEUE = "mediaQueuePlayer";
        private const string MESSAGE_TYPE_QUEUE_PLAY = "mediaQueuePlayer.play";
        private const string MESSAGE_TYPE_QUEUE_PAUSE = "mediaQueuePlayer.pause";
        private const string MESSAGE_TYPE_QUEUE_STOP = "mediaQueuePlayer.stop";
        private const string MESSAGE_TYPE_INFO = "multiScreenMediaShare.info";
        private const string MESSAGE_TYPE_NEXT = "mediaQueuePlayer.next";
        private const string MESSAGE_TYPE_PANEL_PLAY = "mediaPanelPlayer.play";
        private const string MESSAGE_TYPE_PANEL_PAUSE = "mediaPanelPlayer.pause";
        private const string MESSAGE_TYPE_PANEL_STOP = "mediaPanelPlayer.stop";
        private const string MESSAGE_TYPE_PANEL_ENDED = "mediaPanelPlayer.ended";
        private const string MESSAGE_TYPE_CURRENT_ITEM = "mediaQueuePlayer.changeCurrentItem";
        private const string MESSAGE_TYPE_SET_CURRENT_TIME = "mediaPanelPlayer.setCurrentTime";
        private const string MESSAGE_TYPE_SET_MEDIA = "mediaPanel.setMedia";
        private const string MESSAGE_TYPE_CURRENT_MEDIA = "mediaPanel.currentMedia";
        private const string MESSAGE_TYPE_TIME_UPDATE = "mediaPanelPlayer.timeupdate";
        private const string MESSAGE_TYPE_PUSHED_MEDIA = "media.new";
        private const string MESSAGE_TYPE_PLAYER_ERROR = "currentMediaPlayer.error";
        private const string MESSAGE_TYPE_PANEL_ERROR = "mediaPanelPlayer.error";
        private const string MESSAGE_TYPE_PANEL_NOW_PLAYING_CLEAR = "mediaPanelPlayer.clearNP";
        private const string MESSAGE_TYPE_SET_CONFIG = "mediaQueuePlayer.setConfig";
        private DeviceURIResult deviceUri;
        private Device connectedDevice;
        private Application deviceApp;
        private com.samsung.multiscreen.channel.Channel appChannel;
        private DeviceResult<Device> deviceResultByDevice;
        private MultiScreenResult<Application> applicationResult;
        private MultiScreenResult<bool> boolResult;
        private DeviceResult<com.samsung.multiscreen.channel.Channel> deviceResultByChannel;
        private ChannelListner channelListner;
        private HttpServer httpServer;
        private readonly List<Content> shareFiles;
        private string targetAddress;
        private Content currentMediaContent;
        private IEnumerable<Content> mediaQueue;
        private IMediaTimeInfo currentMediaTimeInfo;
        private LoadState loadState;
        private bool updatedTime;
        private MediaState mediaState;
        private MediaState queueState;
        private string queueSpeed;
        private string queueEffect;
        private Queue<Tuple<Content, bool>> loadingQueue;

        public Content CurrentMediaContent
        {
            get
            {
                return this.currentMediaContent;
            }
            private set
            {
                this.currentMediaContent = value;
                this.OnMultiscreenCurrentMediaContentUpdated();
                this.OnPropertyChanged(this, "CurrentMediaContent");
            }
        }

        public IEnumerable<Content> MediaQueue
        {
            get
            {
                return this.mediaQueue;
            }
            private set
            {
                this.mediaQueue = value;
                this.OnPropertyChanged(this, "MediaQueue");
                this.OnPropertyChanged(this, "MediaQueueCount");
            }
        }

        public int MediaQueueCount
        {
            get
            {
                if (this.mediaQueue == null)
                    return 0;
                return Enumerable.Count<Content>(this.mediaQueue);
            }
        }

        public IMediaTimeInfo CurrentMediaTimeInfo
        {
            get
            {
                return this.currentMediaTimeInfo;
            }
            set
            {
                this.currentMediaTimeInfo = value;
                this.OnPropertyChanged(this, "CurrentMediaTimeInfo");
            }
        }

        public LoadState LoadState
        {
            get
            {
                return this.loadState;
            }
            private set
            {
                this.loadState = value;
                this.OnPropertyChanged(this, "LoadState");
            }
        }

        public MediaState MediaState
        {
            get
            {
                return this.mediaState;
            }
            private set
            {
                this.mediaState = value;
                this.OnPropertyChanged(this, "MediaState");
            }
        }

        public MediaState QueueState
        {
            get
            {
                return this.queueState;
            }
            private set
            {
                this.queueState = value;
                this.OnPropertyChanged(this, "QueueState");
            }
        }

        public string QueueSpeed
        {
            get
            {
                return this.queueSpeed;
            }
            private set
            {
                this.queueSpeed = value;
                this.OnPropertyChanged(this, "QueueSpeed");
            }
        }

        public string QueueEffect
        {
            get
            {
                return this.queueEffect;
            }
            private set
            {
                this.queueEffect = value;
                this.OnPropertyChanged(this, "QueueEffect");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler MultiscreenDisconnected;

        public event EventHandler MultiscreenQueueEnded;

        public event EventHandler MultiscreenQueueUpdated;

        public event EventHandler MultiscreenStartFailed;

        public event EventHandler PushToTvEnded;

        public event EventHandler<MediaQueueEventArgs> PushToTvQueueEnded;

        public event EventHandler MultiscreenCurrentMediaContentUpdated;

        public event EventHandler MultiScreenContentBroken;

        public event EventHandler MultiScreenContentFailed;

        public event EventHandler MultiScreenContentNotSupported;

        public MultiScreenController()
        {
            Console.WriteLine("Create MultiScreenController");
            //this.shareFiles = new List<Content>();
            //this.deviceResultByDevice = new DeviceResult<Device>();
            //this.deviceResultByDevice.OnSuccess += new EventHandler<Device>(this.deviceResultByDevice_OnSuccess);
            //this.deviceResultByDevice.OnFailed += new EventHandler<DeviceError>(this.deviceResultByDevice_OnFailed);
            //this.applicationResult = new MultiScreenResult<Application>();
            //this.applicationResult.OnSuccess += new EventHandler<Application>(this.applicationResult_OnSuccess);
            //this.applicationResult.OnFailed += new EventHandler<ApplicationError>(this.applicationResult_OnFailed);
            //this.boolResult = new MultiScreenResult<bool>();
            //this.boolResult.OnSuccess += new EventHandler<bool>(this.boolResult_OnSuccess);
            //this.boolResult.OnFailed += new EventHandler<ApplicationError>(this.boolResult_OnFailed);
            //this.deviceResultByChannel = new DeviceResult<com.samsung.multiscreen.channel.Channel>();
            //this.deviceResultByChannel.OnSuccess += new EventHandler<com.samsung.multiscreen.channel.Channel>(this.deviceResultByChannel_OnSuccess);
            //this.deviceResultByChannel.OnFailed += new EventHandler<DeviceError>(this.deviceResultByChannel_OnFailed);
            //this.channelListner = new ChannelListner();
            //this.channelListner.MessageReceived += new EventHandler<string>(this.channelListner_MessageReceived);
            //this.channelListner.DisconnectReceived += new EventHandler(this.channelListner_DisconnectReceived);
            //this.LoadState = LoadState.Stoped;
            //this.loadingQueue = new Queue<Tuple<Content, bool>>();
        }

        public async Task InitializeAsync(string localAddress, string targetAddress, IHttpServer httpServer)
        {
            Console.WriteLine("InitializeAsync started...");
            Console.WriteLine("LoadState is {0}", (object)this.LoadState.ToString());
            /*if (this.LoadState == LoadState.Stoped || this.LoadState == LoadState.Failed)
            {
                if (string.IsNullOrWhiteSpace(localAddress))
                    throw new ArgumentNullException("localAddress is null");
                if (string.IsNullOrWhiteSpace(targetAddress))
                    throw new ArgumentNullException("tvAddress is null");
                this.targetAddress = targetAddress;
                this.deviceUri = new DeviceURIResult(new Uri(string.Format("http://{0}:8001/ms/1.0/", (object)targetAddress)), new Uri(string.Format("http://{0}:8080/ws/app/", (object)targetAddress)));
                this.httpServer = httpServer as HttpServer;
            }*/
        }

        public void Close()
        {
            Console.WriteLine("Close started...");
            Console.WriteLine("LoadState is {0}", (object)this.LoadState.ToString());
            /*if (this.LoadState != LoadState.Loaded)
                return;
            this.QueueState = MediaState.Stop;
            this.appChannel.Disconnect();
            this.LoadState = LoadState.Stoped;*/
        }

        public void PushMediaToTv(Content file)
        {
            Console.WriteLine("PushMediaToTv started...");
            /*this.TryStartMultiscreenApp();
            if (this.LoadState == LoadState.Loading)
            {
                this.loadingQueue.Enqueue(new Tuple<Content, bool>(file, false));
            }
            else
            {
                if (this.LoadState != LoadState.Loaded || file == null)
                    return;
                if (Enumerable.FirstOrDefault<Content>((IEnumerable<Content>)this.shareFiles, (Func<Content, bool>)(f => f.ID == file.ID)) == null)
                    this.shareFiles.Add(file);
                if (!File.Exists(file.Path))
                {
                    this.OnMultiScreenContentBroken();
                }
                else
                {
                    this.OnPushToTvEnded();
                    this.appChannel.SendToHost(JsonConvert.SerializeObject(new
                    {
                        type = "media.new",
                        media = this.CreateMediaContent(this.httpServer.ServerUrl, file)
                    }).ToString());
                }
            }*/
        }

        public void PushMediaToTvQueue(Content file)
        {
            /*this.TryStartMultiscreenApp();
            if (this.LoadState == LoadState.Loading)
            {
                this.loadingQueue.Enqueue(new Tuple<Content, bool>(file, true));
            }
            else
            {
                if (this.LoadState != LoadState.Loaded || file == null)
                    return;
                this.PushMediaToTvQueue((IEnumerable<Content>)new Content[1]
                {
          file
                });
            }*/
        }

        public void PushMediaToTvQueue(IEnumerable<Content> files)
        {
            Console.WriteLine("PushMediaToTv PushMediaToTvQueue...");
            /*this.TryStartMultiscreenApp();
            if (this.LoadState == LoadState.Loading)
            {
                if (files == null)
                    return;
                foreach (Content content in files)
                    this.loadingQueue.Enqueue(new Tuple<Content, bool>(content, true));
            }
            else
            {
                if (this.LoadState != LoadState.Loaded || files == null)
                    return;
                foreach (Content content in files)
                {
                    Content file = content;
                    if (Enumerable.FirstOrDefault<Content>((IEnumerable<Content>)this.shareFiles, (Func<Content, bool>)(f => f.ID == file.ID)) == null)
                        this.shareFiles.Add(file);
                }
                int num = 0;
                bool flag = false;
                List<object> list = new List<object>();
                foreach (Content file in files)
                {
                    if (!File.Exists(file.Path))
                    {
                        flag = true;
                    }
                    else
                    {
                        MediaContent mediaContent = this.CreateMediaContent(this.httpServer.ServerUrl, file);
                        var fAnonymousType1 = new
                        {
                            index = num,
                            media = mediaContent
                        };
                        list.Add(fAnonymousType1);
                        ++num;
                    }
                }
                int addedFiles = this.mediaQueue == null ? Enumerable.Count<Content>(files) : Enumerable.Count<Content>(Enumerable.Except<Content>(files, this.MediaQueue, (IEqualityComparer<Content>)new ContentCompare()));
                int repeatedFiles = Enumerable.Count<Content>(files) - addedFiles;
                this.OnPushToTvQueueEnded(addedFiles, repeatedFiles);
                this.appChannel.SendToHost(JsonConvert.SerializeObject(new
                {
                    type = "mediaQueuePlayer.addItems",
                    queueItems = list
                }).ToString());
                if (!flag)
                    return;
                this.OnMultiScreenContentBroken();
            }*/
        }

        public void MediaPlay()
        {
            /*if (this.LoadState != LoadState.Loaded)
                return;
            this.appChannel.SendToHost(JsonConvert.SerializeObject(new
            {
                type = "mediaPanelPlayer.play"
            }).ToString());*/
        }

        public void MediaPause()
        {
            if (this.LoadState != LoadState.Loaded)
                return;
            this.appChannel.SendToHost(JsonConvert.SerializeObject(new
            {
                type = "mediaPanelPlayer.pause"
            }).ToString());
        }

        public void MediaStop()
        {
            if (this.LoadState != LoadState.Loaded)
                return;
            this.appChannel.SendToHost(JsonConvert.SerializeObject(new
            {
                type = "mediaPanelPlayer.stop"
            }).ToString());
        }

        public void QueuePlay()
        {
            if (this.LoadState != LoadState.Loaded)
                return;
            this.appChannel.SendToHost(JsonConvert.SerializeObject(new
            {
                type = "mediaQueuePlayer.play"
            }).ToString());
        }

        public void QueuePause()
        {
            if (this.LoadState != LoadState.Loaded)
                return;
            this.appChannel.SendToHost(JsonConvert.SerializeObject(new
            {
                type = "mediaQueuePlayer.pause"
            }).ToString());
        }

        public void QueueStop()
        {
            if (this.LoadState != LoadState.Loaded)
                return;
            this.appChannel.SendToHost(JsonConvert.SerializeObject(new
            {
                type = "mediaQueuePlayer.stop"
            }).ToString());
        }

        public void QueueNext()
        {
            if (this.LoadState != LoadState.Loaded)
                return;
            this.appChannel.SendToHost(JsonConvert.SerializeObject(new
            {
                type = "mediaQueuePlayer.next"
            }).ToString());
        }

        public void MoveQueueItem(string mediaId, int newIndex)
        {
            if (this.LoadState != LoadState.Loaded)
                return;
            this.appChannel.SendToHost(JsonConvert.SerializeObject(new
            {
                type = "mediaQueuePlayer.moveItem",
                queueItem = new
                {
                    id = mediaId
                },
                index = newIndex.ToString()
            }).ToString());
        }

        public void DeleteQueueItem(string mediaId)
        {
            if (this.LoadState != LoadState.Loaded)
                return;
            this.appChannel.SendToHost(JsonConvert.SerializeObject(new
            {
                type = "mediaQueuePlayer.removeItems",
                queueItems = new object[1]
              {
          (object) new
          {
            id = mediaId
          }
              }
            }).ToString());
        }

        public void UpdateMediaQueue()
        {
            if (this.LoadState != LoadState.Loaded)
                return;
            this.appChannel.SendToHost(JsonConvert.SerializeObject(new
            {
                type = "mediaQueuePlayer"
            }).ToString());
        }

        public void UpdateCurrentMedia()
        {
            if (this.LoadState != LoadState.Loaded)
                return;
            this.appChannel.SendToHost(JsonConvert.SerializeObject(new
            {
                type = "mediaPanel.currentMedia"
            }).ToString());
        }

        public void SetCurrentMediaTimePosition(double currentTime)
        {
            if (this.LoadState != LoadState.Loaded)
                return;
            this.appChannel.SendToHost(JsonConvert.SerializeObject(new
            {
                type = "mediaPanelPlayer.setCurrentTime",
                currentTime = currentTime
            }).ToString());
        }

        public void SetSlideShowSettings(SlideShowSettingsModel settings)
        {
            if (this.LoadState != LoadState.Loaded)
                return;
            string str1 = string.Empty;
            string str2;
            if (!string.IsNullOrEmpty(settings.Effect) && !string.IsNullOrEmpty(settings.Speed))
                str2 = JsonConvert.SerializeObject(new
                {
                    type = "mediaQueuePlayer.setConfig",
                    config = new
                    {
                        speed = settings.Speed,
                        effect = settings.Effect
                    }
                });
            else if (!string.IsNullOrEmpty(settings.Effect) && string.IsNullOrEmpty(settings.Speed))
            {
                str2 = JsonConvert.SerializeObject(new
                {
                    type = "mediaQueuePlayer.setConfig",
                    config = new
                    {
                        effect = settings.Effect
                    }
                });
            }
            else
            {
                if (string.IsNullOrEmpty(settings.Speed) || !string.IsNullOrEmpty(settings.Effect))
                    return;
                str2 = JsonConvert.SerializeObject(new
                {
                    type = "mediaQueuePlayer.setConfig",
                    config = new
                    {
                        speed = settings.Speed
                    }
                });
            }
            this.appChannel.SendToHost(str2.ToString());
        }

        private void TryStartMultiscreenApp()
        {
            Console.WriteLine("TryStartMultiscreenApp started...");
            //Console.WriteLine("TryStartMultiscreenApp httpServer IsWorking: {0}, LoadState: {1}", (object)(bool)(this.httpServer.IsWorking ? 1 : 0), (object)this.LoadState.ToString());
            if (!this.httpServer.IsWorking || this.LoadState != LoadState.Stoped && this.LoadState != LoadState.Failed)
                return;
            this.LoadState = LoadState.Loading;
            GetDialDeviceRequest dialDeviceRequest = new GetDialDeviceRequest(this.deviceUri, (DeviceAsyncResult<Device>)this.deviceResultByDevice);
            Console.WriteLine("TryStartMultiscreenApp getDevice run.");
            dialDeviceRequest.run();
        }

        private MediaContent CreateMediaContent(string serverAddress, Content file)
        {
            MediaContent mediaContent = new MediaContent()
            {
                Source = new Uri(serverAddress + "/ms/file/" + file.ID.ToString() + Path.GetExtension(file.Path)),
                Type = this.ConvertContentTypeToString(file.ContentType),
                Name = file.Name
            };
            if (file.ContentType == ContentType.Track)
            {
                Track track = file as Track;
                if (track != null && track.Artist != null && track.Album != null)
                {
                    mediaContent.AlbumName = track.Album.Name;
                    mediaContent.ArtistName = track.Artist.Name;
                }
            }
            if (file.Preview != (Uri)null)
                mediaContent.Thumbnail = new Uri(serverAddress + "/ms/cover/" + file.ID.ToString());
            return mediaContent;
        }

        private string ConvertContentTypeToString(ContentType type)
        {
            if (type == ContentType.Track)
                return "audio";
            if (type == ContentType.Video)
                return "video";
            return type == ContentType.Image ? "image" : "";
        }

        private void deviceResultByDevice_OnSuccess(object sender, Device device)
        {
            Console.WriteLine("deviceResultByDevice_OnSuccess started...");
            if (device == null)
                return;
            this.connectedDevice = device;
            Console.WriteLine("deviceResultByDevice_OnSuccess connectedDevice GetApplication : {0}", (object)"Mediashare");
            this.connectedDevice.GetApplication("Mediashare", (ApplicationAsyncResult<Application>)this.applicationResult);
        }

        private void deviceResultByDevice_OnFailed(object sender, DeviceError error)
        {
            Console.WriteLine("deviceResultByDevice_OnFailed with Code: {0}, message: {1}, ", (object)error.Code, (object)error.Message);
            this.LoadState = LoadState.Failed;
            this.OnMultiscreenStartFailed();
        }

        private void applicationResult_OnSuccess(object sender, Application application)
        {
            Console.WriteLine("applicationResult_OnSuccess started...");
            if (application == null)
                return;
            this.deviceApp = application;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("_headers", "{\"SilentLaunch\" : \"true\"}");
            Console.WriteLine("applicationResult_OnSuccess started...");
            this.deviceApp.Launch((IDictionary<string, string>)dictionary, (ApplicationAsyncResult<bool>)this.boolResult);
        }

        private void applicationResult_OnFailed(object sender, ApplicationError e)
        {
            Console.WriteLine("applicationResult_OnFailed with Code: {0}, message: {1}, ", (object)e.Code, (object)e.Message);
            this.LoadState = LoadState.Failed;
            this.OnMultiscreenStartFailed();
        }

        private async void boolResult_OnSuccess(object sender, bool e)
        {
            Console.WriteLine("boolResult_OnSuccess started...");
            Console.WriteLine("boolResult_OnSuccess connectedDevice ConnectToChannel by id: {0}", (object)"multiscreen_media_share");
            await Task.Delay(2000);
        }

        private void boolResult_OnFailed(object sender, ApplicationError e)
        {
            Console.WriteLine("boolResult_OnFailed with Code: {0}, message: {1}, ", (object)e.Code, (object)e.Message);
            this.LoadState = LoadState.Failed;
            this.OnMultiscreenStartFailed();
        }

        private void deviceResultByChannel_OnSuccess(object sender, com.samsung.multiscreen.channel.Channel channel)
        {   
        }

        private void deviceResultByChannel_OnFailed(object sender, DeviceError e)
        {   
        }

        private void channelListner_DisconnectReceived(object sender, EventArgs e)
        {   
        }

        private void channelListner_MessageReceived(object sender, string message)
        {
            string str = this.ParseJson<string>(message, "type");
            Console.WriteLine("channelListner_MessageReceived with message type: {0}, ", (object)str);
            if (str == null)
                return;
            Logger.Instance.LogMessage("Multiscreen message: " + str);
            switch (str)
            {
                case "mediaQueuePlayer.setConfig":
                    QueueConfig queueConfig = this.ParseJson<QueueConfig>(message, "config");
                    this.QueueSpeed = queueConfig.Speed;
                    this.QueueEffect = queueConfig.Effect;
                    break;
                case "mediaQueuePlayer.addItems":
                    this.UpdateMediaQueue();
                    break;
                case "mediaQueuePlayer.removeItems":
                    this.UpdateMediaQueue();
                    break;
                case "mediaQueuePlayer.moveItem":
                    this.UpdateMediaQueue();
                    break;
                case "mediaQueuePlayer":
                    MediaQueuePlayer mediaQueuePlayer1 = this.ParseJson<MediaQueuePlayer>(message, "mediaqueue");
                    if (mediaQueuePlayer1 == null || mediaQueuePlayer1.MediaQueue == null)
                        break;
                    IEnumerable<MediaContent> enumerable1 = Enumerable.Select<MediaQueueItem, MediaContent>((IEnumerable<MediaQueueItem>)mediaQueuePlayer1.MediaQueue, (Func<MediaQueueItem, MediaContent>)(m => m.Media));
                    List<Content> list1 = new List<Content>();
                    foreach (IMediaContent media in enumerable1)
                    {
                        Content content = this.ConvertMediaContent(media);
                        if (content != null)
                            list1.Add(content);
                    }
                    this.MediaQueue = (IEnumerable<Content>)list1;
                    this.OnMultiscreenQueueUpdated();
                    break;
                case "mediaQueuePlayer.play":
                    this.QueueState = MediaState.Play;
                    break;
                case "mediaQueuePlayer.pause":
                    this.QueueState = MediaState.Pause;
                    break;
                case "mediaQueuePlayer.stop":
                    this.QueueState = MediaState.Stop;
                    break;
                case "multiScreenMediaShare.info":
                    MediaQueuePlayer mediaQueuePlayer2 = this.ParseJson<MediaQueuePlayer>(message, "mediaQueuePlayer");
                    MediaPanel mediaPanel = this.ParseJson<MediaPanel>(message, "mediaPanel");
                    switch (mediaQueuePlayer2.Status)
                    {
                        case "playing":
                            this.QueueState = MediaState.Play;
                            break;
                        case "pause":
                            this.QueueState = MediaState.Pause;
                            break;
                        default:
                            this.QueueState = MediaState.Stop;
                            break;
                    }
                    if (mediaQueuePlayer2 != null && mediaQueuePlayer2.MediaQueue != null)
                    {
                        IEnumerable<MediaContent> enumerable2 = Enumerable.Select<MediaQueueItem, MediaContent>((IEnumerable<MediaQueueItem>)mediaQueuePlayer2.MediaQueue, (Func<MediaQueueItem, MediaContent>)(m => m.Media));
                        List<Content> list2 = new List<Content>();
                        foreach (IMediaContent media in enumerable2)
                        {
                            Content content = this.ConvertMediaContent(media);
                            if (content != null)
                                list2.Add(content);
                        }
                        this.MediaQueue = (IEnumerable<Content>)list2;
                        this.OnMultiscreenQueueUpdated();
                    }
                    if (mediaPanel == null)
                        break;
                    this.CurrentMediaContent = this.ConvertMediaContent((IMediaContent)mediaPanel.CurrentMedia);
                    this.UpdateCurrentMedia();
                    break;
                case "mediaPanelPlayer.play":
                    this.MediaState = MediaState.Play;
                    break;
                case "mediaPanelPlayer.pause":
                    this.MediaState = MediaState.Pause;
                    break;
                case "mediaPanelPlayer.stop":
                    this.MediaState = MediaState.Stop;
                    break;
                case "mediaPanelPlayer.ended":
                    if (this.MediaQueue == null || Enumerable.Count<Content>(this.MediaQueue) == 0)
                    {
                        this.OnMultiscreenQueueEnded();
                        this.CurrentMediaTimeInfo = (IMediaTimeInfo)null;
                        this.CurrentMediaContent = (Content)null;
                        break;
                    }
                    if (this.QueueState == MediaState.Pause)
                    {
                        this.QueuePlay();
                        break;
                    }
                    if (this.QueueState != MediaState.Stop)
                        break;
                    this.CurrentMediaTimeInfo = (IMediaTimeInfo)null;
                    this.CurrentMediaContent = (Content)null;
                    this.OnMultiscreenQueueEnded();
                    break;
                case "mediaPanel.setMedia":
                    this.UpdateCurrentMedia();
                    break;
                case "mediaPanel.currentMedia":
                    this.CurrentMediaContent = this.ConvertMediaContent((IMediaContent)this.ParseJson<MediaContent>(message, "media"));
                    if (this.CurrentMediaContent != null)
                        break;
                    this.CurrentMediaTimeInfo = (IMediaTimeInfo)null;
                    if (this.MediaQueue != null && Enumerable.Count<Content>(this.MediaQueue) != 0)
                        break;
                    this.OnMultiscreenQueueEnded();
                    break;
                case "mediaPanelPlayer.timeupdate":
                    MediaTimeInfo mediaTimeInfo = JsonConvert.DeserializeObject<MediaTimeInfo>(message);
                    if (this.CurrentMediaContent == null)
                    {
                        this.CurrentMediaTimeInfo = (IMediaTimeInfo)null;
                        break;
                    }
                    this.CurrentMediaTimeInfo = (IMediaTimeInfo)mediaTimeInfo;
                    break;
                case "currentMediaPlayer.error":
                    this.UpdateCurrentMedia();
                    break;
                case "mediaPanelPlayer.error":
                    Console.WriteLine("channelListner_MessageReceived MESSAGE_TYPE_PANEL_ERROR: {0}", (object)message);
                    if (this.ParseJson<string>(message, "errorType") == "not_supported")
                    {
                        this.CurrentMediaContent = (Content)null;
                        this.OnMultiScreenContentNotSupported();
                    }
                    else
                        this.OnMultiScreenContentFailed();
                    this.UpdateCurrentMedia();
                    break;
                case "mediaPanelPlayer.clearNP":
                    this.OnMultiscreenCurrentMediaContentUpdated();
                    break;
            }
        }

        private Content ConvertMediaContent(IMediaContent media)
        {
            if (media == null)
                return (Content)null;
            Guid guid1 = Guid.Empty;
            Guid guid2;
            try
            {
                int num = Enumerable.Count<string>((IEnumerable<string>)media.Thumbnail.Segments);
                guid2 = new Guid(media.Thumbnail.Segments[num - 1]);
            }
            catch
            {
                guid2 = Guid.NewGuid();
            }
            if (media.Type == "audio")
            {
                Track track = new Track(media.Name, "", guid2, Guid.Empty, new Artist(media.ArtistName, (Uri)null), new Album(media.AlbumName, (Uri)null), new Genre(string.Empty), (Uri)null, media.Id);
                track.Thumbnail = media.Thumbnail;
                return (Content)track;
            }
            if (media.Type == "video")
            {
                MultimediaFile multimediaFile = new MultimediaFile(media.Name, "", guid2, Guid.Empty, ContentType.Video, DateTime.Now, media.Id);
                multimediaFile.Thumbnail = media.Thumbnail;
                return (Content)multimediaFile;
            }
            if (!(media.Type == "image"))
                return (Content)null;
            MultimediaFile multimediaFile1 = new MultimediaFile(media.Name, "", guid2, Guid.Empty, ContentType.Image, DateTime.Now, media.Id);
            multimediaFile1.Thumbnail = media.Thumbnail;
            return (Content)multimediaFile1;
        }

        private T ParseJson<T>(string json, string property)
        {
            JToken jtoken = JObject.Parse(json).GetValue(property);
            if (jtoken == null)
                return default(T);
            return jtoken.ToObject<T>();
        }

        public void Dispose()
        {
            if (this.LoadState != LoadState.Loaded)
                return;
            this.Close();
        }

        protected void OnPropertyChanged(object sender, string propertyName)
        {
            PropertyChangedEventHandler changedEventHandler = this.PropertyChanged;
            if (changedEventHandler == null)
                return;
            changedEventHandler(sender, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnMultiscreenDisconnected()
        {
            EventHandler eventHandler = this.MultiscreenDisconnected;
            if (eventHandler == null)
                return;
            eventHandler(this, new EventArgs());
        }

        protected void OnMultiscreenQueueEnded()
        {
            EventHandler eventHandler = this.MultiscreenQueueEnded;
            if (eventHandler == null)
                return;
            eventHandler(this, EventArgs.Empty);
        }

        protected void OnMultiscreenQueueUpdated()
        {
            EventHandler eventHandler = this.MultiscreenQueueUpdated;
            if (eventHandler == null)
                return;
            eventHandler(this, EventArgs.Empty);
        }

        protected void OnMultiscreenStartFailed()
        {
            EventHandler eventHandler = this.MultiscreenStartFailed;
            if (eventHandler == null)
                return;
            eventHandler(this, EventArgs.Empty);
        }

        protected void OnPushToTvEnded()
        {
            EventHandler eventHandler = this.PushToTvEnded;
            if (eventHandler == null)
                return;
            eventHandler(this, new EventArgs());
        }

        protected void OnPushToTvQueueEnded(int addedFiles, int repeatedFiles)
        {
            EventHandler<MediaQueueEventArgs> eventHandler = this.PushToTvQueueEnded;
            if (eventHandler == null)
                return;
            eventHandler(this, new MediaQueueEventArgs(addedFiles, repeatedFiles));
        }

        protected void OnMultiscreenCurrentMediaContentUpdated()
        {
            EventHandler eventHandler = this.MultiscreenCurrentMediaContentUpdated;
            if (eventHandler == null)
                return;
            eventHandler(this, EventArgs.Empty);
        }

        protected void OnMultiScreenContentBroken()
        {
            EventHandler eventHandler = this.MultiScreenContentBroken;
            if (eventHandler == null)
                return;
            eventHandler(this, EventArgs.Empty);
        }

        protected void OnMultiScreenContentFailed()
        {
            EventHandler eventHandler = this.MultiScreenContentFailed;
            if (eventHandler == null)
                return;
            eventHandler(this, EventArgs.Empty);
        }

        protected void OnMultiScreenContentNotSupported()
        {
            EventHandler eventHandler = this.MultiScreenContentNotSupported;
            if (eventHandler == null)
                return;
            eventHandler(this, EventArgs.Empty);
        }
    }
}
