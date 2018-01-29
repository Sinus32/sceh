using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using s32.Sceh.DataModel;
using s32.Sceh.UserNoteTags;

namespace s32.Sceh.WinApp.BBCodeWriters
{
    public class SelectedCardsWriter : IBBCodeWriter
    {
        public bool MyCards { get; set; }

        public bool OtherCards { get; set; }

        public bool CanWrite(List<SteamApp> steamApps)
        {
            if (steamApps == null)
                return false;

            if (MyCards)
            {
                if (OtherCards)
                    return steamApps.Any(steamApp => steamApp.MyIsSelected || steamApp.OtherIsSelected);
                else
                    return steamApps.Any(steamApp => steamApp.MyIsSelected);
            }
            else
            {
                if (OtherCards)
                    return steamApps.Any(steamApp => steamApp.OtherIsSelected);
                else
                    return false;
            }
        }

        public void Write(TextBox tbEditor, List<SteamApp> steamApps)
        {
            var sb = new StringBuilder();

            if (MyCards)
            {
                if (OtherCards)
                {
                    BuildCardsTags(sb, steamApps, steamApp => steamApp.MyIsSelected, steamApp => steamApp.MyCards);
                    sb.Append(" [->] ");
                    BuildCardsTags(sb, steamApps, steamApp => steamApp.OtherIsSelected, steamApp => steamApp.OtherCards);
                }
                else
                {
                    BuildCardsTags(sb, steamApps, steamApp => steamApp.MyIsSelected, steamApp => steamApp.MyCards);
                }
            }
            else
            {
                if (OtherCards)
                    BuildCardsTags(sb, steamApps, steamApp => steamApp.OtherIsSelected, steamApp => steamApp.OtherCards);
            }

            tbEditor.SelectedText = sb.Append(' ').ToString();
            tbEditor.CaretIndex = tbEditor.SelectionStart + tbEditor.SelectionLength;
        }

        private void BuildCardsTags(StringBuilder sb, List<SteamApp> steamApps, Func<SteamApp, bool> getIsSelected, Func<SteamApp, IEnumerable<Card>> getCards)
        {
            foreach (var app in steamApps)
            {
                if (!getIsSelected(app))
                    continue;

                if (sb.Length > 0)
                    sb.Append("; ");
                sb.Append(new SteamAppTag(app));

                int i = -1;
                foreach (var card in getCards(app))
                {
                    if (!card.IsSelected)
                        continue;

                    if (sb.Length > 0)
                        sb.Append(++i == 0 ? ": " : ", ");
                    sb.Append(new SteamCardTag(card));
                }
            }
        }
    }
}