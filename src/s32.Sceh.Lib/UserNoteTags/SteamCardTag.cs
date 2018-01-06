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
        public SteamCardTag()
        { }

        public SteamCardTag(Card card)
        {
            if (card != null)
            {
                MarketHashName = card.MarketHashName;
                Name = card.Name;
            }
        }

        public string MarketHashName { get; set; }

        public string Name { get; set; }

        public string BuildTag()
        {
            return String.Concat("[card=", MarketHashName, ']', Name, "[/card]");
        }

        public override string ToString()
        {
            return BuildTag();
        }
    }
}