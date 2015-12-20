using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace s32.Sceh.Code
{
    public class Inventory
    {
        public Inventory()
        {
            Cards = new List<Card>();
        }

        public List<Card> Cards { get; set; }

        public string Link { get; set; }

        public void Load(GetInventoryResponse ret)
        {
            foreach (var dt in ret.RgInventory.Values)
            {
                var card = new Card();
                card.InventoryItem = dt;
                var key = new GetInventoryResponse.RgDescriptionKey(dt.ClassId, dt.InstanceId);
                card.DescriptionItem = ret.RgDescriptions[key];
                if (card.DescriptionItem.Tradable && card.DescriptionItem.Marketable)
                    Cards.Add(card);
            }
        }

        public void Prepare()
        {
            Cards.Sort(CardComparison);
        }

        private int CardComparison(Card x, Card y)
        {
            int ret = x.AppDataAppId.CompareTo(y.AppDataAppId);
            if (ret == 0)
            {
                ret = x.AppDataItemType.CompareTo(y.AppDataItemType);
                if (ret == 0)
                    x.Id.CompareTo(y.Id);
            }
            return ret;
        }
    }
}