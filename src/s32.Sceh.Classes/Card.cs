using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace s32.Sceh.Classes
{
    public class Card
    {
        public long Id { get { return InventoryItem.Id; } }

        public long AppId { get { return DescriptionItem.AppId; } }

        public long ClassId { get { return DescriptionItem.ClassId; } }

        public long InstanceId { get { return DescriptionItem.InstanceId; } }

        public int Amount { get { return InventoryItem.Amount; } }

        public int Pos { get { return InventoryItem.Pos; } }

        public string Name { get { return DescriptionItem.Name; } }

        public string IconUrl { get { return DescriptionItem.IconUrl; } }

        public string MarketHashName { get { return DescriptionItem.MarketHashName; } }

        public long AppDataAppId { get { return DescriptionItem.AppData.AppId; } }

        public int AppDataItemType { get { return DescriptionItem.AppData.ItemType; } }

        public GetInventoryResponse.RgInventoryItem InventoryItem { get; set; }

        public GetInventoryResponse.RgDescriptionItem DescriptionItem { get; set; }

        public bool IsDuplicated { get; set; }

        public string ThumbnailUrl { get; set; }

        public override string ToString()
        {
            return String.Concat(AppDataAppId, '/', AppDataItemType, ' ', Name);
        }
    }
}