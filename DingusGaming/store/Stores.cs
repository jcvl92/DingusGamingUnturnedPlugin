using System.Collections.Generic;
using Rocket.Unturned.Player;

namespace DingusGaming.Store
{
    public class Stores
    {
        private static List<Store> stores;

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
            var str = "";
            for (var i = 0; i < stores.Count; ++i)
                str += "(" + (i + 1) + ")" + stores[i].name + ", ";
            return str.Substring(0, str.Length - 2);
        }

        public static string viewSubstore(int storeNumber)
        {
            //check the bounds
            if (storeNumber < 1 || storeNumber > stores.Count)
                return "Store number does not exist!";

            var str = "";
            foreach (var item in stores[storeNumber - 1].items)
                str += "$" + item.cost + "-" + item.name + "(" + item.itemID + "), ";
            return str.Substring(0, str.Length - 2);
        }

        public static void purchase(UnturnedPlayer caller, ushort itemId, byte quantity)
        {
            if (quantity <= 0)
            {
                DGPlugin.messagePlayer(caller, "Invalid item quantity.");
                return;
            }

            //search for the item in one of the stores
            var item = findItemById(itemId);

            if (item == null)
            {
                DGPlugin.messagePlayer(caller, "Could not find item in the stores.");
                return;
            }

            //check to see if the caller has sufficient funds to make the purchase
            if (Currency.getBalance(caller) >= item.cost*quantity)
            {
                if (DGPlugin.givePlayerItem(caller, itemId, quantity))
                {
                    //subtract the cost of the item(s) from their balance
                    Currency.changeBalance(caller, item.cost*quantity*-1);
                    DGPlugin.messagePlayer(caller,
                        "You have purchased " + quantity + " " + item.name + " for $" + item.cost*quantity +
                        ". Your new balance is $" + Currency.getBalance(caller) + ".");
                }
                else
                    DGPlugin.messagePlayer(caller, item.name+" could not be purchased. Please file a bug report.");
            }
            else
                DGPlugin.messagePlayer(caller,
                    "Insufficient funds to purchase " + quantity + " " + item.name + "($" + Currency.getBalance(caller) +
                    "/$" + item.cost*quantity + ")!");
        }

        private static Item findItemById(ushort itemId)
        {
            Item item;
            foreach (var store in stores)
                if ((item = store.getItemById(itemId)) != null)
                    return item;
            return null;
        }
    }
}