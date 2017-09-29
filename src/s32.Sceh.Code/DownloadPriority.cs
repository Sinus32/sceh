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

        static DownloadPriority()
        {
            _order = new DownloadPriority[]
            {
                High = new DownloadPriority(0),
                Medium = new DownloadPriority(1),
                Low = new DownloadPriority(2),
            };
        }

        private DownloadPriority(int index)
        {
            _index = index;
        }

        public static IReadOnlyList<DownloadPriority> Order
        {
            get { return _order; }
        }

        public int OrderIndex
        {
            get { return _index; }
        }
    }
}