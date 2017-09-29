using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace s32.Sceh.Data
{
    [Serializable]
    [XmlRoot("SteamProfile", Namespace = ScehData.XML_NAMESPACE)]
    public class SteamProfile
    {
        [XmlAttribute]
        public long SteamID { get; set; }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string CustomURL { get; set; }

        [XmlElement]
        public ImageFile AvatarSmall { get; set; }

        [XmlElement]
        public ImageFile AvatarMedium { get; set; }

        [XmlElement]
        public ImageFile AvatarFull { get; set; }

        [XmlAttribute]
        public DateTime LastUpdate { get; set; }

        [XmlIgnore]
        public string CustomURLOrSteamID
        {
            get
            {
                if (String.IsNullOrEmpty(CustomURL))
                    return SteamID.ToString();
                return CustomURL;
            }
        }
    }
}
