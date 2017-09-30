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
        [XmlElement(Order = 4)]
        public ImageFile AvatarFull { get; set; }

        [XmlElement(Order = 3)]
        public ImageFile AvatarMedium { get; set; }

        [XmlElement(Order = 2)]
        public ImageFile AvatarSmall { get; set; }

        [XmlElement(Order = 1)]
        public string CustomURL { get; set; }

        [XmlIgnore]
        public string CustomURLOrSteamID
        {
            get
            {
                if (String.IsNullOrEmpty(CustomURL))
                    return SteamId.ToString();
                return CustomURL;
            }
        }

        [XmlAttribute]
        public DateTime LastUpdate { get; set; }

        [XmlElement(Order = 0)]
        public string Name { get; set; }

        [XmlAttribute]
        public long SteamId { get; set; }
    }
}