using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Classes
{
    public class TradeSuggestions
    {
        public Inventory MyInv { get; set; }

        public Inventory OtherInv { get; set; }

        public List<SteamApp> SteamApps { get; set; }

        public int OriginalsUsed { get; set; }

        public int ThumbnailsUsed { get; set; }
    }
}
