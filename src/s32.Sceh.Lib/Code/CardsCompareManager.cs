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
        { }

        public CardsCompareManager(IReadOnlyCollection<Card> myCards, IReadOnlyCollection<Card> otherCards)
        {
            Fill(myCards, otherCards);
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
            var dict = new Dictionary<long, SteamApp>();

            foreach (var card in _myCards)
            {
                if (current.Id != card.MarketFeeApp && !dict.TryGetValue(card.MarketFeeApp, out current))
                {
                    current = new SteamApp(card.MarketFeeApp, card.Type);
                    dict.Add(current.Id, current);
                }

                current.MyCards.Add(card);
            }

            foreach (var card in _otherCards)
            {
                if (current.Id != card.MarketFeeApp && !dict.TryGetValue(card.MarketFeeApp, out current))
                {
                    current = new SteamApp(card.MarketFeeApp, card.Type);
                    dict.Add(current.Id, current);
                }

                current.OtherCards.Add(card);
            }

            _steamApps = new List<SteamApp>(dict.Count);
            _steamApps.AddRange(dict.Values);

            //var mySet = new HashSet<Card>(CardEqualityComparer.Instance);
            //var otherSet = new HashSet<Card>(CardEqualityComparer.Instance);
        }

        private class CardEqualityComparer : IEqualityComparer<Card>
        {
            public static readonly CardEqualityComparer Instance = new CardEqualityComparer();

            public bool Equals(Card x, Card y)
            {
                return x.ClassId == y.ClassId && x.InstanceId == y.InstanceId;
            }

            public int GetHashCode(Card obj)
            {
                return obj.ClassId.GetHashCode() ^ obj.InstanceId.GetHashCode();
            }
        }
    }
}