using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Code
{
    public class FileType
    {
        public static readonly FileType[] AllTypes;
        public static readonly FileType Jpeg = new FileType("image/jpeg", ".jpg");
        public static readonly FileType Png = new FileType("image/png", ".png");
        public static readonly string AcceptedImageTypes;

        static FileType()
        {
            AllTypes = new FileType[] { Jpeg, Png };
            AcceptedImageTypes = String.Join(",", AllTypes.Select(q => q.MimeType));
        }

        public FileType(string mimeType, string extension)
        {
            MimeType = mimeType;
            Extension = extension;
        }

        public string Extension { get; private set; }
        public string MimeType { get; private set; }

        public static FileType FindByMimeType(string mimeType)
        {
            if (String.IsNullOrEmpty(mimeType))
                return null;

            foreach (var dt in AllTypes)
                if (mimeType.StartsWith(dt.MimeType, StringComparison.InvariantCultureIgnoreCase))
                    return dt;

            return null;
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", MimeType, Extension);
        }
    }
}