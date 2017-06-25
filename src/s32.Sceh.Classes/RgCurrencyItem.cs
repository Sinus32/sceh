using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Classes
{
    public class RgCurrencyItem
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
