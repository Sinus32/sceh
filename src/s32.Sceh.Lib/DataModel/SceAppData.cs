using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.DataModel
{
    public class SceAppData
    {
        private bool _cardsAvailable, _lastCard, _enabled, _marketable;
        private long _id;
        private string _name;
        private int _worth, _cardsInStock, _totalCards;

        public bool CardsAvailable
        {
            get { return _cardsAvailable; }
            set { _cardsAvailable = value; }
        }

        public int CardsInStock
        {
            get { return _cardsInStock; }
            set { _cardsInStock = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public bool LastCard
        {
            get { return _lastCard; }
            set { _lastCard = value; }
        }

        public bool Marketable
        {
            get { return _marketable; }
            set { _marketable = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public SceAppState State
        {
            get
            {
                if (_cardsAvailable)
                    return _lastCard ? SceAppState.LastCard : SceAppState.Normal;
                else
                    return _enabled ? SceAppState.NoCards : SceAppState.Disabled;
            }
        }

        public int TotalCards
        {
            get { return _totalCards; }
            set { _totalCards = value; }
        }

        public int Worth
        {
            get { return _worth; }
            set { _worth = value; }
        }
    }
}