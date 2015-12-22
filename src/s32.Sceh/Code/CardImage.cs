using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace s32.Sceh.Code
{
    public class CardImage
    {
        private string _iconUrl;
        private string _thumbnailPhisicalPath;

        public CardImage(string iconUrl, string thumbnailPhisicalPath)
        {
            _iconUrl = iconUrl;
            _thumbnailPhisicalPath = thumbnailPhisicalPath;
        }
    }
}