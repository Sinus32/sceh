using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using s32.Sceh.Data;

namespace s32.Sceh.Code
{
    public class ImageDownloader
    {
        private static readonly ConcurrentQueue<ImageFile>[] _queue;

        static ImageDownloader()
        {
            _queue = new ConcurrentQueue<ImageFile>[DownloadPriority.Order.Count];
            foreach (var priority in DownloadPriority.Order)
                _queue[priority.OrderIndex] = new ConcurrentQueue<ImageFile>();
        }

        public static void EnqueueDownload(ImageFile image, DownloadPriority priority)
        {
            var queue = _queue[priority.OrderIndex];
            queue.Enqueue(image);
        }

        public static bool DownloadNext(out ImageFile image, out string imagePath)
        {
            var reqTime = DateTime.Now.AddMinutes(-1);
            image = null;
            foreach (var priority in DownloadPriority.Order)
            {
                if (_queue[priority.OrderIndex].TryDequeue(out image))
                {
                    if (image.LastUpdate < reqTime)
                        break;
                    else
                        image = null;
                }
            }

            if (image == null)
            {
                imagePath = null;
                return false;
            }

            imagePath = ScehData.LocalFilePath(image);

            try
            {
                TryDownloadFile(image, imagePath);
                return true;
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                else
                    Debug.WriteLine("Error while downloading file: {0}", ex.Message);
                return false;
            }
        }

        private static void TryDownloadFile(ImageFile image, string filePath)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(image.ImageUrl);
            request.Method = "GET";
            request.Timeout = 10000;
            request.Accept = "image/png,image/jpeg";
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Referer = "http://steamcommunity.com/";

            if (File.Exists(filePath))
            {
                if (image.LastUpdate.Year > 1900)
                    request.IfModifiedSince = image.LastUpdate;
                if (!String.IsNullOrEmpty(image.ETag))
                    request.Headers.Add(HttpRequestHeader.IfNoneMatch, image.ETag);
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    using (var fileStream = File.OpenWrite(filePath))
                    using (var sourceStream = response.GetResponseStream())
                    {
                        const int BUFFER_SIZE = 0x10000;
                        byte[] buffer = new byte[BUFFER_SIZE];
                        int readed;
                        while ((readed = sourceStream.Read(buffer, 0, BUFFER_SIZE)) > 0)
                            fileStream.Write(buffer, 0, readed);
                    }

                    image.LastUpdate = DateTime.Now;
                    image.ETag = response.Headers[HttpResponseHeader.ETag];
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response == null || response.StatusCode != HttpStatusCode.NotModified)
                    {
                        if (response != null)
                            Debug.WriteLine("Cannot download file - http status: {0}", response.StatusCode);
                        else
                            Debug.WriteLine("Cannot download file - no response");
                    }
                }
                else
                {
                    throw;
                }
            }
        }
    }
}