using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using s32.Sceh.Code;

namespace s32.Sceh.Tests
{
    [TestClass]
    public class HttpWebRequestTests
    {
        private const string INVALID_URL = "http://steamcommunity-a.akamaihd.net/economy/image/9C2v1o6cKJ4qEnGqnE7IoTQgZI-VTdwyTBeimAc1o6cKJ4qEnGqnE7IoTQgZI-VTdwyTBeimAcI";
        private const string REDIRECT_URL = "http://steamcommunity-a.akamaihd.net/economy/image/U8721VM9p9C2v1o6cKJ4qEnGqnE7IoTQgZI-VTdwyTBeimAcI";
        private const string VALID_URL = "http://steamcommunity-a.akamaihd.net/economy/image/U8721VM9p9C2v1o6cKJ4qEnGqnE7IoTQgZI-VTdwyTBeimAcIoxXpgK8bPeslY9pPJIvB5IWW2-452kaM8heLSRgleGBobRBx-94a_Mu2eP6WFVy7OQTATLqFkCE1z7DLuuuwgMzYJEnIEygxMkWvN0BD595SOIu0BeYTQ";

        [DataTestMethod]
        [DataRow(VALID_URL)]
        public void CheckAbort(string url)
        {
            const string referer = "https://steamcommunity.com/";
            var uri = new Uri(url);
            HttpWebRequest request = SteamDataDownloader.PrepareRequest(uri, HttpMethod.Get, FileType.AcceptedImageTypes, referer);
            request.AllowAutoRedirect = false;

            Assert.IsFalse(request.HaveResponse);

            var ar = request.BeginGetResponse(null, null);
            Assert.IsFalse(ar.IsCompleted);

            request.Abort();

            ar.AsyncWaitHandle.WaitOne();
            Assert.IsTrue(ar.IsCompleted);

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.EndGetResponse(ar);
            }
            catch (WebException)
            {
            }

            Assert.IsNull(response);
            //Assert.IsFalse(request.HaveResponse);
        }

        [TestMethod]
        public void Poc()
        {
            CancellationToken ct;
            int tmp = 0;
            var cts = new CancellationTokenSource();

            ct = cts.Token;
            ct.Register(() => tmp += 1);

            Assert.IsFalse(ct.IsCancellationRequested);
            Assert.AreEqual(tmp, 0);

            cts.Cancel();

            Assert.IsTrue(ct.IsCancellationRequested);
            Assert.AreEqual(tmp, 1);

            cts.Cancel();

            Assert.IsTrue(ct.IsCancellationRequested);
            Assert.IsTrue(cts.IsCancellationRequested);
            Assert.AreEqual(tmp, 1);

            cts.Dispose();

            Assert.IsTrue(ct.CanBeCanceled);
            Assert.IsTrue(ct.IsCancellationRequested);
            Assert.IsTrue(cts.IsCancellationRequested);

            //const int ID_LENGTH = 4;

            //var ss = Path.GetFileNameWithoutExtension("C:\\ff\\gg\\dd.edf");
            //var ss2 = Path.GetFileNameWithoutExtension("C:\\ff\\dd");
            //var ss3 = Path.GetFileNameWithoutExtension("C:\\ff\\dd\\");

            //for (int i = 0; i < 256; ++i)
            //{
            //    var id1 = new ulong[ID_LENGTH];
            //    id1[i / 64] = 1ul << (i % 64);

            //    var id2 = new ulong[ID_LENGTH];
            //    id2[i >> 6] = 1ul << (i & 0x3F);

            //    Assert.IsTrue(id1.SequenceEqual(id2));
            //}
        }
    }
}
