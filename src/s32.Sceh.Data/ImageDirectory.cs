using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace s32.Sceh.Data
{
    [Serializable]
    [XmlRoot("Directory", Namespace = ScehData.XML_NAMESPACE)]
    public class ImageDirectory
    {
        public ImageDirectory()
        {
            Images = new List<ImageFile>();
        }

        public ImageDirectory(string relativePath)
            : this()
        {
            RelativePath = relativePath;
        }

        [XmlAttribute]
        public string RelativePath { get; set; }

        [XmlElement("Image")]
        public List<ImageFile> Images { get; set; }
    }
}
