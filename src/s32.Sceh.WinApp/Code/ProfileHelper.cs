using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.Classes;
using s32.Sceh.Code;
using s32.Sceh.DataStore;
using s32.Sceh.WinApp.Translations;

namespace s32.Sceh.WinApp.Code
{
    public class ProfileHelper
    {
        public static SteamProfile GetSteamUser(SteamProfile steamProfile, string profileIdOrUrl, out string errorMessage)
        {
            Uri profileUri = null;
            if (steamProfile != null)
                profileUri = SteamDataDownloader.GetProfileUri(steamProfile, ProfilePage.API_GET_PROFILE);
            else if (profileIdOrUrl != null)
                profileUri = SteamDataDownloader.GetProfileUri(profileIdOrUrl, ProfilePage.API_GET_PROFILE);

            if (profileUri == null)
            {
                errorMessage = Strings.InvalidProfileIdOrUrl;
                return null;
            }

            try
            {
                SteamDataDownloader.GetProfileError error;
                var resp = SteamDataDownloader.GetProfile(profileUri, out error);
                switch (error)
                {
                    case SteamDataDownloader.GetProfileError.Success:
                        SteamProfile profile = DataManager.AddOrUpdateSteamProfile(resp);
                        errorMessage = null;
                        return profile;

                    case SteamDataDownloader.GetProfileError.WrongProfile:
                        errorMessage = Strings.WrongProfileIdOrUrl;
                        return null;

                    case SteamDataDownloader.GetProfileError.DeserializationError:
                        errorMessage = Strings.ProfileDeserializationError;
                        return null;

                    default:
                        errorMessage = Strings.UnknownErrorOccured;
                        return null;
                }
            }
            catch (Exception ex)
            {
                errorMessage = String.Format(Strings.ExceptionDuringDownloadingSteamProfile, ex.Message);
                return null;
            }
        }

        public static List<SteamProfile> LoadProfiles()
        {
            var profiles = DataManager.GetSteamProfiles();
            var list = new List<SteamProfile>(profiles.Count);
            list.AddRange(profiles);
            list.Sort((a, b) => String.Compare(a.Name, b.Name));
            return list;
        }
    }
}