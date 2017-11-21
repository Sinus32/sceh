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
        private const int IMAGE_FILENAME_LENGTH = 8;
        private const int MINUTES_DELAY = 60 * 24 * 7;
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

        private static void TryDownloadFile(ImageFile image, ref string filePath, bool fileExists)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(image.ImageUrl);
            request.Method = "GET";
            request.Timeout = 10000;
            request.ContinueTimeout = 10000;
            request.ReadWriteTimeout = 10000;
            request.Accept = FileType.AcceptedImageTypes;
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Referer = "http://steamcommunity.com/";

            if (fileExists)
            {
                if (!String.IsNullOrEmpty(image.ETag))
                    request.Headers.Add(HttpRequestHeader.IfNoneMatch, image.ETag);
                else if (image.LastUpdate.Year > 1900)
                    request.IfModifiedSince = image.LastUpdate;
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var fileType = FileType.FindByMimeType(response.ContentType);
                    if (fileType == null)
                        throw new NotSupportedException(String.Format("Files of type '{0}' are not supported", response.ContentType));

                    if (String.IsNullOrEmpty(filePath))
                    {
                        image.Filename = RandomString.Generate(IMAGE_FILENAME_LENGTH) + fileType.Extension;
                        filePath = DataManager.LocalFilePath(image);
                    }
                    else if (Path.GetExtension(image.Filename) != fileType.Extension)
                    {
                        if (fileExists)
                            File.Delete(filePath);
                        var baseFilename = Path.GetFileNameWithoutExtension(image.Filename);
                        if (baseFilename.Length != IMAGE_FILENAME_LENGTH)
                            baseFilename = RandomString.Generate(IMAGE_FILENAME_LENGTH);
                        image.Filename = baseFilename + fileType.Extension;
                        filePath = DataManager.LocalFilePath(image);
                    }

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
                    image.MimeType = fileType.MimeType;
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
                    var fileExists = imagePath != null && File.Exists(imagePath);

                    if (fileExists && image.LastUpdate > reqTime)
                        continue;

                    try
                    {
                        TryDownloadFile(image, ref imagePath, fileExists);
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
            private Action<ImageFile, string> _callback;
            private ManualResetEvent _terminateEvent;
            private Thread _thread;

            public Worker(Action<ImageFile, string> fileDownloadedCallback)
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

            public void Stop()
            {
                if (!IsWorking)
                    return;

                if (_terminateEvent != null)
                    _terminateEvent.Set();
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
                            _callback(image, imagePath);
                        threadWait = 10;
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