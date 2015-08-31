namespace DingusGaming.Store
{
    public class Item
    {
        public readonly int cost;
        public readonly ushort itemID;
        public readonly string name;

        public Item()
        {
        }

        public Item(string name, ushort itemID, int cost)
        {
            this.name = name;
            this.itemID = itemID;
            this.cost = cost;
        }
    }
}