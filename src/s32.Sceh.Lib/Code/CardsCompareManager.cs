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
        private List<Card> _myCards;
        private List<Card> _otherCards;
        private List<SteamApp> _steamApps;

        public CardsCompareManager()
        {
            _myCards = new List<Card>();
            _otherCards = new List<Card>();
            _steamApps = new List<SteamApp>();
        }

        public CardsCompareManager(IReadOnlyCollection<Card> myCards, IReadOnlyCollection<Card> otherCards)
        {
            Fill(myCards, otherCards);
        }

        public IReadOnlyList<SteamApp> SteamApps
        {
            get { return _steamApps; }
        }

        public void Fill(IReadOnlyCollection<Card> myCards, IReadOnlyCollection<Card> otherCards)
        {
            if (myCards == null)
            {
                _myCards = new List<Card>();
            }
            else
            {
                _myCards = new List<Card>(myCards.Count);
                _myCards.AddRange(myCards);
            }

            if (otherCards == null)
            {
                _otherCards = new List<Card>();
            }
            else
            {
                _otherCards = new List<Card>(otherCards.Count);
                _otherCards.AddRange(otherCards);
            }

            var current = new SteamApp(-1, null);
            _steamApps = new List<SteamApp>();
            var it = _otherCards.GetEnumerator();
            bool hasOther = it.MoveNext();

            var mySet = new HashSet<CardEqualityKey>();
            var otherSet = new HashSet<CardEqualityKey>();

            foreach (var card in _myCards)
            {
                while (hasOther && it.Current.MarketFeeApp < card.MarketFeeApp)
                {
                    if (it.Current.MarketFeeApp != current.Id)
                    {
                        current = new SteamApp(it.Current.MarketFeeApp, it.Current.GetMarketFeeAppName());
                        _steamApps.Add(current);
                    }
                    current.OtherCards.Add(it.Current);
                    it.Current.SteamAppOtherSide = new WeakReference<SteamApp>(current);
                    it.Current.IsDuplicated = !otherSet.Add(it.Current);
                    hasOther = it.MoveNext();
                }

                if (card.MarketFeeApp != current.Id)
                {
                    current = new SteamApp(card.MarketFeeApp, card.GetMarketFeeAppName());
                    _steamApps.Add(current);
                }

                current.MyCards.Add(card);
                card.SteamAppMySide = new WeakReference<SteamApp>(current);
                card.IsDuplicated = !mySet.Add(card);
            }

            while (hasOther)
            {
                if (it.Current.MarketFeeApp != current.Id)
                {
                    current = new SteamApp(it.Current.MarketFeeApp, it.Current.GetMarketFeeAppName());
                    _steamApps.Add(current);
                }
                current.OtherCards.Add(it.Current);
                it.Current.SteamAppOtherSide = new WeakReference<SteamApp>(current);
                it.Current.IsDuplicated = !otherSet.Add(it.Current);
                hasOther = it.MoveNext();
            }

            foreach (var card in _myCards)
            {
                card.OtherHaveIt = otherSet.Contains(card);
            }

            foreach (var card in _otherCards)
            {
                card.OtherHaveIt = mySet.Contains(card);
            }

            _steamApps.Sort(SteamAppsComparison);
        }

        public void ShowHideCards(ShowHideStrategy strategy)
        {
            foreach (var dt in _steamApps)
                strategy.ShowHideCards(dt);
        }

        private static int SteamAppsComparison(SteamApp x, SteamApp y)
        {
            return String.Compare(x.Name, y.Name, true);
        }

        #region Show card strategies

        public static readonly ShowHideStrategy ShowAllStrategy = new ShowHideStrategy.ShowAllStrategy();
        public static readonly ShowHideStrategy ShowMyCardsStrategy = new ShowHideStrategy.ShowMyCardsStrategy();
        public static readonly ShowHideStrategy ShowOnlySelectedStrategy = new ShowHideStrategy.ShowOnlySelectedStrategy();
        public static readonly ShowHideStrategy ShowOtherCardsStrategy = new ShowHideStrategy.ShowOtherCardsStrategy();
        public static readonly ShowHideStrategy ShowSelectedAppStrategy = new ShowHideStrategy.ShowSelectedAppStrategy();
        public static readonly ShowHideStrategy ShowTradeSugestionsStrategy = new ShowHideStrategy.ShowTradeSugestionsStrategy();

        public abstract class ShowHideStrategy
        {
            public abstract void ShowHideCards(SteamApp steamApp);

            public class ShowAllStrategy : ShowHideStrategy
            {
                public override void ShowHideCards(SteamApp steamApp)
                {
                    foreach (var dt in steamApp.MyCards)
                        dt.Hide = false;
                    foreach (var dt in steamApp.OtherCards)
                        dt.Hide = false;
                    steamApp.Hide = false;
                }
            }

            public class ShowMyCardsStrategy : ShowHideStrategy
            {
                public override void ShowHideCards(SteamApp steamApp)
                {
                    foreach (var dt in steamApp.MyCards)
                        dt.Hide = false;
                    foreach (var dt in steamApp.OtherCards)
                        dt.Hide = false;
                    steamApp.Hide = steamApp.MyCards.Count == 0;
                }
            }

            public class ShowOnlySelectedStrategy : ShowHideStrategy
            {
                public override void ShowHideCards(SteamApp steamApp)
                {
                    var shouldHide = true;
                    foreach (var dt in steamApp.MyCards)
                    {
                        if (dt.IsSelected)
                            dt.Hide = shouldHide = false;
                        else
                            dt.Hide = true;
                    }
                    foreach (var dt in steamApp.OtherCards)
                    {
                        if (dt.IsSelected)
                            dt.Hide = shouldHide = false;
                        else
                            dt.Hide = true;
                    }
                    steamApp.Hide = shouldHide;
                }
            }

            public class ShowOtherCardsStrategy : ShowHideStrategy
            {
                public override void ShowHideCards(SteamApp steamApp)
                {
                    foreach (var dt in steamApp.MyCards)
                        dt.Hide = false;
                    foreach (var dt in steamApp.OtherCards)
                        dt.Hide = false;
                    steamApp.Hide = steamApp.OtherCards.Count == 0;
                }
            }

            public class ShowSelectedAppStrategy : ShowHideStrategy
            {
                public override void ShowHideCards(SteamApp steamApp)
                {
                    var shouldHide = true;
                    foreach (var dt in steamApp.MyCards)
                    {
                        if (dt.IsSelected)
                            shouldHide = false;
                        dt.Hide = false;
                    }
                    foreach (var dt in steamApp.OtherCards)
                    {
                        if (dt.IsSelected)
                            shouldHide = false;
                        dt.Hide = false;
                    }
                    steamApp.Hide = shouldHide;
                }
            }

            public class ShowTradeSugestionsStrategy : ShowHideStrategy
            {
                public override void ShowHideCards(SteamApp steamApp)
                {
                    foreach (var dt in steamApp.MyCards)
                        dt.Hide = false;
                    foreach (var dt in steamApp.OtherCards)
                        dt.Hide = false;
                    steamApp.Hide = steamApp.MyCards.Count == 0 || steamApp.OtherCards.Count == 0;
                }
            }
        }

        #endregion Show card strategies
    }
}