using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.DataModel
{
    public class StAppData
    {
        private long _id;
        private string _name;
        private double _setPrice, _cardAvg;

        public double CardAvg
        {
            get { return _cardAvg; }
            set { _cardAvg = value; }
        }

        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public double SetPrice
        {
            get { return _setPrice; }
            set { _setPrice = value; }
        }
    }
}