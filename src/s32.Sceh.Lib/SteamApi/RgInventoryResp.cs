using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace s32.Sceh.SteamApi
{
    public class RgInventoryResp
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("Error")]
        public string Error { get; set; }

        [JsonProperty("rgInventory")]
        public Dictionary<long, RgInventoryItem> RgInventory { get; set; }

        [JsonProperty("rgCurrency")]
        public List<RgCurrencyItem> RgCurrency { get; set; }

        [JsonProperty("rgDescriptions")]
        public Dictionary<RgDescriptionKey, RgDescriptionItem> RgDescriptions { get; set; }

        [JsonProperty("more")]
        public bool More { get; set; }

        [JsonProperty("more_start")]
        public string MoreStart { get; set; }

        public class RgDescriptionKeyConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;

                return base.CanConvertFrom(context, sourceType);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(RgDescriptionKey))
                    return true;

                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string)
                {
                    var parts = ((string)value).Split('_');
                    return new RgDescriptionKey(Int64.Parse(parts[0]), Int64.Parse(parts[1]));
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return ((RgDescriptionKey)value).ToString();
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        [TypeConverter(typeof(RgDescriptionKeyConverter))]
        public class RgDescriptionKey : IEquatable<RgDescriptionKey>
        {
            public RgDescriptionKey(long classId, long instanceId)
            {
                ClassId = classId;
                InstanceId = instanceId;
            }

            [JsonProperty("classid")]
            public long ClassId { get; set; }

            [JsonProperty("instanceid")]
            public long InstanceId { get; set; }

            public override bool Equals(object obj)
            {
                var other = obj as RgDescriptionKey;
                if (other == null)
                    return false;
                return Equals(other);
            }

            public bool Equals(RgDescriptionKey other)
            {
                return ClassId == other.ClassId && InstanceId == other.InstanceId;
            }

            public override int GetHashCode()
            {
                return ClassId.GetHashCode() ^ InstanceId.GetHashCode();
            }

            public override string ToString()
            {
                return String.Concat(ClassId, '_', InstanceId);
            }
        }
    }
}
