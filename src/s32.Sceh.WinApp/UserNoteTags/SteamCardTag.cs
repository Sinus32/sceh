using s32.Sceh.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.WinApp.UserNoteTags
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
                SetName(card.Name, card.MarketHashName.EndsWith("(trading card)", StringComparison.CurrentCultureIgnoreCase));
            }
        }

        public string MarketHashName { get; set; }

        public string Name { get; set; }

        public string BuildTag()
        {
            return String.Concat("[card=", MarketHashName, ']', Name, "[/card]");
        }

        public void SetName(string steamAppName, bool excludeBrackets)
        {
            if (String.IsNullOrEmpty(steamAppName))
            {
                Name = String.Empty;
                return;
            }

            if (excludeBrackets)
            {
                var pos = steamAppName.LastIndexOf('(');
                if (pos > 0)
                    Name = steamAppName.Remove(pos).Trim();
                else
                    Name = steamAppName;
            }
            else
            {
                Name = steamAppName;
            }
        }

        public override string ToString()
        {
            return BuildTag();
        }
    }
}