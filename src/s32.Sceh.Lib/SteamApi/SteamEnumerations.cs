using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.SteamApi
{
    public static class SteamEnumerations
    {
        public const string DefaultCountry = "US";
        public const int DefaultCurrencyId = 1;
        public const string DefaultLanguage = "english";

        private static readonly string[] _countries;
        private static readonly SteamCurrency[] _currencies;
        private static readonly string[] _languages;

        static SteamEnumerations()
        {
            _languages = new string[]
            {
                "brazilian",
                "bulgarian",
                "czech",
                "danish",
                "dutch",
                "english",
                "finnish",
                "french",
                "german",
                "greek",
                "hungarian",
                "italian",
                "japanese",
                "korean",
                "norwegian",
                "polish",
                "portuguese",
                "romanian",
                "russian",
                "schinese",
                "spanish",
                "swedish",
                "tchinese",
                "thai",
                "turkish",
                "ukrainian",
            };

            _currencies = new SteamCurrency[]
            {
                new SteamCurrency("USD", 1, "$"),
                new SteamCurrency("GBP", 2, "\x00a3"),
                new SteamCurrency("EUR", 3, "\x20ac"),
                new SteamCurrency("CHF", 4, "CHF"),
                new SteamCurrency("RUB", 5, "p\x0443\x0431."),
                new SteamCurrency("PLN", 6, "z\x0142"),
                new SteamCurrency("BRL", 7, "R$"),
                new SteamCurrency("JPY", 8, "\x00a5"),
                new SteamCurrency("NOK", 9, "kr"),
                new SteamCurrency("IDR", 10, "Rp"),
                new SteamCurrency("MYR", 11, "RM"),
                new SteamCurrency("PHP", 12, "P"),
                new SteamCurrency("SGD", 13, "S$"),
                new SteamCurrency("THB", 14, "\x0e3f"),
                new SteamCurrency("VND", 15, "\x20ab"),
                new SteamCurrency("KRW", 16, "\x20a9"),
                new SteamCurrency("TRY", 17, "TL"),
                new SteamCurrency("UAH", 18, "\x20b4"),
                new SteamCurrency("MXN", 19, "Mex$"),
                new SteamCurrency("CAD", 20, "CDN$"),
                new SteamCurrency("AUD", 21, "A$"),
                new SteamCurrency("NZD", 22, "NZ$"),
                new SteamCurrency("CNY", 23, "\x00a5"),
                new SteamCurrency("INR", 24, "\x20b9"),
                new SteamCurrency("CLP", 25, "CLP$"),
                new SteamCurrency("PEN", 26, "S/."),
                new SteamCurrency("COP", 27, "COL$"),
                new SteamCurrency("ZAR", 28, "R"),
                new SteamCurrency("HKD", 29, "HK$"),
                new SteamCurrency("TWD", 30, "NT$"),
                new SteamCurrency("SAR", 31, "SR"),
                new SteamCurrency("AED", 32, "AED"),
                new SteamCurrency("ARS", 34, "ARS$"),
                new SteamCurrency("ILS", 35, "\x20aa"),
                new SteamCurrency("BYN", 36, "Br"),
                new SteamCurrency("KZT", 37, "\x20b8"),
                new SteamCurrency("KWD", 38, "KD"),
                new SteamCurrency("QAR", 39, "QR"),
                new SteamCurrency("CRC", 40, "\x20a1"),
                new SteamCurrency("UYU", 41, "$U"),
            };

            _countries = new string[]
            {
                "AD", // Andorra
                "AE", // United Arab Emirates
                "AF", // Afghanistan
                "AG", // Antigua and Barbuda
                "AI", // Anguilla
                "AL", // Albania
                "AM", // Armenia
                "AO", // Angola
                "AQ", // Antarctica
                "AR", // Argentina
                "AS", // American Samoa
                "AT", // Austria
                "AU", // Australia
                "AW", // Aruba
                "AX", // Åland Islands
                "AZ", // Azerbaijan
                "BA", // Bosnia and Herzegovina
                "BB", // Barbados
                "BD", // Bangladesh
                "BE", // Belgium
                "BF", // Burkina Faso
                "BG", // Bulgaria
                "BH", // Bahrain
                "BI", // Burundi
                "BJ", // Benin
                "BL", // Saint Barthélemy
                "BM", // Bermuda
                "BN", // Brunei Darussalam
                "BO", // Bolivia, Plurinational State of
                "BQ", // Bonaire, Sint Eustatius and Saba
                "BR", // Brazil
                "BS", // Bahamas
                "BT", // Bhutan
                "BV", // Bouvet Island
                "BW", // Botswana
                "BY", // Belarus
                "BZ", // Belize
                "CA", // Canada
                "CC", // Cocos (Keeling) Islands
                "CD", // Congo, the Democratic Republic of the
                "CF", // Central African Republic
                "CG", // Congo
                "CH", // Switzerland
                "CI", // Côte d'Ivoire
                "CK", // Cook Islands
                "CL", // Chile
                "CM", // Cameroon
                "CN", // China
                "CO", // Colombia
                "CR", // Costa Rica
                "CU", // Cuba
                "CV", // Cabo Verde
                "CW", // Curaçao
                "CX", // Christmas Island
                "CY", // Cyprus
                "CZ", // Czechia
                "DE", // Germany
                "DJ", // Djibouti
                "DK", // Denmark
                "DM", // Dominica
                "DO", // Dominican Republic
                "DZ", // Algeria
                "EC", // Ecuador
                "EE", // Estonia
                "EG", // Egypt
                "EH", // Western Sahara
                "ER", // Eritrea
                "ES", // Spain
                "ET", // Ethiopia
                "FI", // Finland
                "FJ", // Fiji
                "FK", // Falkland Islands (Malvinas)
                "FM", // Micronesia, Federated States of
                "FO", // Faroe Islands
                "FR", // France
                "GA", // Gabon
                "GB", // United Kingdom of Great Britain and Northern Ireland
                "GD", // Grenada
                "GE", // Georgia
                "GF", // French Guiana
                "GG", // Guernsey
                "GH", // Ghana
                "GI", // Gibraltar
                "GL", // Greenland
                "GM", // Gambia
                "GN", // Guinea
                "GP", // Guadeloupe
                "GQ", // Equatorial Guinea
                "GR", // Greece
                "GS", // South Georgia and the South Sandwich Islands
                "GT", // Guatemala
                "GU", // Guam
                "GW", // Guinea-Bissau
                "GY", // Guyana
                "HK", // Hong Kong
                "HM", // Heard Island and McDonald Islands
                "HN", // Honduras
                "HR", // Croatia
                "HT", // Haiti
                "HU", // Hungary
                "ID", // Indonesia
                "IE", // Ireland
                "IL", // Israel
                "IM", // Isle of Man
                "IN", // India
                "IO", // British Indian Ocean Territory
                "IQ", // Iraq
                "IR", // Iran, Islamic Republic of
                "IS", // Iceland
                "IT", // Italy
                "JE", // Jersey
                "JM", // Jamaica
                "JO", // Jordan
                "JP", // Japan
                "KE", // Kenya
                "KG", // Kyrgyzstan
                "KH", // Cambodia
                "KI", // Kiribati
                "KM", // Comoros
                "KN", // Saint Kitts and Nevis
                "KP", // Korea, Democratic People's Republic of
                "KR", // Korea, Republic of
                "KW", // Kuwait
                "KY", // Cayman Islands
                "KZ", // Kazakhstan
                "LA", // Lao People's Democratic Republic
                "LB", // Lebanon
                "LC", // Saint Lucia
                "LI", // Liechtenstein
                "LK", // Sri Lanka
                "LR", // Liberia
                "LS", // Lesotho
                "LT", // Lithuania
                "LU", // Luxembourg
                "LV", // Latvia
                "LY", // Libya
                "MA", // Morocco
                "MC", // Monaco
                "MD", // Moldova, Republic of
                "ME", // Montenegro
                "MF", // Saint Martin (French part)
                "MG", // Madagascar
                "MH", // Marshall Islands
                "MK", // Macedonia, the former Yugoslav Republic of
                "ML", // Mali
                "MM", // Myanmar
                "MN", // Mongolia
                "MO", // Macao
                "MP", // Northern Mariana Islands
                "MQ", // Martinique
                "MR", // Mauritania
                "MS", // Montserrat
                "MT", // Malta
                "MU", // Mauritius
                "MV", // Maldives
                "MW", // Malawi
                "MX", // Mexico
                "MY", // Malaysia
                "MZ", // Mozambique
                "NA", // Namibia
                "NC", // New Caledonia
                "NE", // Niger
                "NF", // Norfolk Island
                "NG", // Nigeria
                "NI", // Nicaragua
                "NL", // Netherlands
                "NO", // Norway
                "NP", // Nepal
                "NR", // Nauru
                "NU", // Niue
                "NZ", // New Zealand
                "OM", // Oman
                "PA", // Panama
                "PE", // Peru
                "PF", // French Polynesia
                "PG", // Papua New Guinea
                "PH", // Philippines
                "PK", // Pakistan
                "PL", // Poland
                "PM", // Saint Pierre and Miquelon
                "PN", // Pitcairn
                "PR", // Puerto Rico
                "PS", // Palestine, State of
                "PT", // Portugal
                "PW", // Palau
                "PY", // Paraguay
                "QA", // Qatar
                "RE", // Réunion
                "RO", // Romania
                "RS", // Serbia
                "RU", // Russian Federation
                "RW", // Rwanda
                "SA", // Saudi Arabia
                "SB", // Solomon Islands
                "SC", // Seychelles
                "SD", // Sudan
                "SE", // Sweden
                "SG", // Singapore
                "SH", // Saint Helena, Ascension and Tristan da Cunha
                "SI", // Slovenia
                "SJ", // Svalbard and Jan Mayen
                "SK", // Slovakia
                "SL", // Sierra Leone
                "SM", // San Marino
                "SN", // Senegal
                "SO", // Somalia
                "SR", // Suriname
                "SS", // South Sudan
                "ST", // Sao Tome and Principe
                "SV", // El Salvador
                "SX", // Sint Maarten (Dutch part)
                "SY", // Syrian Arab Republic
                "SZ", // Swaziland
                "TC", // Turks and Caicos Islands
                "TD", // Chad
                "TF", // French Southern Territories
                "TG", // Togo
                "TH", // Thailand
                "TJ", // Tajikistan
                "TK", // Tokelau
                "TL", // Timor-Leste
                "TM", // Turkmenistan
                "TN", // Tunisia
                "TO", // Tonga
                "TR", // Turkey
                "TT", // Trinidad and Tobago
                "TV", // Tuvalu
                "TW", // Taiwan, Province of China
                "TZ", // Tanzania, United Republic of
                "UA", // Ukraine
                "UG", // Uganda
                "UM", // United States Minor Outlying Islands
                "US", // United States of America
                "UY", // Uruguay
                "UZ", // Uzbekistan
                "VA", // Holy See
                "VC", // Saint Vincent and the Grenadines
                "VE", // Venezuela, Bolivarian Republic of
                "VG", // Virgin Islands, British
                "VI", // Virgin Islands, U.S.
                "VN", // Viet Nam
                "VU", // Vanuatu
                "WF", // Wallis and Futuna
                "WS", // Samoa
                "YE", // Yemen
                "YT", // Mayotte
                "ZA", // South Africa
                "ZM", // Zambia
                "ZW", // Zimbabwe
            };
        }

        public static SteamCurrency FindCurrencyBySymbol(string currencySymbol)
        {
            return Array.Find(_currencies, q => StringComparer.InvariantCultureIgnoreCase.Equals(q.CurrencySymbol, currencySymbol));
        }

        public static string GetCountry(string language)
        {
            var pos = Array.BinarySearch<string>(_countries, language, StringComparer.OrdinalIgnoreCase);
            return pos >= 0 ? _countries[pos] : null;
        }

        public static SteamCurrency GetCurrency(int steamCurrencyId)
        {
            return Array.Find(_currencies, q => q.SteamId == steamCurrencyId);
        }

        public static SteamCurrency GetCurrency(string isoCode)
        {
            return Array.Find(_currencies, q => StringComparer.OrdinalIgnoreCase.Equals(q.IsoCode, isoCode));
        }

        public static string GetLanguage(string language)
        {
            var pos = Array.BinarySearch<string>(_languages, language, StringComparer.OrdinalIgnoreCase);
            return pos >= 0 ? _languages[pos] : null;
        }
    }
}