using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace s32.Sceh.Code
{
    public class SteamApp
    {
        public SteamApp(long id, string name)
        {
            Id = id;
            Name = name;
            MyCards = new List<Card>();
            OtherCards = new List<Card>();
            MySet = new HashSet<int>();
            OtherSet = new HashSet<int>();
        }

        public long Id { get; private set; }

        public string Name { get; private set; }

        public List<Card> MyCards { get; private set; }

        public List<Card> OtherCards { get; private set; }

        public bool Hide { get; set; }

        public HashSet<int> MySet { get; set; }

        public HashSet<int> OtherSet { get; set; }
    }
}
