using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DingusGaming
{
    public class Item
    {
        public readonly string name;
        public readonly ushort itemID;
        public readonly int cost;

        public Item() { }

        public Item(string name, ushort itemID, int cost)
        {
            this.name = name;
            this.itemID = itemID;
            this.cost = cost;
        }
    }
}
