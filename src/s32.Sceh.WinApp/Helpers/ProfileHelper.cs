﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.Classes;
using s32.Sceh.Code;
using s32.Sceh.DataModel;
using s32.Sceh.WinApp.Translations;

namespace s32.Sceh.WinApp.Helpers
{
    public class ProfileHelper
    {
        private const int MINUTES_DELAY = 60;

        public static SteamProfile GetSteamUser(SteamProfile steamProfile, string profileIdOrUrl, out string errorMessage)
        {
            SteamProfileKey profileKey = null;
            if (steamProfile != null)
            {
                profileKey = steamProfile;
            }
            else if (profileIdOrUrl != null)
            {
                profileKey = SteamDataDownloader.GetProfileKey(profileIdOrUrl);
            }

            if (profileKey == null)
            {
                errorMessage = Strings.InvalidProfileIdOrUrl;
                return null;
            }

            var cached = DataManager.GetSteamProfile(profileKey);
            if (cached != null)
            {
                var reqTime = DateTime.UtcNow.AddMinutes(-MINUTES_DELAY);
                if (cached.LastUpdate > reqTime)
                {
                    errorMessage = null;
                    return cached;
                }
            }

            Uri profileUri = null;
            if (cached != null)
            {
                profileUri = SteamDataDownloader.GetProfileUri(cached, SteamUrlPattern.ApiGetProfile);
            }
            else if (profileIdOrUrl != null)
            {
                profileUri = SteamDataDownloader.GetProfileUri(profileIdOrUrl, SteamUrlPattern.ApiGetProfile);
            }

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

        public static List<SteamProfile> LoadProfiles(long? firstProfileId)
        {
            var profiles = DataManager.GetSteamProfiles();
            var list = new List<SteamProfile>(profiles.Count);
            list.AddRange(profiles);
            list.Sort(new SteamProfileComparer(firstProfileId));
            return list;
        }

        private class SteamProfileComparer : IComparer<SteamProfile>
        {
            private long? _firstProfileId;

            public SteamProfileComparer(long? firstProfileId)
            {
                _firstProfileId = firstProfileId;
            }

            public int Compare(SteamProfile x, SteamProfile y)
            {
                if (x.SteamId == _firstProfileId)
                    return -1;

                if (y.SteamId == _firstProfileId)
                    return 1;

                var ret = DateTime.Compare(y.LastUse, x.LastUse);
                return ret != 0 ? ret : String.Compare(x.Name, y.Name);
            }
        }
    }
}
