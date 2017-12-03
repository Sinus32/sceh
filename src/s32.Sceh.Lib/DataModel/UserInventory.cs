using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.DataModel
{
    public class UserInventory
    {
        public List<Card> Cards { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsInventoryAvailable { get; set; }
        public long SteamId { get; set; }
    }
}