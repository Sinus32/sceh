using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using s32.Sceh.Code;
using System.IO;

namespace s32.Sceh.DataModel
{
    public class ImageFile
    {
        public const int FILENAME_LENGTH = 12;
        private readonly ImageDirectory _directory;
        private string _filename;
        private string _id;

        public ImageFile(ImageDirectory directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            _directory = directory;
        }

        public ImageDirectory Directory
        {
            get { return _directory; }
        }

        public string ETag { get; set; }

        public string Filename
        {
            get { return _filename; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    _filename = value;
                }
                else if (!String.Equals(value, _filename, StringComparison.Ordinal))
                {
                    _filename = value;
                    _id = null;
                }
            }
        }

        public string Id
        {
            get
            {
                if (_id == null)
                {
                    if (String.IsNullOrEmpty(_filename))
                        _id = RandomString.Generate(FILENAME_LENGTH);
                    else
                        _id = Path.GetFileNameWithoutExtension(_filename);
                }
                return _id;
            }
        }

        public Uri ImageUrl { get; set; }
        public DateTime LastUpdate { get; set; }
        public string MimeType { get; set; }
    }
}
