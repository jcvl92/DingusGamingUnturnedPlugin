namespace DingusGaming
{
	public class Stores
	{
	    static List<Store> stores;

		public static void init()
		{
            loadStoreData();
		}
		
		private static void loadStoreData() 
		{
			//read in the stores data
            stores = DGPlugin.readFromFile<List<Store>>("stores.xml");
		}

		public static string listSubstores()
		{
			// TODO: Refactor to a toString method
			string str = "";
			for(int i=0; i<stores.Count; ++i)
				str += "("+(i+1)+")"+ stores[i].name + ", ";
			return str.Substring(0, str.Length - 2);
		}

		public static string viewSubstore(int storeNumber)
		{
			//check the bounds
			if(storeNumber < 1 || storeNumber > stores.Count) 
				return "Store number does not exist!";

			string str = "";
			foreach(Store.Item item in stores[storeNumber-1].items)
				str += "$"+item.cost+"-"+item.name + "("+item.itemID+"), ";
			return str.Substring(0, str.Length - 2);
		}

		public static void purchase(UnturnedPlayer caller, ushort itemId, byte quantity)
		{
			if(quantity <= 0)
			{
				return DGPlugin.messagePlayer(caller, "Invalid item quantity.");
			}

			//search for the item in one of the stores
			Store.item item = findItemById(itemId);

			if(item == null)
			{
				return DGPlugin.messagePlayer(caller, "Could not find item in the stores.");
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
	}
	
	private static Store.item findItemById(ushort itemId)
	{
		Store.Item item = null;
			foreach(Store store in stores)
				if((item = store.getItemById(itemID)) != null)
					return item;
		return null;					
	}
}