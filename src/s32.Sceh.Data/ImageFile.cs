using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace s32.Sceh.Data
{
    [Serializable]
    [XmlRoot("Image", Namespace = ScehData.NS_SCEH)]
    public class ImageFile
    {
        [XmlAttribute]
        public DateTime LastUpdate { get; set; }

        [XmlAttribute]
        public string ETag { get; set; }

        [XmlElement(Order = 0)]
        public string Filename { get; set; }

        [XmlElement(Order = 1)]
        public string ImageUrl { get; set; }

        [XmlIgnore]
        public ImageDirectory Directory { get; set; }
    }
}
