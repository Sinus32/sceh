using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace s32.Sceh.Code
{
    public class CardImageManager
    {
        public const string IMAGE_SOURCE = "http://steamcommunity-a.akamaihd.net/economy/image/";
        public const string THUMBNAIL_PATH = "~/Content/cards";
        public const string THUMBNAIL_EXT = ".png";
        public const int THUMBNAIL_WIDTH = 96;

        private static ConcurrentQueue<CardThumbnailRequest> _cardsToProcess;
        private static MvcApplication _app;
        private static string _thumbnailFullPath;
        private Dictionary<string, string> _tmp;

        static CardImageManager()
        {
            _cardsToProcess = new ConcurrentQueue<CardThumbnailRequest>();
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

        public int ThumbnailsUsed { get; private set; }

        public int OryginalsUsed { get; private set; }

        public string GetCardThumbnailUrl(string iconUrl)
        {
            string ret;
            if (_tmp.TryGetValue(iconUrl, out ret))
                return ret;

            string filename = MakeShortFilename(iconUrl);
            var thumbnailUrl = Path.Combine(filename.Remove(2), filename) + THUMBNAIL_EXT;
            var thumbnailPhysicalPath = Path.Combine(_thumbnailFullPath, thumbnailUrl);

            if (_cardsToProcess.Count < 20000)
            {
                var card = new CardThumbnailRequest(iconUrl, thumbnailPhysicalPath);
                _cardsToProcess.Enqueue(card);
            }

            if (File.Exists(thumbnailPhysicalPath))
            {
                ret = VirtualPathUtility.ToAbsolute(Path.Combine(THUMBNAIL_PATH, thumbnailUrl));
                ThumbnailsUsed += 1;
            }
            else
            {
                ret = String.Concat(IMAGE_SOURCE, iconUrl);
                OryginalsUsed += 1;
            }

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
            var result = new byte[20];
            for (i = 0; i < bytes.Length; ++i)
                result[i % 20] ^= bytes[i];

            return Base32.ToBase32String(result);
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
                    CardThumbnailRequest card;
                    while (_cardsToProcess.TryDequeue(out card))
                    {
                        if (File.Exists(card.PhysicalPath))
                        {
                            var lastMod = File.GetLastWriteTime(card.PhysicalPath);
                            if (lastMod.AddDays(1) > DateTime.Now)
                                continue;
                        }

                        try
                        {
                            TryMakeThumbnail(card);
                        }
                        catch (Exception ex)
                        {
                            if (Debugger.IsAttached)
                                Debugger.Break();
                            else
                                Debug.WriteLine("Error while creating thumbnail: {0}", ex.Message);
                        }
                        break;
                    }
                }
            }

            private void TryMakeThumbnail(CardThumbnailRequest card)
            {
                var url = String.Concat(IMAGE_SOURCE, card.IconUrl);
                var request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 10000;
                request.Accept = "image/png,image/jpeg";
                request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Referer = "http://steamcommunity.com/";

                Image image = null, thumbnailImage = null;
                try
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                        image = Image.FromStream(response.GetResponseStream());

                    int w = THUMBNAIL_WIDTH, h = THUMBNAIL_WIDTH;
                    if (image.Width > THUMBNAIL_WIDTH || image.Height > THUMBNAIL_WIDTH)
                    {
                        if (image.Width > image.Height)
                            h = (int)Math.Ceiling((image.Height * THUMBNAIL_WIDTH) / ((double)image.Width));
                        else
                            w = (int)Math.Ceiling((image.Width * THUMBNAIL_WIDTH) / ((double)image.Height));
                    }
                    else
                    {
                        thumbnailImage = image;
                        image = null;
                    }

                    if (image != null)
                        thumbnailImage = image.GetThumbnailImage(w, h, GetThumbnailImageAbortCallback, IntPtr.Zero);

                    Directory.CreateDirectory(Path.GetDirectoryName(card.PhysicalPath));
                    thumbnailImage.Save(card.PhysicalPath, ImageFormat.Png);
                }
                finally
                {
                    if (image != null)
                        image.Dispose();
                    if (thumbnailImage != null)
                        thumbnailImage.Dispose();
                }
            }

            private bool GetThumbnailImageAbortCallback()
            {
                return false;
            }
        }
    }
}