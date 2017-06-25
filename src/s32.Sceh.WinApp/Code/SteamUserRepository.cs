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

        public bool TryGetUser(string idOrUrl, out SteamUser result, out string errorMessage)
        {
            if (_dict.TryGetValue(idOrUrl, out result))
            {
                errorMessage = null;
                return true;
            }

            return TryLoadUser(idOrUrl, out result, out errorMessage);
        }

        private bool TryLoadUser(string idOrUrl, out SteamUser result, out string errorMessage)
        {
            var profile = SteamDataDownloader.GetProfile(idOrUrl, out errorMessage);

            if (errorMessage != null)
            {
                result = null;
                return false;
            }

            result = new SteamUser();
            result.Profile = profile;

            if (!String.IsNullOrEmpty(profile.CustomURL))
                _dict[profile.CustomURL] = result;

            var idAsString = profile.SteamId.ToString();
            _dict[idAsString] = result;

            errorMessage = null;
            return true;
        }
    }
}
