using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace s32.Sceh.Code
{
    public class CardThumbnailRequest
    {
        private string _iconUrl;
        private string _physicalPath;

        public CardThumbnailRequest(string iconUrl, string thumbnailPhisicalPath)
        {
            _iconUrl = iconUrl;
            _physicalPath = thumbnailPhisicalPath;
        }

        public string IconUrl { get { return _iconUrl; } }

        public string PhysicalPath { get { return _physicalPath; } }
    }
}