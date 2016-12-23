using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace s32.Sceh.Classes
{
    public class SteamApp
    {
        public SteamApp(long id, string name)
        {
            Id = id;
            Name = name;
            MyCards = new List<Card>();
            OtherCards = new List<Card>();
            MySet = new HashSet<string>();
            OtherSet = new HashSet<string>();
        }

        public long Id { get; private set; }

        public string Name { get; private set; }

        public List<Card> MyCards { get; private set; }

        public List<Card> OtherCards { get; private set; }

        public bool Hide { get; set; }

        public HashSet<string> MySet { get; set; }

        public HashSet<string> OtherSet { get; set; }
    }
}
