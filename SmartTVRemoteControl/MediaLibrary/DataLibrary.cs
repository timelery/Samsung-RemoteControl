using MediaLibrary.DataModels;
using SmartView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TagLib;

namespace SmartView2.Native.MediaLibrary
{
    public class DataLibrary : IDataLibrary
    {
        public readonly static string[] imageSearchPattern;

        public readonly static string[] musicSearchPattern;

        public readonly static string[] videoSearchPattern;

        private bool isDataLoaded;

        private CancellationTokenSource cancelToken;

        private Task addingTask;

        private string localDataDir;

        private Uri defaultTrackIcon = new Uri("http://www.google.com");

        private Queue<Tuple<string[], ContentType, bool>> contentAddingQueue;

        private FileSystemWatcher watcher;

        private IBaseDispatcher dispatcher;

        public Guid ID
        {
            get;
            set;
        }

        public bool IsDataLoaded
        {
            get
            {
                return true;
            }
            set
            {
                JustDecompileGenerated_set_IsDataLoaded(true);
            }
        }

        public bool JustDecompileGenerated_get_IsDataLoaded()
        {
            return this.isDataLoaded;
        }

        private void JustDecompileGenerated_set_IsDataLoaded(bool value)
        {
            this.isDataLoaded = value;
            this.OnDataLoaded();
        }

        public MultimediaFolder RootFolder
        {
            get
            {
                return JustDecompileGenerated_get_RootFolder();
            }
            set
            {
                JustDecompileGenerated_set_RootFolder(value);
            }
        }

        private MultimediaFolder JustDecompileGenerated_RootFolder_k__BackingField;

        public MultimediaFolder JustDecompileGenerated_get_RootFolder()
        {
            return null;
        }

        private void JustDecompileGenerated_set_RootFolder(MultimediaFolder value)
        {
        }

        public MultimediaFolder RootImageFolder
        {
            get
            {
                return null;
            }
            set
            {
                JustDecompileGenerated_set_RootImageFolder(value);
            }
        }

        private MultimediaFolder JustDecompileGenerated_RootImageFolder_k__BackingField;

        public MultimediaFolder JustDecompileGenerated_get_RootImageFolder()
        {
            return null;
        }

        private void JustDecompileGenerated_set_RootImageFolder(MultimediaFolder value)
        {
        }

        public MultimediaFolder RootMusicFolder
        {
            get
            {
                return null;
            }
            set
            {
                JustDecompileGenerated_set_RootMusicFolder(value);
            }
        }

        private MultimediaFolder JustDecompileGenerated_RootMusicFolder_k__BackingField;

        public MultimediaFolder JustDecompileGenerated_get_RootMusicFolder()
        {
            return null;
        }

        private void JustDecompileGenerated_set_RootMusicFolder(MultimediaFolder value)
        {
        }

        public MultimediaFolder RootVideoFolder
        {
            get
            {
                return null;
            }
            set
            {
                JustDecompileGenerated_set_RootVideoFolder(value);
            }
        }

        private MultimediaFolder JustDecompileGenerated_RootVideoFolder_k__BackingField;

        public MultimediaFolder JustDecompileGenerated_get_RootVideoFolder()
        {
            return null;
        }

        private void JustDecompileGenerated_set_RootVideoFolder(MultimediaFolder value)
        {
        }

        public List<Track> TrackList
        {
            get
            {
                return null;
            }
            set
            {
                JustDecompileGenerated_set_TrackList(value);
            }
        }

        private List<Track> JustDecompileGenerated_TrackList_k__BackingField;

        public List<Track> JustDecompileGenerated_get_TrackList()
        {
            return null;
        }

        private void JustDecompileGenerated_set_TrackList(List<Track> value)
        {
        }

        static DataLibrary()
        {
            /*string[] strArrays = new string[] { "*.jpeg", "*.jpg", "*.png", "*.bmp", "*.tif", "*.tiff", "*.gif" };
            DataLibrary.imageSearchPattern = strArrays;
            string[] strArrays1 = new string[] { "*.mp3", "*.flac", "*.mid", "*.midi" };
            DataLibrary.musicSearchPattern = strArrays1;
            string[] strArrays2 = new string[] { "*.mp4", "*.avi", "*.wmv", "*.mpeg" };
            DataLibrary.videoSearchPattern = strArrays2;*/
        }

        public DataLibrary(IBaseDispatcher dispatcher)
        {
            /*this.dispatcher = dispatcher;
            this.localDataDir = null;
            this.RootMusicFolder = null;
            this.RootImageFolder = null;
            this.RootVideoFolder = null;
            this.RootFolder = null;
            this.TrackList = null;
            this.contentAddingQueue = null;
            this.watcher = new FileSystemWatcher()
            {
                Path = this.localDataDir,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.LastAccess,
                Filter = ""
            };
            this.watcher.Changed += new FileSystemEventHandler(this.OnCacheDataFolderChanged);
            this.watcher.Deleted += new FileSystemEventHandler(this.OnCacheDataFolderChanged);
            this.watcher.EnableRaisingEvents = true;*/
        }

