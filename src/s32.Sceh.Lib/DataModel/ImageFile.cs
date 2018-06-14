using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace s32.Sceh.DataModel
{
    public class ImageFile
    {
        public ImageFile(ImageDirectory directory)
        {
            Directory = directory;
        }

        public DateTime LastUpdate { get; set; }

        public string ETag { get; set; }

        public string MimeType { get; set; }

        public string Filename { get; set; }

        public Uri ImageUrl { get; set; }

        public ImageDirectory Directory { get; set; }
    }
}