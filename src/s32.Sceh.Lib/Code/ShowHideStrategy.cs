using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.DataModel;

namespace s32.Sceh.Code
{
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

        public class ShowDuplicatesStrategy : ShowHideStrategy
        {
            public override void ShowHideCards(SteamApp steamApp)
            {
                bool iHaveDuplicate = false, otherHaveDuplicate = false;

                foreach (var dt in steamApp.MyCards)
                {
                    dt.Hide = false;
                    if (dt.IsDuplicated && !dt.OtherHaveIt)
                        iHaveDuplicate = true;
                }
                foreach (var dt in steamApp.OtherCards)
                {
                    dt.Hide = false;
                    if (dt.IsDuplicated && !dt.OtherHaveIt)
                        otherHaveDuplicate = true;
                }

                steamApp.Hide = !(iHaveDuplicate && otherHaveDuplicate);
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
}
