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
	}
}