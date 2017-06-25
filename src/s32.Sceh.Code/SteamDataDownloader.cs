using Newtonsoft.Json;
using s32.Sceh.Classes;
using System;
using System.Collections.Generic;
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
    public class SteamDataDownloader
    {
        private static readonly Regex _steamidRe = new Regex("^[0-9]{3,20}$", RegexOptions.None);
        private static readonly Regex _userurlRe = new Regex("^[0-9a-zA-Z_-]{3,20}$", RegexOptions.None);

        public static List<Card> GetCards(string url, out string errorMessage)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
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
                Thread.Sleep(100);

                request = (HttpWebRequest)HttpWebRequest.Create(String.Concat(url, "?start=", ret.MoreStart));
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
            Thread.Sleep(100);

            errorMessage = null;
            return result;
        }

        public static Inventory GetInventory(string profile, out string errorMessage)
        {
            string url;
            var result = new Inventory();
            result.User = profile;
            if (_steamidRe.IsMatch(profile))
            {
                url = String.Concat("http://steamcommunity.com/profiles/", profile, "/inventory/json/753/6");
                result.Link = String.Concat("http://steamcommunity.com/profiles/", profile);
            }
            else if (_userurlRe.IsMatch(profile))
            {
                url = String.Concat("http://steamcommunity.com/id/", profile, "/inventory/json/753/6");
                result.Link = String.Concat("http://steamcommunity.com/id/", profile);
            }
            else
            {
                errorMessage = "Invalid profile";
                return null;
            }

            result.Cards = GetCards(url, out errorMessage);
            return result;
        }

        public static SteamProfile GetProfile(string profile, out string errorMessage)
        {
            string url;
            if (_steamidRe.IsMatch(profile))
            {
                url = String.Concat("http://steamcommunity.com/profiles/", profile, "?xml=1");
            }
            else if (_userurlRe.IsMatch(profile))
            {
                url = String.Concat("http://steamcommunity.com/id/", profile, "?xml=1");
            }
            else
            {
                errorMessage = "Invalid profile";
                return null;
            }

            var request = (HttpWebRequest)HttpWebRequest.Create(url);
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