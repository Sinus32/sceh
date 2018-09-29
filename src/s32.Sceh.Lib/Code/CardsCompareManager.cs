using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.DataModel;

namespace s32.Sceh.Code
{
    public class CardsCompareManager
    {
        public static readonly ShowHideStrategy ShowAllStrategy = new ShowHideStrategy.ShowAllStrategy();
        public static readonly ShowHideStrategy ShowDuplicatesStrategy = new ShowHideStrategy.ShowDuplicatesStrategy();
        public static readonly ShowHideStrategy ShowMyCardsStrategy = new ShowHideStrategy.ShowMyCardsStrategy();
        public static readonly ShowHideStrategy ShowOnlySelectedStrategy = new ShowHideStrategy.ShowOnlySelectedStrategy();
        public static readonly ShowHideStrategy ShowOtherCardsStrategy = new ShowHideStrategy.ShowOtherCardsStrategy();
        public static readonly ShowHideStrategy ShowSelectedAppStrategy = new ShowHideStrategy.ShowSelectedAppStrategy();
        public static readonly ShowHideStrategy ShowTradeSugestionsStrategy = new ShowHideStrategy.ShowTradeSugestionsStrategy();

        private List<SteamApp> _steamApps;

        public CardsCompareManager()
        {
            _steamApps = new List<SteamApp>();
        }

        public IReadOnlyList<SteamApp> SteamApps
        {
            get { return _steamApps; }
        }

        public void Fill(UserInventory myInventory, UserInventory otherInventory)
        {
            if (myInventory == null)
                throw new ArgumentNullException(nameof(myInventory));
            if (otherInventory == null)
                throw new ArgumentNullException(nameof(otherInventory));

            var myCards = myInventory.Cards ?? new List<CardData>();
            var otherCards = otherInventory.Cards ?? new List<CardData>();

            _steamApps = new List<SteamApp>();
            var current = new SteamApp(-1, null);
            var myDuplicates = new Dictionary<CardEqualityKey, Card>();
            var otherDuplicates = new Dictionary<CardEqualityKey, Card>();
            using (var it = otherCards.GetEnumerator())
            {
                bool hasOther = it.MoveNext();

                foreach (var card in myCards)
                {
                    while (hasOther && it.Current.MarketFeeApp < card.MarketFeeApp)
                    {
                        if (it.Current.MarketFeeApp != current.Id)
                        {
                            current = new SteamApp(it.Current.MarketFeeApp, it.Current.GetMarketFeeAppName());
                            _steamApps.Add(current);
                        }
                        AddCard(otherInventory.Profile, it.Current, current, current.OtherCards, otherDuplicates);
                        hasOther = it.MoveNext();
                    }

                    if (card.MarketFeeApp != current.Id)
                    {
                        current = new SteamApp(card.MarketFeeApp, card.GetMarketFeeAppName());
                        _steamApps.Add(current);
                    }
                    AddCard(myInventory.Profile, card, current, current.MyCards, myDuplicates);
                }

                while (hasOther)
                {
                    if (it.Current.MarketFeeApp != current.Id)
                    {
                        current = new SteamApp(it.Current.MarketFeeApp, it.Current.GetMarketFeeAppName());
                        _steamApps.Add(current);
                    }
                    AddCard(otherInventory.Profile, it.Current, current, current.OtherCards, otherDuplicates);
                    hasOther = it.MoveNext();
                }
            }

            foreach (var app in _steamApps)
            {
                SceAppData sceData = DataManager.GetSceAppData(app.Id);
                if (sceData != null)
                {
                    app.SceState = sceData.State;
                    app.Marketable = sceData.Marketable;
                    app.TotalUniqueCards = sceData.TotalCards;
                    app.SceWorth = sceData.Worth;
                }

                StAppData stData = DataManager.GetStAppData(app.Id);
                if (stData != null)
                {
                    app.SetPrice = stData.SetPrice;
                    app.BoosterAvg = stData.BoosterAvg;
                    app.CardAvg = stData.CardAvg;
                }

                foreach (var card in app.MyCards)
                {
                    card.IsSelected = false;
                    card.OtherHaveIt = otherDuplicates.ContainsKey(card.Key);

                    if (card.ItemClass != ItemClass.TradingCard || card.IsFoilCard == true)
                        continue;

                    if (card.IsDuplicated)
                    {
                        app.MyCardsTotal += card.DuplicateCount;
                    }
                    else
                    {
                        app.MyUniqueCards += 1;
                        app.MyCardsTotal += 1;
                    }
                }

                foreach (var card in app.OtherCards)
                {
                    card.IsSelected = false;
                    card.OtherHaveIt = myDuplicates.ContainsKey(card.Key);

                    if (card.ItemClass != ItemClass.TradingCard || card.IsFoilCard == true)
                        continue;

                    if (card.IsDuplicated)
                    {
                        app.OtherCardsTotal += card.DuplicateCount;
                    }
                    else
                    {
                        app.OtherUniqueCards += 1;
                        app.OtherCardsTotal += 1;
                    }
                }
            }

            int steamAppsComparison(SteamApp x, SteamApp y) => String.Compare(x.Name, y.Name, true);

            _steamApps.Sort(steamAppsComparison);
        }

        public void ShowHideCards(ShowHideStrategy strategy)
        {
            foreach (var dt in _steamApps)
                strategy.ShowHideCards(dt);
        }

        private static void AddCard(SteamProfileKey profile, CardData cardData, SteamApp steamApp, IList<Card> cardList, Dictionary<CardEqualityKey, Card> duplicateDict)
        {
            var key = cardData.Key;
            Card duplicate;

            if (duplicateDict.TryGetValue(key, out duplicate))
            {
                if (duplicate == null)
                {
                    var card = new Card(profile, cardData, steamApp);
                    cardList.Add(card);
                    card.IsDuplicated = true;
                    card.DuplicateCount = 1;
                    duplicateDict[key] = card;
                }
                else
                {
                    duplicate.DuplicateCount += 1;
                }
            }
            else
            {
                var card = new Card(profile, cardData, steamApp);
                cardList.Add(card);
                card.IsDuplicated = false;
                duplicateDict.Add(key, null);
            }
        }
    }
}
