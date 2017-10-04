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
        private int _amount, _pos;
        private long _id, _appId, _classId, _instanceId, _marketFeeApp;
        private bool _isSelected, _tradable, _marketable, _isDuplicated, _otherHaveIt, _hide;
        private string _marketHashName, _name, _type, _iconUrl, _thumbnailUrl;

        public Card()
        { }

        public Card(RgInventoryItem rgInvItem, RgDescriptionItem rgDescItem)
        {
            _id = rgInvItem.Id;
            _appId = rgDescItem.AppId;
            _classId = rgDescItem.ClassId;
            _instanceId = rgDescItem.InstanceId;
            _amount = rgInvItem.Amount;
            _pos = rgInvItem.Pos;
            _marketHashName = rgDescItem.MarketHashName;
            _name = rgDescItem.Name;
            _type = rgDescItem.Type;
            _iconUrl = rgDescItem.IconUrl;
            _tradable = rgDescItem.Tradable;
            _marketable = rgDescItem.Marketable;
            _marketFeeApp = rgDescItem.MarketFeeApp;
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

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
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

        public int Pos
        {
            get { return _pos; }
            set
            {
                if (_pos != value)
                {
                    _pos = value;
                    NotifyPropertyChanged();
                }
            }
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

        public override string ToString()
        {
            return MarketHashName;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}