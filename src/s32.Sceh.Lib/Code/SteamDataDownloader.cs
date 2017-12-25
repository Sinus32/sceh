using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using s32.Sceh.Classes;
using s32.Sceh.DataModel;
using s32.Sceh.SteamApi;

namespace s32.Sceh.Code
{
    public static class SteamDataDownloader
    {
        private static readonly Regex _steamidRe = new Regex("^[0-9]{3,18}$", RegexOptions.None);

        private static readonly Regex _userurlRe = new Regex("^[0-9a-zA-Z_-]{3,20}$", RegexOptions.None);

        private static DateTime _nextCall = DateTime.MinValue;

        public enum GetProfileError
        {
            Success,
            WrongProfile,
            DeserializationError
        }

        public static List<Card> GetCardsA(SteamProfileKey profile, out string errorMessage)
        {
            var inventoryUri = GetProfileUri(profile, SteamUrlPattern.ApiGetInventoryA);
            if (inventoryUri == null)
            {
                errorMessage = "Invalid profile data";
                return null;
            }
            string rawJson;
            HttpStatusCode statusCode;
            using (var response = DoRequest(() => PrepareRequest(inventoryUri, "application/json"), out statusCode))
            {
                if (response == null)
                {
                    errorMessage = String.Format("Http error: {0} ({1})", (int)statusCode, statusCode);
                    return null;
                }

                if (!response.ContentType.StartsWith("application/json"))
                {
                    errorMessage = "Wrong profile";
                    return null;
                }

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    rawJson = reader.ReadToEnd();
                }
            }

            var jss = new JsonSerializerSettings();
            jss.MissingMemberHandling = MissingMemberHandling.Ignore;
            jss.NullValueHandling = NullValueHandling.Include;
            jss.ObjectCreationHandling = ObjectCreationHandling.Replace;
            var ret = JsonConvert.DeserializeObject<RgInventoryResp>(rawJson, jss);

            if (!ret.Success)
            {
                errorMessage = ret.Error ?? "Wrong query";
                return null;
            }

            var result = new List<Card>();

            Load(profile, result, ret);

            while (ret.More)
            {
                var nextUri = new Uri(String.Concat(inventoryUri.ToString(), "?start=", ret.MoreStart));

                using (var response = DoRequest(() => PrepareRequest(nextUri, "application/json"), out statusCode))
                {
                    if (response == null)
                    {
                        errorMessage = String.Format("Http error: {0} ({1})", (int)statusCode, statusCode);
                        return null;
                    }

                    if (!response.ContentType.StartsWith("application/json"))
                    {
                        errorMessage = "Wrong profile";
                        return null;
                    }

                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        rawJson = reader.ReadToEnd();
                    }
                }

                ret = JsonConvert.DeserializeObject<RgInventoryResp>(rawJson, jss);

                if (!ret.Success)
                {
                    errorMessage = ret.Error ?? "Wrong query";
                    return null;
                }

                Load(profile, result, ret);
            }

            result.Sort(CardComparison);

