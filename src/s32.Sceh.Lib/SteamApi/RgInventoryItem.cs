using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.SteamApi
{
    public class RgInventoryItem
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("classid")]
        public long ClassId { get; set; }

        [JsonProperty("instanceid")]
        public long InstanceId { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("pos")]
        public int Pos { get; set; }
    }
}
