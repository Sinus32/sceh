using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using s32.Sceh.DataModel;
using s32.Sceh.SteamApi;

namespace s32.Sceh.Code
{
    public static class DataManager
    {
        private const string INVENTORY_CACHE_KEY = "SteamUserInventory_";
        private static ScehData _currentData;
        private static Dictionary<string, ImageFile> _imageUrlLookup;

        public static bool AutoLogIn
        {
            get
            {
                if (_currentData != null)
                    return _currentData.Profiles.AutoLogIn;
                return false;
            }
            set
            {
                if (_currentData != null)
                    _currentData.Profiles.AutoLogIn = value;
            }
        }

        public static ImageDirectory AvatarsDirectory
        {
            get { return _currentData != null ? _currentData.AvatarsDirectory : null; }
        }

        public static ImageDirectory CardsDirectory
        {
            get { return _currentData != null ? _currentData.CardsDirectory : null; }
        }

        public static SteamProfile LastSteamProfile
        {
            get
            {
                if (_currentData != null && _currentData.Profiles.LastSteamProfileId > 0L)
                    return _currentData.Profiles.SteamProfiles.FirstOrDefault(q => q.SteamId == _currentData.Profiles.LastSteamProfileId);
                return null;
            }
            set
            {
                if (value == null)
                    _currentData.Profiles.LastSteamProfileId = 0L;
                else
                    _currentData.Profiles.LastSteamProfileId = value.SteamId;
            }
        }

        public static SteamProfile AddOrUpdateSteamProfile(SteamProfile profile)
        {
            if (profile.SteamId <= 0L)
                throw new ArgumentException("Steam profile is invalid", "profile");

            lock (_currentData)
            {
                foreach (var dt in _currentData.Profiles.SteamProfiles)
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

                _currentData.Profiles.SteamProfiles.Add(profile);
                return profile;
            }
        }

        public static SteamProfile AddOrUpdateSteamProfile(SteamProfileResp resp)
        {
            if (resp == null)
                return null;

            var result = new SteamProfile(resp.SteamId, String.IsNullOrEmpty(resp.CustomURL) ? null : resp.CustomURL);
            result.Name = resp.Name;
            result.AvatarSmallUrl = new Uri(resp.AvatarIcon);
            result.AvatarMediumUrl = new Uri(resp.AvatarIconMedium);
            result.AvatarFullUrl = new Uri(resp.AvatarIconFull);
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

            var imageUrl = new Uri(String.Concat(CARD_IMAGE_SOURCE, steamCard.IconUrl));
            return GetOrCreateImageFile(imageUrl, directory, out isNew);
        }

        public static ImageFile GetOrCreateImageFile(Uri imageUrl, ImageDirectory directory, out bool isNew)
        {
            if (imageUrl == null)
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
                result = new ImageFile(directory);
                result.ImageUrl = imageUrl;
                directory.Images.Add(result);
                _imageUrlLookup[key] = result;
            }

            isNew = true;
            return result;
        }

        public static SteamProfile GetSteamProfile(SteamProfileKey profileKey)
        {
            if (profileKey == null)
                return null;

            if (profileKey.SteamId > 0L)
                return _currentData.Profiles.SteamProfiles.FirstOrDefault(q => q.SteamId == profileKey.SteamId);

            if (!String.IsNullOrEmpty(profileKey.CustomUrl))
                return _currentData.Profiles.SteamProfiles.FirstOrDefault(q => String.Equals(q.CustomUrl, profileKey.CustomUrl, StringComparison.OrdinalIgnoreCase));

            return null;
        }

        public static IReadOnlyList<SteamProfile> GetSteamProfiles()
        {
            if (_currentData == null)
                return new SteamProfile[0];

            return _currentData.Profiles.SteamProfiles;
        }

        public static UserInventory GetSteamUserInventory(SteamProfile profile, bool forceRefresh)
        {
            UserInventory result;
            var key = String.Concat(INVENTORY_CACHE_KEY, profile.SteamId);

            if (!forceRefresh)
            {
                result = MemoryCache.Default.Get(key) as UserInventory;
                if (result != null && result.IsInventoryAvailable && result.Cards != null)
                    return result;
            }

            string errorMessage;
            result = new UserInventory();
            result.SteamId = profile.SteamId;
            result.Cards = SteamDataDownloader.GetCardsB(profile, out errorMessage);
            result.ErrorMessage = errorMessage;
            result.IsInventoryAvailable = errorMessage == null && result.Cards != null;

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
            _currentData.Profiles = new ProfilesData();
            _currentData.AvatarsDirectory = new ImageDirectory("Avatars");
            _currentData.CardsDirectory = new ImageDirectory("Cards");

            _currentData.Paths = new PathConfig();

            var dataLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var localDataLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            _currentData.Paths.AppDataPath = Path.Combine(dataLocation, "Sceh");
            _currentData.Paths.LocalAppDataPath = Path.Combine(localDataLocation, "Sceh");
            _currentData.Paths.BackupsPath = Path.Combine(_currentData.Paths.AppDataPath, "Backups");
            _currentData.Paths.AvatarsDirectoryPath = Path.Combine(_currentData.Paths.LocalAppDataPath, _currentData.AvatarsDirectory.RelativePath);
            _currentData.Paths.CardsDirectoryPath = Path.Combine(_currentData.Paths.LocalAppDataPath, _currentData.CardsDirectory.RelativePath);

            Directory.CreateDirectory(_currentData.Paths.AppDataPath);
            Directory.CreateDirectory(_currentData.Paths.LocalAppDataPath);
            Directory.CreateDirectory(_currentData.Paths.AvatarsDirectoryPath);
            Directory.CreateDirectory(_currentData.Paths.CardsDirectoryPath);

            var dataSerializer = new DataSerializer();
            dataSerializer.LoadFiles(_currentData);

            _imageUrlLookup = new Dictionary<string, ImageFile>();
            foreach (var dir in _currentData.ImageDirectories)
            {
                foreach (var img in dir.Images)
                {
                    img.Directory = dir;
                    _imageUrlLookup[LookupKey(dir, img)] = img;
                }
            }
        }

        public static string LocalFilePath(ImageFile image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (image.Directory == null)
                throw new ArgumentException("Image directory is null", "image");

            if (String.IsNullOrEmpty(image.Filename) || image.Filename.Length < 2)
                return null;

            return Path.Combine(_currentData.Paths.LocalAppDataPath, image.Directory.RelativePath, image.Filename.Remove(2), image.Filename);
        }

        public static void SaveFile()
        {
            if (_currentData == null || _currentData.Paths == null)
                return;

            lock (_currentData)
            {
                var dataSerializer = new DataSerializer();
                dataSerializer.SaveFiles(_currentData);
            }
        }

        private static string LookupKey(ImageDirectory dir, ImageFile img)
        {
            return LookupKey(dir, img.ImageUrl);
        }

        private static string LookupKey(ImageDirectory dir, Uri imageUrl)
        {
            return String.Concat(dir.RelativePath, '/', imageUrl.ToString());
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