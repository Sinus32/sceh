using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.Classes;
using s32.Sceh.Data;

namespace s32.Sceh.Code
{
    public static class DataManager
    {
        public static ImageFile GetImageByUrl(string imageUrl, ImageDirectory directory)
        {
            if (String.IsNullOrEmpty(imageUrl))
                return null;

            bool isNew;
            ImageFile result = directory.GetOrCreateImageByUrl(imageUrl, out isNew);

            if (result == null)
                return null;

            var priority = isNew || ScehData.LocalFileExists(result)
                ? DownloadPriority.High : DownloadPriority.Low;
            ImageDownloader.Download(result, priority);

            return result;
        }

        public static SteamProfile ReadAndStoreProfile(SteamProfileResp resp)
        {
            if (resp == null)
                return null;

            var result = new SteamProfile();
            result.SteamId = resp.SteamId;
            result.Name = resp.Name;
            result.CustomURL = resp.CustomURL;
            result.AvatarSmall = GetImageByUrl(resp.AvatarIcon, ScehData.AvatarsDirectory);
            result.AvatarMedium = GetImageByUrl(resp.AvatarIconMedium, ScehData.AvatarsDirectory);
            result.AvatarFull = GetImageByUrl(resp.AvatarIconFull, ScehData.AvatarsDirectory);
            result.LastUpdate = DateTime.UtcNow;

            return ScehData.AddOrUpdateProfile(result);
        }
    }
}