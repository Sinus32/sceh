using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using s32.Sceh.Models;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;

namespace s32.Sceh.Code
{
    public class TradeSuggestionsMaker
    {
        public static IndexViewModel Generate(IndexModel input, out string errorMessage)
        {
            var myInv = GetInventory(input.MyProfile, out errorMessage);
            if (errorMessage != null)
                return null;

            var otherInv = GetInventory(input.OtherProfile, out errorMessage);
            if (errorMessage != null)
                return null;

            var current = new SteamApp(-1, null);
            var it = otherInv.Cards.GetEnumerator();
            var steamApps = new List<SteamApp>();
            bool hasOther = it.MoveNext();

            foreach (var card in myInv.Cards)
            {
                while (hasOther && it.Current.AppDataAppId < card.AppDataAppId)
                {
                    if (it.Current.AppDataAppId != current.Id)
                    {
                        current = new SteamApp(it.Current.AppDataAppId, it.Current.DescriptionItem.Type);
                        steamApps.Add(current);
                    }
                    current.OtherCards.Add(it.Current);
                    hasOther = it.MoveNext();
                }

                if (card.AppDataAppId != current.Id)
                {
                    current = new SteamApp(card.AppDataAppId, card.DescriptionItem.Type);
                    steamApps.Add(current);
                }

                current.MyCards.Add(card);
            }

            while (hasOther)
            {
                if (it.Current.AppDataAppId != current.Id)
                {
                    current = new SteamApp(it.Current.AppDataAppId, it.Current.DescriptionItem.Type);
                    steamApps.Add(current);
                }
                current.OtherCards.Add(it.Current);
                hasOther = it.MoveNext();
            }

            Prepare(steamApps);

            var result = new IndexViewModel(input);
            result.MyInv = myInv;
            result.OtherInv = otherInv;
            result.SteamApps = steamApps;

            return result;
        }

        private static void Prepare(List<SteamApp> steamApps)
        {
            foreach (var dt in steamApps)
            {
                dt.Skip = dt.MyCards.Count == 0 || dt.OtherCards.Count == 0;

                if (dt.Skip)
                    continue;

                dt.MySet = new HashSet<int>();
                dt.OtherSet = new HashSet<int>();

                foreach (var card in dt.MyCards)
                    card.IsDuplicated = !dt.MySet.Add(card.AppDataItemType);

                foreach (var card in dt.OtherCards)
                    card.IsDuplicated = !dt.OtherSet.Add(card.AppDataItemType);

                dt.Skip = dt.MySet.SetEquals(dt.OtherSet);
            }
        }

        private static readonly Regex _steamidRe = new Regex("^[0-9]{3,20}$", RegexOptions.None);
        private static readonly Regex _userurlRe = new Regex("^[0-9a-zA-Z_]{3,20}$", RegexOptions.None);

        private static Inventory GetInventory(string profile, out string errorMessage)
        {
            string url;
            var result = new Inventory();
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

            result.Load(ret);

            while (ret.More)
            {
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

                result.Load(ret);
            }

            result.Prepare();

            errorMessage = null;
            return result;
        }
    }
}