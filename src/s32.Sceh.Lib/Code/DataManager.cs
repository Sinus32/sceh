using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using s32.Sceh.DataModel;
using s32.Sceh.DataStore;
using s32.Sceh.SteamApi;

namespace s32.Sceh.Code
{
    public static class DataManager
    {
        private const string INVENTORY_CACHE_KEY = "SteamUserInventory_";
        private static ScehData _currentData;
        private static Dictionary<string, ImageFile> _imageUrlLookup;

        public static ImageDirectory AvatarsDirectory
        {
            get { return _currentData != null ? _currentData.AvatarsDirectory : null; }
        }

        public static ImageDirectory CardsDirectory
        {
            get { return _currentData != null ? _currentData.CardsDirectory : null; }
        }

        public static SteamProfile AddOrUpdateSteamProfile(SteamProfile profile)
        {
            if (profile.SteamId <= 0L)
                throw new ArgumentException("Steam profile is invalid", "profile");

            lock (_currentData)
            {
                foreach (var dt in _currentData.DataFile.SteamProfiles)
                {
                    if (dt.SteamId == profile.SteamId)
                    {
                        dt.Name = profile.Name;
                        dt.CustomUrl = profile.CustomUrl;
                        dt.AvatarSmallUrl = profile.AvatarSmallUrl;
                        dt.AvatarMediumUrl = profile.AvatarMediumUrl;
                        dt.AvatarFullUrl = profile.AvatarFullUrl;
                        dt.LastUpdate = profile.LastUpdate;
                        return dt;
                    }
                }

                _currentData.DataFile.SteamProfiles.Add(profile);
                return profile;
            }
        }

        public static SteamProfile AddOrUpdateSteamProfile(SteamProfileResp resp)
        {
            if (resp == null)
                return null;

            var result = new SteamProfile();
            result.SteamId = resp.SteamId;
            result.Name = resp.Name;
            result.CustomUrl = resp.CustomURL;
            result.AvatarSmallUrl = resp.AvatarIcon;
            result.AvatarMediumUrl = resp.AvatarIconMedium;
            result.AvatarFullUrl = resp.AvatarIconFull;
            result.LastUpdate = DateTime.UtcNow;

            return AddOrUpdateSteamProfile(result);
        }

        public static ImageFile GetOrCreateImageFile(Card steamCard, ImageDirectory directory, out bool isNew)
        {
            const string CARD_IMAGE_SOURCE = "http://steamcommunity-a.akamaihd.net/economy/image/";

            if (steamCard == null || String.IsNullOrEmpty(steamCard.IconUrl))
            {
                isNew = false;
                return null;
            }

            return GetOrCreateImageFile(String.Concat(CARD_IMAGE_SOURCE, steamCard.IconUrl), directory, out isNew);
        }

        public static ImageFile GetOrCreateImageFile(string imageUrl, ImageDirectory directory, out bool isNew)
        {
            if (String.IsNullOrEmpty(imageUrl))
            {
                isNew = false;
                return null;
            }

            ImageFile result;
            var key = LookupKey(directory, imageUrl);
            if (_imageUrlLookup.TryGetValue(key, out result))
            {
                isNew = false;
                return result;
            }

            lock (_currentData)
            {
                result = new ImageFile();
                result.ImageUrl = imageUrl;
                result.Directory = directory;
                directory.Images.Add(result);
                _imageUrlLookup[key] = result;
            }

            isNew = true;
            return result;
        }

        public static IReadOnlyList<SteamProfile> GetSteamProfiles()
        {
            if (_currentData == null)
                return new SteamProfile[0];

            return _currentData.DataFile.SteamProfiles;
        }

        public static List<Card> GetSteamUserInventory(SteamProfile profile, bool forceRefresh, out string errorMessage)
        {
            List<Card> result;
            var key = String.Concat(INVENTORY_CACHE_KEY, profile.SteamId);

            if (!forceRefresh)
            {
                result = MemoryCache.Default.Get(key) as List<Card>;
                if (result != null)
                {
                    errorMessage = null;
                    return result;
                }
            }

            var apiUrl = SteamDataDownloader.GetProfileUri(profile, ProfilePage.API_GET_INVENTORY);
            if (apiUrl == null)
            {
                errorMessage = "Invalid profile data";
                return null;
            }
            result = SteamDataDownloader.GetCards(apiUrl, out errorMessage);
            if (errorMessage != null)
                return null;

            var policy = new CacheItemPolicy();
            policy.SlidingExpiration = new TimeSpan(0, 10, 0);
            MemoryCache.Default.Set(key, result, policy);

            return result;
        }

