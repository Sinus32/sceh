using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace s32.Sceh.Data
{
    [Serializable]
    [XmlRoot("ScehData", Namespace = ScehData.XML_NAMESPACE)]
    public class ScehDataFile
    {
        [XmlElement(Order = 0)]
        public long LastSteamProfileID { get; set; }

        [XmlArray(Order=1)]
        [XmlArrayItem]
        public List<SteamProfile> SteamProfiles { get; set; }

        [XmlElement("ImageDirectory", Order=2)]
        public List<ImageDirectory> ImageDirectories { get; set; }
    }
}
