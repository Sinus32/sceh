using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using s32.Sceh.Classes;
using System.Threading;
using s32.Sceh.DataModel;

namespace s32.Sceh.Code
{
    public class TradeSuggestionsMaker
    {
        public static Inventory GetInventory(string idOrUrl, out string errorMessage)
        {
            var result = new Inventory();
            result.User = idOrUrl;

            var profileKey = SteamDataDownloader.GetProfileKey(idOrUrl);
            var cached = DataManager.GetSteamProfile(profileKey);
            if (cached == null)
            {
                var profileUri = SteamDataDownloader.GetProfileUri(profileKey, SteamUrlPattern.ApiGetProfile);
                try
                {
                    SteamDataDownloader.GetProfileError error;
                    var resp = SteamDataDownloader.GetProfile(profileUri, out error);
                    switch (error)
                    {
                        case SteamDataDownloader.GetProfileError.Success:
                            profileKey = DataManager.AddOrUpdateSteamProfile(resp);
                            break;

                        case SteamDataDownloader.GetProfileError.WrongProfile:
                            errorMessage = "WrongProfileIdOrUrl";
                            return null;

                        case SteamDataDownloader.GetProfileError.DeserializationError:
                            errorMessage = "ProfileDeserializationError";
                            return null;

                        default:
                            errorMessage = "UnknownErrorOccured";
                            return null;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = String.Format("ExceptionDuringDownloadingSteamProfile {0}", ex.Message);
                    return null;
                }
            }
            else
            {
                profileKey = cached;
            }

            result.Link = SteamDataDownloader.GetProfileUri(profileKey, SteamUrlPattern.Inventory).ToString();
            result.Cards = SteamDataDownloader.GetCardsA(profileKey, out errorMessage);

            return result;
        }

        public static TradeSuggestions Generate(string myProfile, string otherProfile, out string errorMessage)
        {
            var myInv = GetInventory(myProfile, out errorMessage);
            if (errorMessage != null)
                return null;

            var otherInv = GetInventory(otherProfile, out errorMessage);
            if (errorMessage != null)
                return null;

            var steamApps = Generate(myInv.Cards, otherInv.Cards, out errorMessage);
            var manager = new CardImageManager();

            foreach (var dt in steamApps)
            {
                var mySet = new HashSet<string>();
                var otherSet = new HashSet<string>();

                foreach (var card in dt.MyCards)
                {
                    card.IsDuplicated = !mySet.Add(card.MarketHashName);
                    card.ThumbnailUrl = manager.GetCardThumbnailUrl(card.IconUrl);
                }

                foreach (var card in dt.OtherCards)
                {
                    card.IsDuplicated = !otherSet.Add(card.MarketHashName);
                    card.ThumbnailUrl = manager.GetCardThumbnailUrl(card.IconUrl);
                }

                dt.Hide = mySet.Count == 0 || otherSet.Count == 0 || mySet.SetEquals(otherSet);
            }

            var result = new TradeSuggestions();
            result.MyInv = myInv;
            result.OtherInv = otherInv;
            result.SteamApps = steamApps;
            result.OriginalsUsed = manager.OryginalsUsed;
            result.ThumbnailsUsed = manager.ThumbnailsUsed;

            errorMessage = null;
            return result;
        }

        public static List<SteamApp> Generate(List<Card> myCards, List<Card> otherCards, out string errorMessage)
        {
            var current = new SteamApp(-1, null);
            var it = otherCards.GetEnumerator();
            var steamApps = new List<SteamApp>();
            bool hasOther = it.MoveNext();

            foreach (var card in myCards)
            {
                while (hasOther && it.Current.MarketFeeApp < card.MarketFeeApp)
                {
                    if (it.Current.MarketFeeApp != current.Id)
                    {
                        current = new SteamApp(it.Current.MarketFeeApp, it.Current.Type);
                        steamApps.Add(current);
                    }
                    current.OtherCards.Add(it.Current);
                    hasOther = it.MoveNext();
                }

                if (card.MarketFeeApp != current.Id)
                {
                    current = new SteamApp(card.MarketFeeApp, card.Type);
                    steamApps.Add(current);
                }

                current.MyCards.Add(card);
            }

            while (hasOther)
            {
                if (it.Current.MarketFeeApp != current.Id)
                {
                    current = new SteamApp(it.Current.MarketFeeApp, it.Current.Type);
                    steamApps.Add(current);
                }
                current.OtherCards.Add(it.Current);
                hasOther = it.MoveNext();
            }

            steamApps.Sort(SteamAppsComparison);

            errorMessage = null;
            return steamApps;
        }

        private static int SteamAppsComparison(SteamApp x, SteamApp y)
        {
            return String.Compare(x.Name, y.Name, true);
        }
    }
}