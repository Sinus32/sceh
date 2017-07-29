using Newtonsoft.Json;
using s32.Sceh.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace s32.Sceh.Code
{
    public static class SteamDataDownloader
    {
        private static readonly Regex _steamidRe = new Regex("^[0-9]{3,18}$", RegexOptions.None);
        private static readonly Regex _userurlRe = new Regex("^[0-9a-zA-Z_-]{3,20}$", RegexOptions.None);
        private static DateTime _prevCall = DateTime.MinValue;

        public static List<Card> GetCards(SteamUser steamUser, out string errorMessage)
        {
            var url = GetProfileUri(steamUser, ProfilePage.API_GET_INVENTORY);

            if (url == null)
            {
                errorMessage = "Invalid profile data";
                return null;
            }

            return GetCards(url, out errorMessage);
        }

        public static List<Card> GetCards(Uri inventoryUri, out string errorMessage)
        {
            Delay();
            var request = (HttpWebRequest)HttpWebRequest.Create(inventoryUri);
            request.Method = "GET";
            request.Timeout = 10000;
            request.Accept = "application/json";
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Referer = "http://steamcommunity.com/";

            string rawJson;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
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
            var ret = JsonConvert.DeserializeObject<GetInventoryResponse>(rawJson, jss);

            if (!ret.Success)
            {
                errorMessage = ret.Error ?? "Wrong query";
                return null;
            }

            var result = new List<Card>();

            Load(result, ret);

            while (ret.More)
            {
                Delay();

                var nextUri = new Uri(String.Concat(inventoryUri.ToString(), "?start=", ret.MoreStart));
                request = (HttpWebRequest)HttpWebRequest.Create(nextUri);
                request.Method = "GET";
                request.Timeout = 10000;
                request.Accept = "application/json";
                request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Referer = "http://steamcommunity.com/";

                using (var response = (HttpWebResponse)request.GetResponse())
                {
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

                ret = JsonConvert.DeserializeObject<GetInventoryResponse>(rawJson, jss);

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

        private static void Delay()
        {
            var now = DateTime.Now;
            if (_prevCall == DateTime.MinValue)
            {
                _prevCall = now;
                return;
            }

            var nextCall = _prevCall.AddMilliseconds(200);

            if (nextCall > now)
            {
                var diff = (int)(now - nextCall).TotalMilliseconds + 1;
                Thread.Sleep(diff);
                _prevCall = DateTime.Now;
            }
            else
            {
                _prevCall = now;
            }
        }

        public static Inventory GetInventory(string idOrUrl, out string errorMessage)
        {
            var result = new Inventory();
            result.User = idOrUrl;
            var uri = GetProfileUri(idOrUrl, ProfilePage.API_GET_INVENTORY);
            if (uri == null)
            {
                errorMessage = "Invalid profile";
                return null;
            }

            result.Link = GetProfileUri(idOrUrl).ToString();
            result.Cards = GetCards(uri, out errorMessage);
            return result;
        }

        public static SteamProfile GetProfile(Uri profileUri, out string errorMessage)
        {
            Delay();

            var request = (HttpWebRequest)HttpWebRequest.Create(profileUri);
            request.Method = "GET";
            request.Timeout = 10000;
            request.Accept = "text/xml";
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Referer = "http://steamcommunity.com/";

            string rawXml;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (!response.ContentType.StartsWith("text/xml"))
                {
                    errorMessage = "Wrong profile";
                    return null;
                }

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    rawXml = reader.ReadToEnd();
                }

                if (!rawXml.Contains("<profile>"))
                {
                    errorMessage = "Wrong profile";
                    return null;
                }
            }

            var ser = new XmlSerializer(typeof(SteamProfile));

            SteamProfile result = null;
            try
            {
                using (var reader = new StringReader(rawXml))
                    result = (SteamProfile)ser.Deserialize(reader);
            }
            catch (InvalidOperationException)
            {
                errorMessage = "Cannot read user profile";
                return null;
            }

            errorMessage = null;
            return result;
        }

        public static Uri GetProfileUri(string idOrUrl, ProfilePage page = null)
        {
            long steamId = 0L;
            string customUrl = null;

            if (_steamidRe.IsMatch(idOrUrl))
                steamId = Int64.Parse(idOrUrl);
            else if (_userurlRe.IsMatch(idOrUrl))
                customUrl = idOrUrl;
            else
                return null;

            return GetProfileUri(steamId, customUrl, page);
        }

        public static Uri GetProfileUri(long steamId, string customUrl, ProfilePage page = null)
        {
            string result;
            if (!String.IsNullOrEmpty(customUrl))
                result = String.Concat("http://steamcommunity.com/id/", customUrl);
            else if (steamId > 0L)
                result = String.Concat("http://steamcommunity.com/profiles/", steamId);
            else
                return null;

            return page == null
                ? new Uri(result)
                : new Uri(String.Concat(result, page.PageUrl));
        }

        public static Uri GetProfileUri(this SteamUser steamUser, ProfilePage page = null)
        {
            if (steamUser == null || steamUser.Profile == null)
                return null;

            return GetProfileUri(steamUser.Profile.SteamId, steamUser.Profile.CustomURL, page);
        }

        public static void Load(List<Card> result, GetInventoryResponse ret)
        {
            foreach (var dt in ret.RgInventory.Values)
            {
                var card = new Card();
                card.InventoryItem = dt;
                var key = new GetInventoryResponse.RgDescriptionKey(dt.ClassId, dt.InstanceId);
                card.DescriptionItem = ret.RgDescriptions[key];
                if (card.DescriptionItem.Tradable && card.DescriptionItem.Marketable)
                    result.Add(card);
            }
        }

        private static int CardComparison(Card x, Card y)
        {
            int ret = x.MarketFeeApp.CompareTo(y.MarketFeeApp);
            if (ret == 0)
            {
                ret = String.Compare(x.MarketHashName, y.MarketHashName);
                if (ret == 0)
                    ret = x.Id.CompareTo(y.Id);
            }
            return ret;
        }
    }
}