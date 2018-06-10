using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.SteamApi;

namespace s32.Sceh.DataModel
{
    public class Card : INotifyPropertyChanged
    {
        private int _amount, _contextId;
        private long _id, _appId, _classId, _instanceId, _marketFeeApp;
        private bool? _isFoilCard;
        private bool _isSelected, _tradable, _marketable, _isDuplicated, _otherHaveIt, _hide;
        private ItemClass _itemClass;

        private string _marketHashName, _marketName, _name, _type, _itemClassName,
            _iconUrl, _thumbnailUrl, _marketFeeAppName, _droprateName, _cardborderName, _eventName;

        private SteamProfileKey _owner;

        //[Obsolete]
        //public Card(SteamProfileKey owner, RgInventoryItem rgInvItem, RgDescriptionItem rgDescItem)
        //{
        //    _owner = owner;
        //    _id = rgInvItem.Id;
        //    _appId = rgDescItem.AppId;
        //    _classId = rgDescItem.ClassId;
        //    _instanceId = rgDescItem.InstanceId;
        //    _amount = rgInvItem.Amount;
        //    _pos = rgInvItem.Pos;
        //    _marketHashName = rgDescItem.MarketHashName;
        //    _marketName = rgDescItem.MarketName;
        //    _name = rgDescItem.Name;
        //    _type = rgDescItem.Type;
        //    _iconUrl = rgDescItem.IconUrl;
        //    _tradable = rgDescItem.Tradable;
        //    _marketable = rgDescItem.Marketable;
        //    _marketFeeApp = rgDescItem.MarketFeeApp;

        //    FormatName();
        //}

        public Card(SteamProfileKey owner, ApiInventoryResp.Asset asset, ApiInventoryResp.Description desc)
        {
            _owner = owner;
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

            FormatName();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Amount
        {
            get { return _amount; }
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public long AppId
        {
            get { return _appId; }
            set
            {
                if (_appId != value)
                {
                    _appId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string CardborderName
        {
            get { return _cardborderName; }
            set
            {
                if (_cardborderName != value)
                {
                    _cardborderName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public long ClassId
        {
            get { return _classId; }
            set
            {
                if (_classId != value)
                {
                    _classId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int ContextId
        {
            get { return _contextId; }
            set
            {
                if (_contextId != value)
                {
                    _contextId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string DroprateName
        {
            get { return _droprateName; }
            set
            {
                if (_droprateName != value)
                {
                    _droprateName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string EventName
        {
            get { return _eventName; }
            set
            {
                if (_eventName != value)
                {
                    _eventName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool Hide
        {
            get { return _hide; }
            set
            {
                if (_hide != value)
                {
                    _hide = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string IconUrl
        {
            get { return _iconUrl; }
            set
            {
                if (_iconUrl != value)
                {
                    _iconUrl = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public long Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public long InstanceId
        {
            get { return _instanceId; }
            set
            {
                if (_instanceId != value)
                {
                    _instanceId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsDuplicated
        {
            get { return _isDuplicated; }
            set
            {
                if (_isDuplicated != value)
                {
                    _isDuplicated = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool? IsFoilCard
        {
            get { return _isFoilCard; }
            set
            {
                if (_isFoilCard != value)
                {
                    _isFoilCard = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged();
                    UpdateSteamApp();
                }
            }
        }

        public ItemClass ItemClass
        {
            get { return _itemClass; }
            set
            {
                if (_itemClass != value)
                {
                    _itemClass = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ItemClassName
        {
            get { return _itemClassName; }
            set
            {
                if (_itemClassName != value)
                {
                    _itemClassName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool Marketable
        {
            get { return _marketable; }
            set
            {
                if (_marketable != value)
                {
                    _marketable = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public long MarketFeeApp
        {
            get { return _marketFeeApp; }
            set
            {
                if (_marketFeeApp != value)
                {
                    _marketFeeApp = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string MarketFeeAppName
        {
            get { return _marketFeeAppName; }
            set
            {
                if (_marketFeeAppName != value)
                {
                    _marketFeeAppName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string MarketHashName
        {
            get { return _marketHashName; }
            set
            {
                if (_marketHashName != value)
                {
                    _marketHashName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string MarketName
        {
            get { return _marketName; }
            set
            {
                if (_marketName != value)
                {
                    _marketName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool OtherHaveIt
        {
            get { return _otherHaveIt; }
            set
            {
                if (_otherHaveIt != value)
                {
                    _otherHaveIt = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public SteamProfileKey Owner
        {
            get { return _owner; }
        }

        public string ThumbnailUrl
        {
            get { return _thumbnailUrl; }
            set
            {
                if (_thumbnailUrl != value)
                {
                    _thumbnailUrl = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool Tradable
        {
            get { return _tradable; }
            set
            {
                if (_tradable != value)
                {
                    _tradable = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    NotifyPropertyChanged();
                }
            }
        }

        internal WeakReference<SteamApp> SteamAppMySide { get; set; }

        internal WeakReference<SteamApp> SteamAppOtherSide { get; set; }

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

        public override string ToString()
        {
            return MarketHashName;
        }

        private void FormatName()
        {
            if (_name != null && _marketHashName != null && _marketHashName.EndsWith("(trading card)", StringComparison.CurrentCultureIgnoreCase))
            {
                var pos = _name.LastIndexOf('(');
                if (pos > 0)
                    _name = _name.Remove(pos).Trim();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateSteamApp()
        {
            SteamApp steamApp;

            if (SteamAppMySide != null && SteamAppMySide.TryGetTarget(out steamApp))
                steamApp.MyIsSelected = steamApp.MyCards.Any(q => q.IsSelected);

            if (SteamAppOtherSide != null && SteamAppOtherSide.TryGetTarget(out steamApp))
                steamApp.OtherIsSelected = steamApp.OtherCards.Any(q => q.IsSelected);
        }
    }
}
