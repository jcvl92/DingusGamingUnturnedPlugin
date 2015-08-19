using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using DingusGaming.DingusGaming.helper;

namespace DingusGaming.Store
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
            stores = File.readFromXml<List<Store>>(Settings.getSettings()["stores.file"]);
        }

        public static string listSubstores()
        {
            // TODO: Refactor to a toString method
            string str = "";
            for (int i = 0; i < stores.Count; ++i)
                str += "(" + (i + 1) + ")" + stores[i].name + ", ";
            return str.Substring(0, str.Length - 2);
        }

        public static string viewSubstore(int storeNumber)
        {
            //check the bounds
            if (storeNumber < 1 || storeNumber > stores.Count)
                return "Store number does not exist!";

            string str = "";
            foreach (Item item in stores[storeNumber - 1].items)
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
            Item item = findItemById(itemId);

            if (item == null)
            {
                DGPlugin.messagePlayer(caller, "Could not find item in the stores.");
                return;
            }

            //check to see if the caller has sufficient funds to make the purchase
            if (Currency.getBalance(caller) >= item.cost * quantity)
            {
                //subtract the cost of the item(s) from their balance
                Currency.changeBalance(caller, item.cost * quantity * -1);
                DGPlugin.givePlayerItem(caller, itemId, quantity);
                DGPlugin.messagePlayer(caller, "You have purchased " + quantity + " " + item.name + " for $" + item.cost * quantity + ", current balance=" + Currency.getBalance(caller) + ".");
            }
            else
                DGPlugin.messagePlayer(caller, "Insufficient funds to purchase " + quantity + " " + item.name + "($" + Currency.getBalance(caller) + "/$" + item.cost * quantity + ").");
        }

        private static Item findItemById(ushort itemId)
        {
            Item item = null;
            foreach (Store store in stores)
                if ((item = store.getItemById(itemId)) != null)
                    return item;
            return null;
        }
    }
}