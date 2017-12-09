using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace s32.Sceh.DataModel
{
    public class SteamProfile : SteamProfileKey
    {
        public SteamProfile(long steamId, string customUrl)
            : base(steamId, customUrl)
        { }

        public DateTime LastUpdate { get; set; }

        public DateTime LastUse { get; set; }

        public string Name { get; set; }

        public Uri AvatarSmallUrl { get; set; }

        public Uri AvatarMediumUrl { get; set; }

        public Uri AvatarFullUrl { get; set; }

        public List<string> Note { get; set; }

        public bool HasNote
        {
            get { return Note != null && Note.Count > 0; }
        }
    }
}