using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace s32.Sceh.Data
{
    [Serializable]
    [XmlRoot("Directory", Namespace = ScehData.NS_SCEH)]
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

        [XmlElement("Image")]
        public List<ImageFile> Images { get; set; }

        [XmlAttribute]
        public string RelativePath { get; set; }
    }
}