        public async Task AddContentFromFiles(string[] files, ContentType contentType)
        {
        }

        public async Task AddContentFromFolder(string pathtofolder, ContentType contentType)
        {

        }

        private async Task AddContentFromQueue()
        {

        }

        private bool AddFile(MultimediaFolder targetfolder, string filepath, ContentType type)
        {
            return true;
        }

        private int AddFilesToRootFolder(string[] filepaths, ContentType type, CancellationToken cancellationToken)
        {
            return 0;
        }

        private async Task AddMusics(MultimediaFolder folder, string[] filesPath)
        {
        }

        private int AddMusicsToRootFolder(string[] filesPath, CancellationToken cancellationToken)
        {
            return 0;
        }

        private bool AddTrack(MultimediaFolder rootfolder, string musictrack)
        {
            return true;
        }

        public void CancelAdding()
        {
        }

        private Album CheckAlbum(string albumname)
        {
            return null;
        }

        private Artist CheckArtist(string artistname)
        {
            return null;
        }

        private bool CheckFile(List<Content> files, string newfile)
        {
            return false;
        }

        private MultimediaFolder CheckFolder(MultimediaFolder parentfolder, string name)
        {
            return null;
        }

        private bool CheckFolderByName(MultimediaFolder item, string name)
        {
            return false;
        }

        private Genre CheckGenre(string genrename, Uri albumPreview)
        {
            return null;
        }

        private bool CheckMusicFile(string filepath)
        {
            return false;
        }

        private bool CheckTrack(MultimediaFolder folder, string pathtotrack)
        {
            return false;
        }

        private static BitmapFrame CreateResizedImage(ImageSource source, int width, int height, int margin)
        {
            return null;
        }

        public void DeleteFileOrFolder(MultimediaFolder rootfolder, ItemBase itemtodelete)
        {
        }

        public void DeleteItems(MultimediaFolder rootfolder, List<Content> itemstodelete)
        {
        }

        private static IEnumerable<string> FindAccessableFiles(string path, string file_pattern, bool recurse)
        {
            return null;
        }

        public static string[] FindFiles(string dir, string[] searchPatterns, SearchOption searchOption = SearchOption.AllDirectories)
        {
            return null;
        }

        private Uri GetAlbumPreview(string filepath, Guid fileGuid, IPicture[] pictures, bool onlytag = false)
        {
            return null;
        }

        public List<ItemBase> GetAlbums()
        {
            return null;
        }

        public List<Content> GetAlbumTracks(Guid id)
        {
            return null;
        }

        public List<ItemBase> GetArtists()
        {
            return null;
        }

        public List<Content> GetArtistsTracks(Guid id)
        {
            return null;
        }

        private TimeSpan GetDuration(string filepath)
        {
            TimeSpan zero;
            zero = TimeSpan.Zero;
            return zero;
        }

        private string[] GetFilesFromArray(string[] files, string[] searchpatterns)
        {
            List<string> strs = new List<string>();
            return strs.ToArray();
        }

        public void GetFolderById(ref MultimediaFolder searchresult, Guid id, MultimediaFolder folderwheretosearch)
        {
        }

        public List<ItemBase> GetGenres()
        {
            return null;
        }

        public List<Content> GetGenreTracks(string genre)
        {
            return null;
        }

        public ItemBase GetItemById(Guid id, MultimediaFolder root)
        {
            return null;
        }

        private Uri GetPreview(string file, ContentType type, Guid guid)
        {
            return null;
        }

        private MultimediaFolder GetTargetFolder(MultimediaFolder rootfolder, string file)
        {
            return null;
        }

        private byte[] ImageToByteArray(BitmapImage image)
        {
            return null;
        }

        public async Task LoadLibrary()
        {
            this.IsDataLoaded = true;
        }

        private TagLib.File MidiResolver(TagLib.File.IFileAbstraction abstraction, string mimetype, ReadStyle style)
        {
            return null;
        }

        private void OnCacheDataFolderChanged(object sender, FileSystemEventArgs e)
        {
            this.dispatcher.Invoke(() => {
                this.IsDataLoaded = true;
            });
        }

        private void OnContentAlreadyExist()
        {
            EventHandler eventHandler = this.ContentAlreadyExist;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        private void OnDataLoaded()
        {
            EventHandler eventHandler = this.DataLoaded;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        private void OnDataUpdated()
        {
            EventHandler eventHandler = this.DataUpdated;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        private void ReloadAlbumsTracksCount()
        {
        }

        private async Task RemoveDuplicatedObjects()
        {
        }

        private bool RemoveItemFromFolderById(MultimediaFolder folder, Guid guid)
        {
            return true;
        }

        public async Task SaveLibrary()
        {
        }

        private static List<string> SearchFiles(string folder, string pattern)
        {
            List<string> strs = new List<string>();
            return strs;
        }

        private void StartPreviewLoad()
        {
        }

        public event EventHandler ContentAlreadyExist;

        public event EventHandler DataLoaded;

        public event EventHandler DataUpdated;
    }
}