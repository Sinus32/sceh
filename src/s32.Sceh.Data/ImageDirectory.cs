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
    [XmlRoot("Directory", Namespace = ScehData.XML_NAMESPACE)]
    public class ImageDirectory
    {
        public const int FILENAME_LENGTH = 6;
        private Dictionary<string, ImageFile> _urlLookup;

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

        public ImageFile GetOrCreateImageByUrl(string imageUrl, out bool isNew)
        {
            if (String.IsNullOrEmpty(imageUrl))
            {
                isNew = false;
                return null;
            }

            if (_urlLookup == null)
            {
                lock (this)
                {
                    if (_urlLookup == null)
                    {
                        _urlLookup = new Dictionary<string, ImageFile>();
                        foreach (var dt in Images)
                            _urlLookup[dt.ImageUrl] = dt;
                    }
                }
            }

            ImageFile result;
            if (_urlLookup.TryGetValue(imageUrl, out result))
            {
                isNew = false;
                return result;
            }

            lock (_urlLookup)
            {
                result = new ImageFile();
                result.ImageUrl = imageUrl;
                result.Directory = this;
                result.Filename = RandomString.Generate(FILENAME_LENGTH);
                var ext = Path.GetExtension(imageUrl);
                if (!String.IsNullOrEmpty(ext))
                    result.Filename += ext;
                Images.Add(result);
                _urlLookup[imageUrl] = result;
            }

            isNew = true;
            return result;
        }
    }
}