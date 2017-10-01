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
using s32.Sceh.DataStore;

namespace s32.Sceh.Code
{
    public class ImageDownloader
    {
        public const int MINUTES_DELAY = 15;
        private static readonly ConcurrentDictionary<ImageFile, int> _isInQueue;
        private static readonly ConcurrentQueue<ImageFile>[] _queue;

        static ImageDownloader()
        {
            _queue = new ConcurrentQueue<ImageFile>[DownloadPriority.Order.Count];
            foreach (var priority in DownloadPriority.Order)
                _queue[priority.OrderIndex] = new ConcurrentQueue<ImageFile>();
            _isInQueue = new ConcurrentDictionary<ImageFile, int>();
        }

        public static bool EnqueueDownload(ImageFile image, DownloadPriority priority, bool forceDownload)
        {
            var reqTime = DateTime.Now.AddMinutes(-MINUTES_DELAY);
            if (forceDownload || image.LastUpdate < reqTime)
            {
                if (_isInQueue.TryAdd(image, priority.OrderIndex))
                    _queue[priority.OrderIndex].Enqueue(image);
                return true;
            }
            return false;
        }

        private static void TryDownloadFile(ImageFile image, string filePath, bool fileExists)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(image.ImageUrl);
            request.Method = "GET";
            request.Timeout = 10000;
            request.Accept = "image/png,image/jpeg";
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Referer = "http://steamcommunity.com/";

            if (fileExists)
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
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        private static bool TryDownloadNext(out ImageFile image, out string imagePath)
        {
            var reqTime = DateTime.Now.AddMinutes(-MINUTES_DELAY);
            image = null;
            imagePath = null;

            foreach (var priority in DownloadPriority.Order)
            {
                while (_queue[priority.OrderIndex].TryDequeue(out image))
                {
                    int dummy;
                    _isInQueue.TryRemove(image, out dummy);

                    imagePath = DataManager.LocalFilePath(image);
                    var fileExists = File.Exists(imagePath);

                    if (fileExists && image.LastUpdate > reqTime)
                        continue;

                    try
                    {
                        TryDownloadFile(image, imagePath, fileExists);
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
            }

            return false;
        }

        public class Worker
        {
            private Action<string> _callback;
            private ManualResetEvent _terminateEvent;
            private Thread _thread;

            public Worker(Action<string> fileDownloadedCallback)
            {
                _callback = fileDownloadedCallback;
            }

            public bool IsWorking
            {
                get { return _thread != null && _thread.IsAlive; }
            }

            public void Start()
            {
                if (_thread != null)
                    throw new InvalidOperationException("Start already called");

                _thread = new Thread(DownloadImages);
                _terminateEvent = new ManualResetEvent(false);
                _thread.Start();
            }

            public void StopAndJoin(int millisecondsTimeout)
            {
                if (!IsWorking)
                    return;

                if (_terminateEvent != null)
                    _terminateEvent.Set();

                _thread.Join(millisecondsTimeout);
            }

            private void DownloadImages()
            {
                var threadWait = 3000;

                while (!_terminateEvent.WaitOne(threadWait))
                {
                    ImageFile image;
                    string imagePath;
                    if (ImageDownloader.TryDownloadNext(out image, out imagePath))
                    {
                        if (_callback != null)
                            _callback(imagePath);
                        threadWait = 50;
                    }
                    else
                    {
                        threadWait = 500;
                    }
                }
            }
        }
    }
}