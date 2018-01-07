using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.DataModel;

namespace s32.Sceh.UserNoteTags
{
    public class SteamCardTag : IUserNoteTag
    {
        public const string TagName = "card";

        public SteamCardTag()
        { }

        public SteamCardTag(string marketHashName, string cardName)
        {
            MarketHashName = marketHashName;
            CardName = cardName;
        }

        public SteamCardTag(Card card)
        {
            if (card != null)
            {
                MarketHashName = card.MarketHashName;
                CardName = card.Name;
            }
        }

        public string CardName { get; set; }

        public string MarketHashName { get; set; }

        public string Name
        {
            get { return TagName; }
        }

        public string BuildSourceText()
        {
            return String.Concat("[card=", MarketHashName, ']', CardName, "[/card]");
        }

        public string GetFormatedText()
        {
            return CardName;
        }

        public override string ToString()
        {
            return BuildSourceText();
        }
    }
}