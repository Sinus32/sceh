using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.Code;
using s32.Sceh.SteamApi;

namespace s32.Sceh.DataModel
{
    public class Card : INotifyPropertyChanged
    {
        private readonly CardData _cardData;
        private readonly SteamProfileKey _owner;
        private readonly WeakReference<SteamApp> _steamApp;
        private int _duplicateCount;
        private bool _isSelected, _isDuplicated, _otherHaveIt, _hide;
        private string _thumbnailUrl;

        public Card(SteamProfileKey owner, CardData cardData, SteamApp steamApp)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));
            if (cardData == null)
                throw new ArgumentNullException(nameof(cardData));
            if (steamApp == null)
                throw new ArgumentNullException(nameof(steamApp));

            _owner = owner;
            _cardData = cardData;
            _steamApp = new WeakReference<SteamApp>(steamApp);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Amount
        {
            get { return _cardData.Amount; }
        }

        public long AppId
        {
            get { return _cardData.AppId; }
        }

        public string CardborderName
        {
            get { return _cardData.CardborderName; }
        }

        public long ClassId
        {
            get { return _cardData.ClassId; }
        }

        public int ContextId
        {
            get { return _cardData.ContextId; }
        }

        public string DroprateName
        {
            get { return _cardData.DroprateName; }
        }

        public int DuplicateCount
        {
            get { return _duplicateCount; }
            set
            {
                if (_duplicateCount != value)
                {
                    _duplicateCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string EventName
        {
            get { return _cardData.EventName; }
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
            get { return _cardData.IconUrl; }
        }

        public long Id
        {
            get { return _cardData.Id; }
        }

        public long InstanceId
        {
            get { return _cardData.InstanceId; }
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

        public bool IsFoilCard
        {
            get { return _cardData.IsFoilCard; }
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
            get { return _cardData.ItemClass; }
        }

        public string ItemClassName
        {
            get { return _cardData.ItemClassName; }
        }

        public CardEqualityKey Key
        {
            get { return _cardData.Key; }
        }

        public bool Marketable
        {
            get { return _cardData.Marketable; }
        }

        public long MarketFeeApp
        {
            get { return _cardData.MarketFeeApp; }
        }

        public string MarketFeeAppName
        {
            get { return _cardData.MarketFeeAppName; }
        }

        public string MarketHashName
        {
            get { return _cardData.MarketHashName; }
        }

        public string MarketName
        {
            get { return _cardData.MarketName; }
        }

        public string Name
        {
            get { return _cardData.Name; }
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
            get { return _cardData.Tradable; }
        }

        public string Type
        {
            get { return _cardData.Type; }
        }

        public override string ToString()
        {
            return MarketHashName;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateSteamApp()
        {
            SteamApp steamApp;
            if (_steamApp != null && _steamApp.TryGetTarget(out steamApp))
            {
                steamApp.MyIsSelected = steamApp.MyCards.Any(q => q.IsSelected);
                steamApp.OtherIsSelected = steamApp.OtherCards.Any(q => q.IsSelected);
            }
        }
    }
}
