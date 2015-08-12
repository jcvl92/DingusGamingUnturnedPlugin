using System;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using DingusGaming;

namespace Store
{
	public class Currency
	{
		public static readonly int startingAmount = 5;
		static Dictionary<string, int> balances = new Dictionary<string, int>();

		public static void addPlayer(UnturnedPlayer player)
		{
			if(!balances.ContainsKey(DGPlugin.getConstantID(player)))
				balances.Add(player.STEAMID, startingAmount);
		}

		public static void changeBalance(UnturnedPlayer player, int amount)
		{
			balances[DGPlugin.getConstantID(player)] += amount;
		}

		public static int getBalance(UnturnedPlayer player)
		{
			return balances[DGPlugin.getConstantID(player)];
		}

		public static boolean transferBalance(UnturnedPlayer src, UnturnedPlayer dest, int amount)
		{
			if(amount > 0 && balances[src] >= amount)
			{
				balances[src] -= amount;
				balances[dest] += amount;
				return true;
			}
			return false
		}
	}

	public class Stores
	{
		public static readonly List<Store> stores = new List<Store>();

		static Stores
		{
			//TODO: read in stores object from file here
		}

		public static string listSubstores()
		{
			string str = "";
			for(int i=0; i<stores.Length; ++i)
				str += "("+(i+1)+")"+ stores[i] + ", ";
			return str.Substring(0, str.Length - 2);
		}

		public static string viewSubstore(int storeNumber)
		{
			//check the bounds
			if(storeNumber < 0 || storeNumber >= stores.Length)
				return "Store number does not exist!";

			string str = "";
			foreach(Item item in stores[storeNumber-1].items)
				str += "$"+item.cost+"-"+item.name + "("+item.itemID+"), ";
			return str.Substring(0, str.Length - 2);
		}

		public static void purchase(UnturnedPlayer caller, ushort itemID, int quantity)
		{
			if(quantity <= 0)
			{
				DGPlugin.messagePlayer(caller, "Invalid item quantity.");
				return;
			}

			//search for the item in one of the stores
			Item item = null;
			foreach(Store store in stores)
				if(item = getItem(itemID) != null)
					break;

			if(item == null)
			{
				DGPlugin.messagePlayer(caller, "Could not find item in the stores.");
				return;
			}

			//check to see if the caller has sufficient funds to make the purchase
			if(Currency.getBalance(caller) >= item.cost*quantity)
			{
				//subtract the cost of the item(s) from their balance
				Currency.changeBalance(caller, item.cost * quantity * -1);
				DGPlugin.givePlayerItem(caller, itemID, quantity);
				DGPlugin.messagePlayer(caller, "You have purchased "+quantity+" "+item.name+" for $"+item.cost*quantity+", current balance="+Currency.getBalance(caller)+".");
			}
			else
				DGPlugin.messagePlayer(caller, "Insufficient funds to purchase "+quantity+" "+item.name+"($"+Currency.getBalance(caller)+"/$"+item.cost*quantity+").");
		}

		class Store
		{
			public readonly List<Item> items = new List<Item>();

			public Item getItem(ushort itemID)
			{
				return items.Find(x => x.itemID == itemID);
			}

			class Item
			{
				public readonly string name;
				public readonly ushort itemID;
				public readonly int cost;

				public Item(name, itemID, cost)
				{
					this.name = name;
					this.itemID = itemID;
					this.cost = cost;
				}
			}
		}
	}

	/*
		commands will be as follows:
		/store - view substores(weapons, building suppplies, ammo+attachments, medical+food+water, vehicle)
		/store <substore> - view substore items(list of less than 10) items have names and IDs
		/purchase <item_ID> (<quantity>) - purchase one or more items with currency
		/balance - shows a user their current credit balance
		/gift <amount> <playerName> - gifts another player a whole-number amount of credits(round down in casting), cannot be 0 or negative.
		^displays a confirmation, then the user must do /gconfirm(/confirmgift) or /gcancel(/cancelgift)
		*/

		/*
		events will be as follows:
		onUnload - balances will be dumped to file
		onLoad - balances will be read from file
	*/

	public class StorePlayerComponent : UnturnedPlayerComponent
	{
		private void FixedUpdate()
		{
			//death rewards for killing players
			if (this.Player.Dead && !dead)
			{
				dead = true;
				
				//get the killing player
				killer = this.Player.Death.getCause().player

				//grant the killing user 5 credits + 10% of their victim's credits
				Store.addCredits(killer, 5 + Store.getCredits(this.Player)/10);
			}
			if (!this.Player.Dead && dead)
				dead = false;
		}
	}
}