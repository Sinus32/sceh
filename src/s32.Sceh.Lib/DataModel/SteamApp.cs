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
        private bool _hide;
        private long _id;
        private ObservableCollection<Card> _myCards, _otherCards;
        private string _name;

        public SteamApp()
        {
            _myCards = new ObservableCollection<Card>();
            _otherCards = new ObservableCollection<Card>();
        }

        public SteamApp(long id, string name)
        {
            _id = id;
            _name = name;
            _myCards = new ObservableCollection<Card>();
            _otherCards = new ObservableCollection<Card>();
        }

        public SteamApp(long id, string name, IEnumerable<Card> myCards, IEnumerable<Card> otherCards)
        {
            _id = id;
            _name = name;
            _myCards = new ObservableCollection<Card>(myCards);
            _otherCards = new ObservableCollection<Card>(otherCards);
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<Card> MyCards
        {
            get { return _myCards; }
            set
            {
                if (_myCards != value)
                {
                    _myCards = value;
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

        public ObservableCollection<Card> OtherCards
        {
            get { return _otherCards; }
            set
            {
                if (_otherCards != value)
                {
                    _otherCards = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}