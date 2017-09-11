using s32.Sceh.Classes;
using s32.Sceh.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.WinApp.Code
{
    public class SteamUserRepository
    {
        public static readonly SteamUserRepository Instance = new SteamUserRepository();
        private Dictionary<string, SteamUser> _dict;

        private SteamUserRepository()
        {
            _dict = new Dictionary<string, SteamUser>();
        }

        public bool TryGetUser(long steamId, string customURL, out SteamUser result, out string errorMessage)
        {
            if (steamId > 0L && _dict.TryGetValue(steamId.ToString(), out result))
            {
                errorMessage = null;
                return true;
            }

            if (!String.IsNullOrEmpty(customURL) && _dict.TryGetValue(customURL, out result))
            {
                errorMessage = null;
                return true;
            }

            var profileUri = SteamDataDownloader.GetProfileUri(steamId, customURL, ProfilePage.API_GET_PROFILE);
            if (profileUri == null)
            {
                errorMessage = "Invalid profile";
                result = null;
                return false;
            }

            var profile = SteamDataDownloader.GetProfile(profileUri, out errorMessage);

            if (errorMessage != null)
            {
                result = null;
                return false;
            }

            result = RegisterSteamProfile(profile);
            errorMessage = null;
            return true;
        }

        public bool TryGetUser(string idOrUrl, out SteamUser result, out string errorMessage)
        {
            if (_dict.TryGetValue(idOrUrl, out result))
            {
                errorMessage = null;
                return true;
            }

            var profileUri = SteamDataDownloader.GetProfileUri(idOrUrl, ProfilePage.API_GET_PROFILE);
            if (profileUri == null)
            {
                errorMessage = "Invalid profile";
                return false;
            }

            var profile = SteamDataDownloader.GetProfile(profileUri, out errorMessage);

            if (errorMessage != null)
            {
                result = null;
                return false;
            }

            result = RegisterSteamProfile(profile);
            errorMessage = null;
            return true;
        }

        private SteamUser RegisterSteamProfile(SteamProfile steamProfile)
        {
            var result = new SteamUser();
            result.Profile = steamProfile;

            if (!String.IsNullOrEmpty(steamProfile.CustomURL))
                _dict[steamProfile.CustomURL] = result;

            var idAsString = steamProfile.SteamId.ToString();
            _dict[idAsString] = result;

            return result;
        }
    }
}