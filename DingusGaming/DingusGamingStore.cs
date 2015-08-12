using System;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;

namespace DingusGaming
{
	public class Currency
	{
		public static readonly int startingAmount = 200;//TODO: change this back to 5, 200 is just for testing purposes
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

		public static boolean transferCredits(UnturnedPlayer src, UnturnedPlayer dest, int amount)
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
			if(storeNumber < 1 || storeNumber > stores.Length)
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

	public class StorePlayerComponent : UnturnedPlayerComponent
	{
		private void FixedUpdate()
		{
			//death rewards for killing players
			if (this.Player.Dead && !dead)
			{
				dead = true;
				
				//get the killing player
				killer = this.Player.Death.getCause().player;

				//grant the killing user 5 credits + 10% of their victim's credits
				Store.addCredits(killer, 5 + Store.getCredits(this.Player)/10);
			}
			if (!this.Player.Dead && dead)
				dead = false;
		}
	}

	/********** COMMANDS **********/

	public class CommandStore : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "store"; }
		}

		public string Help
		{
			get { return "Access the store."; }
		}

		public string Syntax
		{
			get { return "(<storeNumber>)"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "viewstore", "s" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			if(command.Length == 0)
				DGPlugin.messagePlayer(caller, Stores.listSubstores());
			else if(command.Length == 1)
			{
				int num;
				if(Int32.TryParse(command[0], out num));
					DGPlugin.messagePlayer(caller, Stores.viewSubstore(num));
				else
					DGPlugin.messagePlayer(caller, "Invalid storeNumber.");
			}
			else
				DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/store\" or \"/store storeNumber\".");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}

	public class CommandBuy : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "buy"; }
		}

		public string Help
		{
			get { return "Purchase an item from the store."; }
		}

		public string Syntax
		{
			get { return "<itemID> (<quantity>)"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "purchase", "b", "buyitem", "purchaseitem" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			if(command.Length == 0 || command.Length > 2)
				DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/buy itemID\" or \"/buy itemID quantity\".");
			else
			{
				int itemID, quantity=1;

				if(!Int32.TryParse(command[0], out itemID));
					DGPlugin.messagePlayer(caller, "Invalid itemID.");
				else if(command.Length == 2 && !Int32.TryParse(command[1], out quantity)
					DGPlugin.messagePlayer(caller, "Invalid quantity.");
				else
					DGPlugin.messagePlayer(caller, Stores.purchase(caller, itemID, quantity));
			}
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}

	public class CommandBalance : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "balance"; }
		}

		public string Help
		{
			get { return "View your credit balance."; }
		}

		public string Syntax
		{
			get { return ""; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "bank", "wallet", "viewwallet", "viewbalance", "viewbank" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			if(command.Length > 0)
				DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/balance\".");
			else
				DGPlugin.messagePlayer(caller, "You currently have "+Currency.getBalance(caller)+" credits.");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}

	public class CommandTransfer : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "transfer"; }
		}

		public string Help
		{
			get { return "Transfer credits to another player."; }
		}

		public string Syntax
		{
			get { return "<amount> <playerName>"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "gift", "giftcredits", "transfercredits", "sendcredits" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			if(command.Length != 2)
				DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/transfer amount playerName\".");
			else
			{
				int amount;
				if(!Int32.TryParse(command[0], out amount))
					DGPlugin.messagePlayer(caller, "Invalid amount.");
				else
				{
					string playerName = Join(" ", Skip(command, 1));//TODO: might need.ToArray() at the end of it
					UnturnedPlayer player;
					if(player = DGPlugin.getPlayer(playerName) == null)
						DGPlugin.messagePlayer(caller, "Failed to find player named \"" + playerName + "\"");
					else
					{
						Currency.transferCredits(caller, player, amount);
						DGPlugin.messagePlayer(caller, "You have sent "+amount+" credits to "+playerName+".");
					}
				}
			}
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}
}