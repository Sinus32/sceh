﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using s32.Sceh.Classes;
using s32.Sceh.DataModel;
using s32.Sceh.DataStore;
using s32.Sceh.SteamApi;

namespace s32.Sceh.Code
{
    public static class SteamDataDownloader
    {
        public static readonly DebugInfo Info = new DebugInfo();

        public const string SteamCommunityPageByCustomUrl = "http://steamcommunity.com/id/";

        public const string SteamCommunityPageBySteamId = "http://steamcommunity.com/profiles/";

        private static readonly Regex _steamidRe = new Regex("^[0-9]{3,18}$", RegexOptions.None);

        private static readonly Regex _userurlRe = new Regex("^[0-9a-zA-Z_-]{3,20}$", RegexOptions.None);

        private static DateTime _nextCall = DateTime.MinValue;

        public enum GetProfileError
        {
            Success,
            WrongProfile,
            DeserializationError
        }

        public static List<Card> GetCards(Uri inventoryUri, out string errorMessage)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(inventoryUri);
            request.Method = "GET";
            request.Timeout = 10000;
            request.Accept = "application/json";
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Referer = "http://steamcommunity.com/";

            string rawJson;
            HttpStatusCode statusCode;
            using (var response = DoRequest(request, out statusCode))
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

            Load(result, ret);

            while (ret.More)
            {
                var nextUri = new Uri(String.Concat(inventoryUri.ToString(), "?start=", ret.MoreStart));
                request = (HttpWebRequest)HttpWebRequest.Create(nextUri);
                request.Method = "GET";
                request.Timeout = 10000;
                request.Accept = "application/json";
                request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Referer = "http://steamcommunity.com/";

                using (var response = DoRequest(request, out statusCode))
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

                Load(result, ret);
            }

            result.Sort(CardComparison);

            errorMessage = null;
            return result;
        }

        public static List<Card> GetCards2(SteamProfile profile, out string errorMessage)
        {
            var inventoryUri = GetProfileUri(profile, SteamUrlPattern.Api2GetInventory);
            if (inventoryUri == null)
            {
                errorMessage = "Invalid profile data";
                return null;
            }
            var referer = GetProfileUri(profile, SteamUrlPattern.Inventory).ToString();
            var request = (HttpWebRequest)HttpWebRequest.Create(inventoryUri);
            request.Method = "GET";
            request.Timeout = 10000;
            request.Accept = "application/json";
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Referer = referer;

            string rawJson;
            HttpStatusCode statusCode;
            using (var response = DoRequest(request, out statusCode))
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

            Load(result, ret);

            while (ret.MoreItems)
            {
                var nextUri = new Uri(String.Concat(inventoryUri.ToString(), "&start_assetid=", ret.LastAssetId));
                request = (HttpWebRequest)HttpWebRequest.Create(nextUri);
                request.Method = "GET";
                request.Timeout = 10000;
                request.Accept = "application/json";
                request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Referer = referer;

                using (var response = DoRequest(request, out statusCode))
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

                Load(result, ret);
            }

            result.Sort(CardComparison);

            errorMessage = null;
            return result;
        }

        public static Inventory GetInventory(string idOrUrl, out string errorMessage)
        {
            var result = new Inventory();
            result.User = idOrUrl;
            var uri = GetProfileUri(idOrUrl, SteamUrlPattern.ApiGetInventory);
            if (uri == null)
            {
                errorMessage = "Invalid profile";
                return null;
            }

            result.Link = GetProfileUri(idOrUrl, SteamUrlPattern.Inventory).ToString();
            result.Cards = GetCards(uri, out errorMessage);

            return result;
        }

        public static SteamProfileResp GetProfile(Uri profileUri, out GetProfileError error)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(profileUri);
            request.Method = "GET";
            request.Timeout = 10000;
            request.Accept = "text/xml";
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Referer = "http://steamcommunity.com/";

            string rawXml;
            HttpStatusCode statusCode;
            using (var response = DoRequest(request, out statusCode))
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

        public static Uri GetProfileUri(string idOrUrl, SteamUrlPattern page)
        {
            long steamId = 0L;
            string customUrl = null;

            if (TryExtractSteamIdFromUrl(idOrUrl, out steamId))
                return GetProfileUri(steamId, customUrl, page);

            if (TryExtractCustomUrlFromUrl(idOrUrl, out customUrl))
                return GetProfileUri(steamId, customUrl, page);

            if (_steamidRe.IsMatch(idOrUrl))
                steamId = Int64.Parse(idOrUrl);
            else if (_userurlRe.IsMatch(idOrUrl))
                customUrl = idOrUrl;
            else
                return null;

            return GetProfileUri(steamId, customUrl, page);
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

        public static Uri GetProfileUri(this SteamProfile steamProfile, SteamUrlPattern page)
        {
            if (steamProfile == null)
                return null;

            return GetProfileUri(steamProfile.SteamId, steamProfile.CustomUrl, page);
        }

        private static void Load(List<Card> result, RgInventoryResp ret)
        {
            foreach (var dt in ret.RgInventory.Values)
            {
                var key = new RgInventoryResp.RgDescriptionKey(dt.ClassId, dt.InstanceId);
                var desc = ret.RgDescriptions[key];
                if (desc.Tradable && desc.Marketable)
                    result.Add(new Card(dt, desc));
            }
        }

        private static void Load(List<Card> result, ApiInventoryResp ret)
        {
            var dict = new Dictionary<CardEqualityKey, ApiInventoryResp.Description>();
            foreach (var dt in ret.Descriptions)
                dict.Add(new CardEqualityKey(dt.ClassId, dt.InstanceId), dt);
            foreach (var dt in ret.Assets)
            {
                var desc = dict[new CardEqualityKey(dt.ClassId, dt.InstanceId)];
                if (desc.Tradable && desc.Marketable)
                    result.Add(new Card(dt, desc));
            }
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

        private static HttpWebResponse DoRequest(HttpWebRequest request, out HttpStatusCode statusCode)
        {
            var delay = 1;
            Info.IsInProgress = true;
            try
            {
                while (true)
                {
                    var now = DateTime.Now;
                    if (_nextCall > now)
                    {
                        var diff = (int)(_nextCall - now).TotalMilliseconds + 1;
                        Thread.Sleep(diff);
                    }

                    try
                    {
                        Info.RequestCount += 1;
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
                Info.IsInProgress = false;
            }
        }

        private static bool TryExtractCustomUrlFromUrl(string idOrUrl, out string customUrl)
        {
            if (idOrUrl.StartsWith(SteamCommunityPageByCustomUrl))
            {
                var data = idOrUrl.Substring(SteamCommunityPageByCustomUrl.Length);
                int i;
                for (i = 0; i < data.Length; ++i)
                    if (!Char.IsLetterOrDigit(data[i]))
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
            if (idOrUrl.StartsWith(SteamCommunityPageBySteamId))
            {
                var data = idOrUrl.Substring(SteamCommunityPageBySteamId.Length);
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

        public class DebugInfo : INotifyPropertyChanged
        {
            private int _requestCount;

            public event PropertyChangedEventHandler PropertyChanged;
            private bool _isInProgress;

            public int RequestCount
            {
                get { return _requestCount; }
                set
                {
                    if (_requestCount != value)
                    {
                        _requestCount = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            public bool IsInProgress
            {
                get { return _isInProgress; }
                set
                {
                    if (_isInProgress != value)
                    {
                        _isInProgress = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}