using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace s32.Sceh.DataStore
{
    [Serializable]
    [XmlRoot("SteamProfileKey", Namespace = ScehData.NS_SCEH)]
    public class SteamProfileKey
    {
        public SteamProfileKey()
        { }

        public SteamProfileKey(long steamId, string customUrl)
        {
            SteamId = steamId;
            CustomUrl = customUrl;
        }

        [XmlAttribute]
        public long SteamId { get; set; }

        [XmlElement(Order = 0)]
        public string CustomUrl { get; set; }

        [XmlIgnore]
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