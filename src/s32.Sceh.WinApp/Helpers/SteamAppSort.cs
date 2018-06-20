using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.WinApp.Helpers
{
    [Flags]
    public enum SteamAppSort
    {
        None = 0,

        Descending = 0x01,

        ByName = 0x02,

        ByCardAvgPrice = 0x04,

        BySceWorth = 0x06,

        ByTotalUniqueCards = 0x08,

        ByCardsSurplus = 0x0A,

        BySetCompletionRate = 0x0C,
        
        FieldMask = 0xFE,
    }
}
