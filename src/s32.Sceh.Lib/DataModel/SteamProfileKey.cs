using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace s32.Sceh.DataModel
{
    public class SteamProfileKey
    {
        public SteamProfileKey(long steamId, string customUrl)
        {
            SteamId = steamId;
            CustomUrl = customUrl;
        }

        public long SteamId { get; set; }

        public string CustomUrl { get; set; }

        public string CustomUrlOrSteamId
        {
            get
            {
                return String.IsNullOrEmpty(CustomUrl)
                    ? SteamId.ToString()
                    : CustomUrl;
            }
        }
    }
}