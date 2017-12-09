using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace s32.Sceh.DataModel
{
    public class ImageDirectory
    {
        public ImageDirectory(string relativePath)
        {
            Images = new List<ImageFile>();
            RelativePath = relativePath;
        }

        public List<ImageFile> Images { get; set; }

        public string RelativePath { get; set; }
    }
}