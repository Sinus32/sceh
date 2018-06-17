using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.DataModel;

namespace s32.Sceh.WinApp.Helpers
{
    public class SteamAppComparer : IComparer, IComparer<SteamApp>
    {
        private SteamAppSort _sortValue;

        public SteamAppComparer(SteamAppSort sortValue)
        {
            _sortValue = sortValue;
        }

        public int Compare(SteamApp x, SteamApp y)
        {
            int result;
            switch (_sortValue & SteamAppSort.FieldMask)
            {
                case SteamAppSort.ByName:
                    result = Compare(x, y, ByName);
                    break;

                case SteamAppSort.ByCardAvgPrice:
                    result = Compare(x, y, ByCardAvgPrice, BySceWorth, ByName);
                    break;

                case SteamAppSort.BySceWorth:
                    result = Compare(x, y, BySceWorth, ByCardAvgPrice, ByName);
                    break;

                case SteamAppSort.ByTotalUniqueCards:
                    result = Compare(x, y, ByTotalUniqueCards, ByCardsSurplus, ByName);
                    break;

                case SteamAppSort.ByCardsSurplus:
                    result = Compare(x, y, ByCardsSurplus, ByTotalUniqueCards, ByName);
                    break;

                case SteamAppSort.BySetCompletionRate:
                    result = Compare(x, y, BySetCompletionRate, ByCardsSurplus, ByName);
                    break;

                default:
                    result = Compare(x, y, ById);
                    break;
            }

            return (_sortValue & SteamAppSort.Descending) > 0 ? -result : result;
        }

        int IComparer.Compare(object x, object y)
        {
            return Compare((SteamApp)x, (SteamApp)y);
        }

        private int ByCardAvgPrice(SteamApp x, SteamApp y)
        {
            var result = x.CardAvg - y.CardAvg;

            if (result > Double.Epsilon)
                return 1;

            if (result < -Double.Epsilon)
                return 1;

            return 0;
        }

        private int ByCardsSurplus(SteamApp x, SteamApp y)
        {
            if (x.TotalUniqueCards > 0 && y.TotalUniqueCards > 0)
            {
                var a = x.MyCardsTotal << 16;
                a /= x.TotalUniqueCards.Value;
                var b = y.MyCardsTotal << 16;
                b /= y.TotalUniqueCards.Value;
                return a - b;
            }
            else
            {
                return x.MyCardsTotal - y.MyCardsTotal;
            }
        }

        private int ById(SteamApp x, SteamApp y)
        {
            return x.Id > y.Id ? 1 : -1;
        }

        private int ByName(SteamApp x, SteamApp y)
        {
            return String.Compare(x.Name, y.Name, StringComparison.CurrentCultureIgnoreCase);
        }

        private int BySceWorth(SteamApp x, SteamApp y)
        {
            return (x.SceWorth ?? 0) - (y.SceWorth ?? 0);
        }

        private int BySetCompletionRate(SteamApp x, SteamApp y)
        {
            if (x.TotalUniqueCards > 0 && y.TotalUniqueCards > 0)
            {
                var a = x.MyUniqueCards << 16;
                a /= x.TotalUniqueCards.Value;
                var b = y.MyUniqueCards << 16;
                b /= y.TotalUniqueCards.Value;
                return a - b;
            }
            else
            {
                return x.MyUniqueCards - y.MyUniqueCards;
            }
        }

        private int ByTotalUniqueCards(SteamApp x, SteamApp y)
        {
            return (x.TotalUniqueCards ?? 0) - (y.TotalUniqueCards ?? 0);
        }

        private int Compare(SteamApp x, SteamApp y, params Func<SteamApp, SteamApp, int>[] funcs)
        {
            int result = 0;
            for (int i = 0; i < funcs.Length; ++i)
                if ((result = funcs[i](x, y)) != 0)
                    break;
            return result;
        }
    }
}
