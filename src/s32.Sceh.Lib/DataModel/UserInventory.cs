using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.DataModel
{
    public class UserInventory
    {
        public static readonly UserInventory Empty;

        static UserInventory()
        {
            Empty = new UserInventory();
            Empty.Profile = new SteamProfileKey(0, null);
            Empty.Cards = new List<CardData>();
            Empty.IsInventoryAvailable = true;
        }

        public List<CardData> Cards { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsInventoryAvailable { get; set; }
        public SteamProfileKey Profile { get; set; }
    }
}
