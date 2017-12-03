using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace s32.Sceh.SteamApi
{
    public class ApiInventoryResp
    {
        [JsonProperty("assets")]
        public List<Asset> Assets { get; set; }

        [JsonProperty("descriptions")]
        public List<Description> Descriptions { get; set; }

        [JsonProperty("last_assetid")]
        public long LastAssetId { get; set; }

        [JsonProperty("more_items")]
        public bool MoreItems { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("total_inventory_count")]
        public int TotalInventoryCount { get; set; }

        public class Asset
        {
            [JsonProperty("amount")]
            public int Amount { get; set; }

            [JsonProperty("appid")]
            public int AppId { get; set; }

            [JsonProperty("assetid")]
            public long AssetId { get; set; }

            [JsonProperty("classid")]
            public long ClassId { get; set; }

            [JsonProperty("contextid")]
            public int ContextId { get; set; }

            [JsonProperty("instanceid")]
            public long InstanceId { get; set; }
        }

        public class Description
        {
            [JsonProperty("appid")]
            public int AppId { get; set; }

            [JsonProperty("classid")]
            public long ClassId { get; set; }

            [JsonProperty("commodity")]
            public bool Commodity { get; set; }

            [JsonProperty("currency")]
            public int Currency { get; set; }

            [JsonProperty("icon_url")]
            public string IconUrl { get; set; }

            [JsonProperty("instanceid")]
            public long InstanceId { get; set; }

            [JsonProperty("marketable")]
            public bool Marketable { get; set; }

            [JsonProperty("market_fee_app")]
            public long MarketFeeApp { get; set; }

            [JsonProperty("market_hash_name")]
            public string MarketHashName { get; set; }

            [JsonProperty("market_name")]
            public string MarketName { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("tags")]
            public List<Tag> Tags { get; set; }

            [JsonProperty("tradable")]
            public bool Tradable { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }

        public class Tag
        {
            [JsonProperty("category")]
            public string Category { get; set; }

            [JsonProperty("internal_name")]
            public string InternalName { get; set; }

            [JsonProperty("localized_category_name")]
            public string LocalizedCategoryName { get; set; }

            [JsonProperty("localized_tag_name")]
            public string LocalizedTagName { get; set; }
        }
    }
}