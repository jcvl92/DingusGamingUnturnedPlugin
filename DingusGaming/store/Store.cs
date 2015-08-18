using System.Collections.Generic;

namespace DingusGaming.Store
{
    public class Store
    {
        public readonly List<Item> items = new List<Item>();
        public readonly string name;

        public Item getItemById(ushort itemID)
        {
            return items.Find(x => x.itemID == itemID);
        }
    }
}