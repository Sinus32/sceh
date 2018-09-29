using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.Code;
using s32.Sceh.DataModel;

namespace s32.Sceh.Jobs.Impl
{
    public class ImageDownloadJob : StatefulJob
    {
        const int BUFFER_SIZE = 32 * 1024;
        private readonly ImageFile _image;
        private IAsyncResult _getResponseAsyncResult;
        private HttpWebRequest _request;
        private HttpWebResponse _response;
        private JobStage _stage;
        private FileStream _fileStream;
        private Stream _sourceStream;

        public ImageDownloadJob(ImageFile image)
            : base(image.Id)
        {
            _image = image;
            _stage = JobStage.MakeRequest;
        }

        private enum JobStage
        {
            MakeRequest,
            WaitForResponse,
            ReadResponse,
        }

        protected override JobResult DoWork()
        {
            switch (_stage)
            {
                case JobStage.MakeRequest:
                    return MakeRequestStage();

                case JobStage.WaitForResponse:
                    if (_getResponseAsyncResult.IsCompleted)
                        goto case JobStage.ReadResponse;
                    else
                        return JobResult.InProgress();

                case JobStage.ReadResponse:
                    return ReadResponseStage();

                default:
                    throw new InvalidOperationException("The job is in invalid stage");
            }
        }

        private JobResult MakeRequestStage()
        {
            const string referer = "https://steamcommunity.com/";

            _request = SteamDataDownloader.PrepareRequest(_image.ImageUrl, HttpMethod.Get, FileType.AcceptedImageTypes, referer);

            var imagePath = DataManager.LocalFilePath(_image);
            var fileExists = imagePath != null && File.Exists(imagePath);

            if (fileExists)
            {
                if (!String.IsNullOrEmpty(_image.ETag))
                    _request.Headers.Add(HttpRequestHeader.IfNoneMatch, _image.ETag);
                else if (_image.LastUpdate.Year > 1900)
                    _request.IfModifiedSince = _image.LastUpdate;
            }

            _getResponseAsyncResult = _request.BeginGetResponse(null, null);
            _stage = JobStage.WaitForResponse;

            return JobResult.InProgress();
        }

        private JobResult ReadResponseStage()
        {
            try
            {
                _response = (HttpWebResponse)_request.EndGetResponse(_getResponseAsyncResult);

                var fileType = FileType.FindByMimeType(_response.ContentType);
                if (fileType == null)
                    throw new NotSupportedException(String.Format("Files of type '{0}' are not supported", _response.ContentType));

                var oldImagePath = DataManager.LocalFilePath(_image);
                _image.Filename = _image.Id + fileType.Extension;
                var imagePath = DataManager.LocalFilePath(_image);

                if (oldImagePath != null && !String.Equals(oldImagePath, imagePath, StringComparison.Ordinal) && File.Exists(oldImagePath))
                    File.Delete(oldImagePath);

                Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
                _sourceStream = _response.GetResponseStream();
                _fileStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write, FileShare.None, BUFFER_SIZE, true);
                var contentLength = _response.ContentLength;
                if (contentLength > 0L)
                    _fileStream.SetLength(contentLength);

                return JobResult.InProgress();
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.RequestCanceled)
                    return JobResult.Canceled();

                _response = ex.Response as HttpWebResponse;
                if (_response != null && _response.StatusCode == HttpStatusCode.NotModified)
                {
                    _image.LastUpdate = DateTime.Now;
                    CommunicationState.Instance.ImagesNotModified += 1;
                    return JobResult.Finished(ImageDownloadResult.NotModified);
                }
                else
                {
                    if (_response != null)
                        Debug.WriteLine("Cannot download file - http status: {0}", _response.StatusCode);
                    else
                        Debug.WriteLine("Cannot download file - no response");
                    return JobResult.Exception(ex);
                }
            }

        }
    }
}
