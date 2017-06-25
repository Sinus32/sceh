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

namespace s32.Sceh.Code
{
    public class TradeSuggestionsMaker
    {
        public static TradeSuggestions Generate(string myProfile, string otherProfile, out string errorMessage)
        {
            var myInv = InventoryDownloader.GetInventory(myProfile, out errorMessage);
            if (errorMessage != null)
                return null;

            var otherInv = InventoryDownloader.GetInventory(otherProfile, out errorMessage);
            if (errorMessage != null)
                return null;

            var current = new SteamApp(-1, null);
            var it = otherInv.Cards.GetEnumerator();
            var steamApps = new List<SteamApp>();
            bool hasOther = it.MoveNext();

            foreach (var card in myInv.Cards)
            {
                while (hasOther && it.Current.MarketFeeApp < card.MarketFeeApp)
                {
                    if (it.Current.MarketFeeApp != current.Id)
                    {
                        current = new SteamApp(it.Current.MarketFeeApp, it.Current.DescriptionItem.Type);
                        steamApps.Add(current);
                    }
                    current.OtherCards.Add(it.Current);
                    hasOther = it.MoveNext();
                }

                if (card.MarketFeeApp != current.Id)
                {
                    current = new SteamApp(card.MarketFeeApp, card.DescriptionItem.Type);
                    steamApps.Add(current);
                }

                current.MyCards.Add(card);
            }

            while (hasOther)
            {
                if (it.Current.MarketFeeApp != current.Id)
                {
                    current = new SteamApp(it.Current.MarketFeeApp, it.Current.DescriptionItem.Type);
                    steamApps.Add(current);
                }
                current.OtherCards.Add(it.Current);
                hasOther = it.MoveNext();
            }

            var manager = new CardImageManager();

            foreach (var dt in steamApps)
            {
                foreach (var card in dt.MyCards)
                {
                    card.IsDuplicated = !dt.MySet.Add(card.MarketHashName);
                    card.ThumbnailUrl = manager.GetCardThumbnailUrl(card.IconUrl);
                }

                foreach (var card in dt.OtherCards)
                {
                    card.IsDuplicated = !dt.OtherSet.Add(card.MarketHashName);
                    card.ThumbnailUrl = manager.GetCardThumbnailUrl(card.IconUrl);
                }

                dt.Hide = dt.MySet.Count == 0 || dt.OtherSet.Count == 0 || dt.MySet.SetEquals(dt.OtherSet);
            }

            steamApps.Sort(SteamAppsComparison);

            var result = new TradeSuggestions();
            result.MyInv = myInv;
            result.OtherInv = otherInv;
            result.SteamApps = steamApps;
            result.OriginalsUsed = manager.OryginalsUsed;
            result.ThumbnailsUsed = manager.ThumbnailsUsed;

            return result;
        }

        private static int SteamAppsComparison(SteamApp x, SteamApp y)
        {
            return String.Compare(x.Name, y.Name, true);
        }
    }
}