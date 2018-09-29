using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.Code;
using s32.Sceh.SteamApi;

namespace s32.Sceh.DataModel
{
    public class CardData
    {
        private readonly int _amount, _contextId;
        private readonly long _id, _appId, _classId, _instanceId, _marketFeeApp;
        private readonly ItemClass _itemClass;

        private readonly string _marketHashName, _marketName, _name, _type, _itemClassName,
            _iconUrl, _marketFeeAppName, _droprateName, _cardborderName, _eventName;

        private readonly bool _tradable, _marketable, _isFoilCard;

        public CardData(ApiInventoryResp.Asset asset, ApiInventoryResp.Description desc)
        {
            _id = asset.AssetId;
            _appId = asset.AppId;
            _classId = asset.ClassId;
            _instanceId = asset.InstanceId;
            _amount = asset.Amount;
            _contextId = asset.ContextId;
            _marketHashName = desc.MarketHashName;
            _marketName = desc.MarketName;
            _name = desc.Name;
            _type = desc.Type;
            _iconUrl = desc.IconUrl;
            _tradable = desc.Tradable;
            _marketable = desc.Marketable;
            _marketFeeApp = desc.MarketFeeApp;

            foreach (var tag in desc.Tags)
            {
                switch (tag.Category)
                {
                    case "Game":
                        _marketFeeAppName = tag.LocalizedTagName;
                        break;

                    case "item_class":
                        _itemClassName = tag.LocalizedTagName;
                        switch (tag.InternalName)
                        {
                            case "item_class_2":
                                _itemClass = ItemClass.TradingCard;
                                break;

                            case "item_class_3":
                                _itemClass = ItemClass.ProfileBackground;
                                break;

                            case "item_class_4":
                                _itemClass = ItemClass.Emoticon;
                                break;

                            case "item_class_7":
                                _itemClass = ItemClass.Gems;
                                break;
                        }
                        break;

                    case "cardborder":
                        _cardborderName = tag.LocalizedTagName;
                        switch (tag.InternalName)
                        {
                            case "cardborder_0":
                                _isFoilCard = false;
                                break;

                            case "cardborder_1":
                                _isFoilCard = true;
                                break;
                        }
                        break;

                    case "droprate":
                        _droprateName = tag.LocalizedTagName;
                        break;

                    case "Event":
                        _eventName = tag.LocalizedTagName;
                        break;
                }
            }

            if (_name != null && _marketHashName != null && _marketHashName.EndsWith("(trading card)", StringComparison.CurrentCultureIgnoreCase))
            {
                var pos = _name.LastIndexOf('(');
                if (pos > 0)
                    _name = _name.Remove(pos).Trim();
            }
        }

        public int Amount
        {
            get { return _amount; }
        }

        public long AppId
        {
            get { return _appId; }
        }

        public string CardborderName
        {
            get { return _cardborderName; }
        }

        public long ClassId
        {
            get { return _classId; }
        }

        public int ContextId
        {
            get { return _contextId; }
        }

        public string DroprateName
        {
            get { return _droprateName; }
        }

        public string EventName
        {
            get { return _eventName; }
        }

        public string IconUrl
        {
            get { return _iconUrl; }
        }

        public long Id
        {
            get { return _id; }
        }

        public long InstanceId
        {
            get { return _instanceId; }
        }

        public bool IsFoilCard
        {
            get { return _isFoilCard; }
        }

        public ItemClass ItemClass
        {
            get { return _itemClass; }
        }

        public string ItemClassName
        {
            get { return _itemClassName; }
        }

        public CardEqualityKey Key
        {
            get { return new CardEqualityKey(_classId, _instanceId, _marketHashName); }
        }

        public bool Marketable
        {
            get { return _marketable; }
        }

        public long MarketFeeApp
        {
            get { return _marketFeeApp; }
        }

        public string MarketFeeAppName
        {
            get { return _marketFeeAppName; }
        }

        public string MarketHashName
        {
            get { return _marketHashName; }
        }

        public string MarketName
        {
            get { return _marketName; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool Tradable
        {
            get { return _tradable; }
        }

        public string Type
        {
            get { return _type; }
        }

        public string GetMarketFeeAppName()
        {
            if (_marketFeeAppName != null)
                return _marketFeeAppName;

            if (_type != null)
            {
                var pos = _type.LastIndexOf(" - ");
                return pos > 0 ? _type.Remove(pos).Trim() : _type;
            }

            return null;
        }
    }
}
