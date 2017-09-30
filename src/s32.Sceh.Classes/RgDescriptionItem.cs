using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Classes
{
    public class RgDescriptionItem
    {
        [JsonProperty("appid")]
        public long AppId { get; set; }

        [JsonProperty("classid")]
        public long ClassId { get; set; }

        [JsonProperty("instanceid")]
        public long InstanceId { get; set; }

        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }

        [JsonProperty("icon_url_large")]
        public string IconUrlLarge { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("market_hash_name")]
        public string MarketHashName { get; set; }

        [JsonProperty("market_name")]
        public string MarketName { get; set; }

        [JsonProperty("name_color")]
        public string NameColor { get; set; }

        [JsonProperty("background_color")]
        public string BackgroundColor { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("tradable")]
        public bool Tradable { get; set; }

        [JsonProperty("marketable")]
        public bool Marketable { get; set; }

        [JsonProperty("commodity")]
        public bool Commodity { get; set; }

        [JsonProperty("market_fee_app")]
        public long MarketFeeApp { get; set; }

        [JsonProperty("market_tradable_restriction")]
        public int MarketTradableRestriction { get; set; }

        [JsonProperty("market_marketable_restriction")]
        public int MarketMarketableRestriction { get; set; }
    }
}
