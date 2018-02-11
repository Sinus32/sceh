using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.SteamApi
{
    public class SteamCurrency
    {
        private readonly string _isoCode, _currencySymbol;
        private readonly int _steamId;

        public SteamCurrency(string isoCode, int steamId, string currencySymbol)
        {
            _isoCode = isoCode;
            _steamId = steamId;
            _currencySymbol = currencySymbol;
        }

        public string CurrencySymbol
        {
            get { return _currencySymbol; }
        }

        public string IsoCode
        {
            get { return _isoCode; }
        }

        public int SteamId
        {
            get { return _steamId; }
        }

        public override string ToString()
        {
            return String.Format("{0}: {1} ({2})", _isoCode, _steamId, _currencySymbol);
        }
    }
}