            errorMessage = null;
            return result;
        }

        public static List<Card> GetCardsB(SteamProfileKey profile, out string errorMessage)
        {
            var inventoryUri = GetProfileUri(profile, SteamUrlPattern.ApiGetInventoryB);
            if (inventoryUri == null)
            {
                errorMessage = "Invalid profile data";
                return null;
            }
            var referer = GetProfileUri(profile, SteamUrlPattern.Inventory).ToString();

            string rawJson;
            HttpStatusCode statusCode;
            using (var response = DoRequest(() => PrepareRequest(inventoryUri, "application/json", referer: referer), out statusCode))
            {
                if (response == null)
                {
                    errorMessage = String.Format("Http error: {0} ({1})", (int)statusCode, statusCode);
                    return null;
                }

                if (!response.ContentType.StartsWith("application/json"))
                {
                    errorMessage = "Wrong profile";
                    return null;
                }

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    rawJson = reader.ReadToEnd();
                }
            }

            var jss = new JsonSerializerSettings();
            jss.MissingMemberHandling = MissingMemberHandling.Ignore;
            jss.NullValueHandling = NullValueHandling.Include;
            jss.ObjectCreationHandling = ObjectCreationHandling.Replace;
            var ret = JsonConvert.DeserializeObject<ApiInventoryResp>(rawJson, jss);

            if (ret == null)
            {
                errorMessage = "Wrong query";
                return null;
            }

            var result = new List<Card>();

            Load(profile, result, ret);

            while (ret.MoreItems)
            {
                var nextUri = new Uri(String.Concat(inventoryUri.ToString(), "&start_assetid=", ret.LastAssetId));

                using (var response = DoRequest(() => PrepareRequest(nextUri, "application/json", referer: referer), out statusCode))
                {
                    if (response == null)
                    {
                        errorMessage = String.Format("Http error: {0} ({1})", (int)statusCode, statusCode);
                        return null;
                    }

                    if (!response.ContentType.StartsWith("application/json"))
                    {
                        errorMessage = "Wrong profile";
                        return null;
                    }

                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        rawJson = reader.ReadToEnd();
                    }
                }

                ret = JsonConvert.DeserializeObject<ApiInventoryResp>(rawJson, jss);

                if (ret == null)
                {
                    errorMessage = "Wrong query";
                    return null;
                }

                Load(profile, result, ret);
            }

            result.Sort(CardComparison);

            errorMessage = null;
            return result;
        }

        public static SteamProfileResp GetProfile(Uri profileUri, out GetProfileError error)
        {
            string rawXml;
            HttpStatusCode statusCode;
            using (var response = DoRequest(() => PrepareRequest(profileUri, "text/xml"), out statusCode))
            {
                if (response == null)
                {
                    error = GetProfileError.WrongProfile;
                    return null;
                }

                if (!response.ContentType.StartsWith("text/xml"))
                {
                    error = GetProfileError.WrongProfile;
                    return null;
                }

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    rawXml = reader.ReadToEnd();
                }

                if (!rawXml.Contains("<profile>"))
                {
                    error = GetProfileError.WrongProfile;
                    return null;
                }
            }

            var ser = new XmlSerializer(typeof(SteamProfileResp));

            SteamProfileResp result = null;
            try
            {
                using (var reader = new StringReader(rawXml))
                    result = (SteamProfileResp)ser.Deserialize(reader);
            }
            catch (InvalidOperationException)
            {
                error = GetProfileError.DeserializationError;
                return null;
            }

            error = GetProfileError.Success;
            return result;
        }

        public static SteamProfileKey GetProfileKey(string idOrUrl)
        {
            long steamId = 0L;
            string customUrl = null;

            if (TryExtractSteamIdFromUrl(idOrUrl, out steamId))
                return new SteamProfileKey(steamId, customUrl);

            if (TryExtractCustomUrlFromUrl(idOrUrl, out customUrl))
                return new SteamProfileKey(steamId, customUrl);

            if (_steamidRe.IsMatch(idOrUrl))
                steamId = Int64.Parse(idOrUrl);
            else if (_userurlRe.IsMatch(idOrUrl))
                customUrl = idOrUrl;
            else
                return null;

            return new SteamProfileKey(steamId, customUrl);
        }

        public static Uri GetProfileUri(string idOrUrl, SteamUrlPattern page)
        {
            return GetProfileKey(idOrUrl).GetProfileUri(page);
        }

        public static Uri GetProfileUri(long steamId, string customUrl, SteamUrlPattern page)
        {
            if (steamId > 0L && page.HasSupportForSteamId)
                return page.GetUrlBySteamId(steamId);
            else if (!String.IsNullOrEmpty(customUrl) && page.HasSupportForCustomUrl)
                return page.GetUrlByCustomUrl(customUrl);
            else
                return null;
        }

        public static Uri GetProfileUri(this SteamProfileKey steamProfile, SteamUrlPattern page)
        {
            if (steamProfile == null)
                return null;

            return GetProfileUri(steamProfile.SteamId, steamProfile.CustomUrl, page);
        }

        private static int CardComparison(Card x, Card y)
        {
            long ret = x.MarketFeeApp - y.MarketFeeApp;
            if (ret == 0)
            {
                ret = x.ClassId - y.ClassId;
                if (ret == 0)
                {
                    ret = x.InstanceId - y.InstanceId;
                    if (ret == 0)
                        ret = x.Id - y.Id;
                }
            }
            if (ret < 0L)
                return -1;
            if (ret > 0L)
                return 1;
            return 0;
        }

        private static HttpWebResponse DoRequest(Func<HttpWebRequest> makeRequestFunc, out HttpStatusCode statusCode)
        {
            var delay = 1;
            CommunicationState.Instance.IsInProgress = true;
            try
            {
                while (true)
                {
                    CommunicationState.Instance.IsRepeating = delay > 1;
                    var now = DateTime.Now;
                    if (_nextCall > now)
                    {
                        var diff = (int)(_nextCall - now).TotalMilliseconds + 1;
                        Thread.Sleep(diff);
                    }

                    try
                    {
                        var request = makeRequestFunc();
                        CommunicationState.Instance.RequestCount += 1;
                        statusCode = HttpStatusCode.OK;
                        return (HttpWebResponse)request.GetResponse();
                    }
                    catch (WebException ex)
                    {
                        if (ex.Status == WebExceptionStatus.ProtocolError)
                        {
                            var response = ex.Response as HttpWebResponse;
                            if (response != null)
                            {
                                statusCode = response.StatusCode;
                                if (response.StatusCode == (HttpStatusCode)429)
                                {
                                    if (delay < 5)
                                        delay = 5;
                                    else if (delay < 10)
                                        delay += 1;
                                    else
                                        throw;
                                    response.Close();
                                    continue;
                                }
                                if (response.StatusCode == HttpStatusCode.Forbidden)
                                {
                                    return null;
                                }
                                response.Close();
                            }
                        }
                        else if (ex.Status == WebExceptionStatus.Timeout)
                        {
                            if (delay < 7)
                                delay = 7;
                            else if (delay < 15)
                                delay += 2;
                            else
                                throw;
                            continue;
                        }
                        throw;
                    }
                    finally
                    {
                        _nextCall = DateTime.Now.AddSeconds(delay);
                    }
                }
            }
            finally
            {
                CommunicationState.Instance.IsInProgress = false;
                CommunicationState.Instance.IsRepeating = false;
            }
        }

        private static void Load(SteamProfileKey owner, List<Card> result, RgInventoryResp ret)
        {
            foreach (var dt in ret.RgInventory.Values)
            {
                var key = new RgInventoryResp.RgDescriptionKey(dt.ClassId, dt.InstanceId);
                var desc = ret.RgDescriptions[key];
                if (desc.Tradable)
                    result.Add(new Card(owner, dt, desc));
            }
        }

        private static void Load(SteamProfileKey owner, List<Card> result, ApiInventoryResp ret)
        {
            var dict = new Dictionary<CardEqualityKey, ApiInventoryResp.Description>();
            foreach (var dt in ret.Descriptions)
                dict.Add(new CardEqualityKey(dt.ClassId, dt.InstanceId), dt);
            foreach (var dt in ret.Assets)
            {
                var desc = dict[new CardEqualityKey(dt.ClassId, dt.InstanceId)];
                if (desc.Tradable)
                    result.Add(new Card(owner, dt, desc));
            }
        }

        private static HttpWebRequest PrepareRequest(Uri uri, string accept, HttpMethod method = null, string referer = null)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = (method ?? HttpMethod.Get).Method;
            request.Timeout = 10000;
            request.ContinueTimeout = 10000;
            request.ReadWriteTimeout = 10000;
            request.Accept = accept;
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Referer = referer ?? "http://steamcommunity.com/";
            return request;
        }

        private static bool TryExtractCustomUrlFromUrl(string idOrUrl, out string customUrl)
        {
            const string urlPart = "steamcommunity.com/id/";

            var pos = idOrUrl.IndexOf(urlPart);
            if (pos >= 0)
            {
                var data = idOrUrl.Substring(pos + urlPart.Length);
                int i;
                for (i = 0; i < data.Length; ++i)
                    if (!Char.IsLetterOrDigit(data[i]) && data[i] != '_' && data[i] != '-')
                        break;
                if (i > 0)
                {
                    customUrl = data.Length > i ? data.Remove(i) : data;
                    return true;
                }
            }

            customUrl = null;
            return false;
        }

        private static bool TryExtractSteamIdFromUrl(string idOrUrl, out long steamId)
        {
            const string urlPart = "steamcommunity.com/profiles/";

            var pos = idOrUrl.IndexOf(urlPart);
            if (pos >= 0)
            {
                var data = idOrUrl.Substring(pos + urlPart.Length);
                int i;
                for (i = 0; i < data.Length; ++i)
                    if (!Char.IsDigit(data[i]))
                        break;
                if (i > 0)
                {
                    var number = data.Length > i ? data.Remove(i) : data;
                    steamId = Int64.Parse(number, CultureInfo.InvariantCulture);
                    return true;
                }
            }

            steamId = 0L;
            return false;
        }
    }
}