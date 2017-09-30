using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Code
{
    public class DownloadPriority
    {
        public static readonly DownloadPriority High;
        public static readonly DownloadPriority Low;
        public static readonly DownloadPriority Medium;
        private static readonly DownloadPriority[] _order;
        private int _index;
        private string _name;

        static DownloadPriority()
        {
            _order = new DownloadPriority[]
            {
                High = new DownloadPriority(0, "High"),
                Medium = new DownloadPriority(1, "Medium"),
                Low = new DownloadPriority(2, "Low"),
            };
        }

        private DownloadPriority(int index, string name)
        {
            _index = index;
            _name = name;
        }

        public static IReadOnlyList<DownloadPriority> Order
        {
            get { return _order; }
        }

        public string Name
        {
            get { return _name; }
        }

        public int OrderIndex
        {
            get { return _index; }
        }

        public override string ToString()
        {
            return _name;
        }
    }
}