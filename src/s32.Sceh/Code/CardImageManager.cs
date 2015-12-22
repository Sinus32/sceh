using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace s32.Sceh.Code
{
    public class CardImageManager
    {
        public const string IMAGE_SOURCE = "http://steamcommunity-a.akamaihd.net/economy/image/";
        public const string THUMBNAIL_PATH = "~/Content/cards";
        public const string THUMBNAIL_EXT = ".png";

        private static ConcurrentQueue<CardImage> _cardsToProcess;
        private static MvcApplication _app;
        private static string _thumbnailFullPath;
        private Dictionary<string, string> _tmp;

        static CardImageManager()
        {
            _cardsToProcess = new ConcurrentQueue<CardImage>();
        }

        public CardImageManager()
        {
            _tmp = new Dictionary<string, string>();
        }

        public static void SetApp(MvcApplication app)
        {
            _app = app;
            _thumbnailFullPath = app.Server.MapPath(THUMBNAIL_PATH);
        }

        public string GetCardThumbnailUrl(string iconUrl)
        {
            string ret;
            if (_tmp.TryGetValue(iconUrl, out ret))
                return ret;

            string filename = MakeShortFilename(iconUrl);
            var thumbnailUrl = Path.Combine(filename.Remove(2), filename.Remove(4), filename) + THUMBNAIL_EXT;
            var thumbnailPhisicalPath = Path.Combine(_thumbnailFullPath, thumbnailUrl);

            if (_cardsToProcess.Count < 2000)
            {
                var card = new CardImage(iconUrl, thumbnailPhisicalPath);
                _cardsToProcess.Enqueue(card);
            }

            if (File.Exists(thumbnailPhisicalPath))
                ret = VirtualPathUtility.ToAbsolute(Path.Combine(THUMBNAIL_PATH, thumbnailUrl));
            else
                ret = String.Concat(IMAGE_SOURCE, iconUrl);

            _tmp.Add(iconUrl, ret);
            return ret;
        }

        private string MakeShortFilename(string iconUrl)
        {
            if (iconUrl.Length <= 32)
                return iconUrl;

            var pad = iconUrl.Length % 4;
            if (pad > 0)
                pad = 4 - pad;
            var newLength = iconUrl.Length + pad;
            var source = new char[newLength];
            int i;
            for (i = 0; i < iconUrl.Length; ++i)
            {
                switch (iconUrl[i])
                {
                    case '-': source[i] = '+'; break;
                    case '_': source[i] = '/'; break;
                    default: source[i] = iconUrl[i]; break;
                }
            }
            while (i < newLength)
                source[i++] = '=';

            var bytes = Convert.FromBase64CharArray(source, 0, newLength);
            var result = new byte[24];
            for (i = 0; i < bytes.Length; ++i)
                result[i % 24] ^= bytes[i];

            var ret = Convert.ToBase64CharArray(result, 0, 24, source, 0);
            for (i = 0; i < 32; ++i)
            {
                switch (source[i])
                {
                    case '+': source[i] = '-'; break;
                    case '/': source[i] = '_'; break;
                }
            }

            return new String(source, 0, 32);
        }

        public class Worker
        {
            private MvcApplication _app;
            private string _thumbnailFullPath;
            private ManualResetEvent _terminateEvent;

            public Worker(MvcApplication app)
            {
                _app = app;
                _thumbnailFullPath = app.Server.MapPath(THUMBNAIL_PATH);
                _terminateEvent = new ManualResetEvent(false);
            }

            public void Terminate()
            {
                _terminateEvent.Set();
            }

            public void ThreadStart()
            {
                while (!_terminateEvent.WaitOne(250))
                {
                    CardImage card;
                    if (_cardsToProcess.TryDequeue(out card))
                    {
                    }
                }
            }
        }
    }
}