        public static void Initialize()
        {
            if (_currentData != null)
                throw new InvalidOperationException("Data manager is already initialized");

            _currentData = new ScehData();
            _imageUrlLookup = new Dictionary<string, ImageFile>();

            var dataLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _currentData.AppDataPath = Path.Combine(dataLocation, "Sceh");

            Directory.CreateDirectory(_currentData.AppDataPath);

            _currentData.DataFilePath = Path.Combine(_currentData.AppDataPath, "scehdata.xml");
            if (File.Exists(_currentData.DataFilePath))
            {
                try
                {
                    var ser = new XmlSerializer(typeof(ScehDataFile));
                    using (var stream = File.OpenRead(_currentData.DataFilePath))
                    using (var reader = new StreamReader(stream, true))
                        _currentData.DataFile = (ScehDataFile)ser.Deserialize(stream);
                }
                catch (Exception)
                {
                    _currentData.DataFile = new ScehDataFile();
                }
            }
            else
            {
                _currentData.DataFile = new ScehDataFile();
            }

            if (_currentData.DataFile.ImageDirectories == null)
            {
                _currentData.DataFile.ImageDirectories = new List<ImageDirectory>();
            }
            else
            {
                foreach (var dir in _currentData.DataFile.ImageDirectories)
                {
                    foreach (var img in dir.Images)
                    {
                        img.Directory = dir;
                        _imageUrlLookup[LookupKey(dir, img)] = img;
                    }
                }
            }

            _currentData.AvatarsDirectory = MatchImageDirectory(_currentData.DataFile.ImageDirectories, "Avatars");
            _currentData.CardsDirectory = MatchImageDirectory(_currentData.DataFile.ImageDirectories, "Cards");

            _currentData.AvatarsDirectoryPath = Path.Combine(_currentData.AppDataPath, _currentData.AvatarsDirectory.RelativePath);
            _currentData.CardsDirectoryPath = Path.Combine(_currentData.AppDataPath, _currentData.CardsDirectory.RelativePath);

            Directory.CreateDirectory(_currentData.AvatarsDirectoryPath);
            Directory.CreateDirectory(_currentData.CardsDirectoryPath);

            if (_currentData.DataFile.SteamProfiles == null)
                _currentData.DataFile.SteamProfiles = new List<SteamProfile>();
        }

        public static string LocalFilePath(ImageFile image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (image.Directory == null)
                throw new ArgumentException("Image directory is null", "image");

            if (String.IsNullOrEmpty(image.Filename) || image.Filename.Length < 2)
                return null;

            return Path.Combine(_currentData.AppDataPath, image.Directory.RelativePath, image.Filename.Remove(2), image.Filename);
        }

        public static void SaveFile()
        {
            if (_currentData.DataFile == null || _currentData.DataFilePath == null)
                return;

            var ser = new XmlSerializer(typeof(ScehDataFile));
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, ScehData.NS_SCEH);

            lock (_currentData)
            {
                using (var stream = File.OpenWrite(_currentData.DataFilePath))
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                using (var xml = XmlWriter.Create(writer, settings))
                    ser.Serialize(xml, _currentData.DataFile, namespaces);
            }
        }

        private static string LookupKey(ImageDirectory dir, ImageFile img)
        {
            return String.Concat(dir.RelativePath, '/', img.ImageUrl);
        }

        private static string LookupKey(ImageDirectory dir, string imageUrl)
        {
            return String.Concat(dir.RelativePath, '/', imageUrl);
        }

        private static ImageDirectory MatchImageDirectory(List<ImageDirectory> directories, string relativePath)
        {
            var result = directories.FirstOrDefault(q => q.RelativePath == relativePath);
            if (result == null)
            {
                result = new ImageDirectory(relativePath);
                directories.Add(result);
            }
            return result;
        }
    }
}