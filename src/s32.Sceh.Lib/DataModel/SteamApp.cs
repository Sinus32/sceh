using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.DataModel
{
    public class SteamApp : INotifyPropertyChanged
    {
        internal int _myCardsTotal, _myUniqueCards, _otherCardsTotal, _otherUniqueCards;
        private readonly long _id;
        private readonly List<Card> _myCards, _otherCards;
        private readonly string _name;
        private bool _hide, _myIsSelected, _otherIsSelected;
        private bool? _marketable;
        private SceAppState _sceState;
        private double _setPrice, _cardAvg, _boosterAvg;
        private int? _totalUniqueCards, _sceWorth;

        public SteamApp(long id, string name)
        {
            _id = id;
            _name = name;
            _myCards = new List<Card>();
            _otherCards = new List<Card>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public double BoosterAvg
        {
            get { return _boosterAvg; }
            set { _boosterAvg = value; }
        }

        public double CardAvg
        {
            get { return _cardAvg; }
            set { _cardAvg = value; }
        }

        public bool HeHaveMoreThanHalf
        {
            get { return OtherCardsTotal > (TotalUniqueCards + 1) / 2; }
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

        public long Id
        {
            get { return _id; }
        }

        public bool IHaveMoreThanHalf
        {
            get { return MyCardsTotal > (TotalUniqueCards + 1) / 2; }
        }

        public bool? Marketable
        {
            get { return _marketable; }
            set { _marketable = value; }
        }

        public IList<Card> MyCards
        {
            get { return _myCards; }
        }

        public int MyCardsTotal
        {
            get { return _myCardsTotal; }
            set { _myCardsTotal = value; }
        }

        public bool MyIsSelected
        {
            get { return _myIsSelected; }
            set
            {
                if (_myIsSelected != value)
                {
                    _myIsSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int MyUniqueCards
        {
            get { return _myUniqueCards; }
            set { _myUniqueCards = value; }
        }

        public string Name
        {
            get { return _name; }
        }

        public IList<Card> OtherCards
        {
            get { return _otherCards; }
        }

        public int OtherCardsTotal
        {
            get { return _otherCardsTotal; }
            set { _otherCardsTotal = value; }
        }

        public bool OtherIsSelected
        {
            get { return _otherIsSelected; }
            set
            {
                if (_otherIsSelected != value)
                {
                    _otherIsSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int OtherUniqueCards
        {
            get { return _otherUniqueCards; }
            set { _otherUniqueCards = value; }
        }

        public SceAppState SceState
        {
            get { return _sceState; }
            set { _sceState = value; }
        }

        public int? SceWorth
        {
            get { return _sceWorth; }
            set { _sceWorth = value; }
        }

        public double SetPrice
        {
            get { return _setPrice; }
            set { _setPrice = value; }
        }

        public int? TotalUniqueCards
        {
            get { return _totalUniqueCards; }
            set { _totalUniqueCards = value; }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
