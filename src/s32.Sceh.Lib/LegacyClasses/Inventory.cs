using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using s32.Sceh.DataModel;

namespace s32.Sceh.Classes
{
    public class Inventory
    {
        public string User { get; set; }

        public string Link { get; set; }

        public List<Card> Cards { get; set; }
    }
}