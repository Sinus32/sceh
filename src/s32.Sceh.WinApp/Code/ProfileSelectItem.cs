using s32.Sceh.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.WinApp.Code
{
    public class ProfileSelectItem
    {
        public ProfileSelectItem()
        { }

        public ProfileSelectItem(ProfileSelectItem item)
        {
            this.SteamId = item.SteamId;
            this.Name = item.Name;
            this.CustomURL = item.CustomURL;
        }

        public ProfileSelectItem(SteamProfile steamProfile)
        {
            this.SteamId = steamProfile.SteamId;
            this.Name = steamProfile.Name;
            this.CustomURL = steamProfile.CustomURL;
        }

        public string CustomURL { get; set; }
        public string Name { get; set; }
        public long SteamId { get; set; }

        public override string ToString()
        {
            var text = String.IsNullOrEmpty(CustomURL)
                ? SteamId.ToString()
                : CustomURL;

            if (String.IsNullOrEmpty(Name))
                return text;

            return String.Format("{0} ({1})", Name, text);
        }
    }
}