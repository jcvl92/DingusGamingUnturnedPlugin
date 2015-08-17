using System.Collections;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using SDG.Unturned;
using Steamworks;

namespace DingusGaming
{
	public class Store
	{
		public readonly List<Item> items = new List<Item>();
		public readonly string name;

		public Item getItemById(ushort itemID)
		{
			return items.Find(x => x.itemID == itemID);
		}

		public class Item
		{
			public readonly string name;
			public readonly ushort itemID;
			public readonly int cost;

			public Item(){}

			public Item(string name, ushort itemID, int cost)
			{
				this.name = name;
				this.itemID = itemID;
				this.cost = cost;
			}
		}
	}
}