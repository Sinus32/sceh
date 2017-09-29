using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace s32.Sceh.Data
{
    [Serializable]
    [XmlRoot("Image", Namespace = ScehData.XML_NAMESPACE)]
    public class ImageFile
    {
        [XmlElement]
        public string ImageUrl { get; set; }

        [XmlIgnore]
        public ImageDirectory Directory { get; set; }

        [XmlElement]
        public string Filename { get; set; }

        [XmlAttribute]
        public string ETag { get; set; }

        [XmlAttribute]
        public DateTime LastUpdate { get; set; }
    }
}
