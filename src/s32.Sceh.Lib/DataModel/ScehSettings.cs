using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.SteamApi;

namespace s32.Sceh.DataModel
{
    public class ScehSettings
    {
        public string Country { get; set; }
        public SteamCurrency Currency { get; set; }
        public string Language { get; set; }
    }
}
