using System.Collections.Generic;

namespace DingusGaming.DingusGaming.data
{
    interface DataAccess
    {      
        List<Store.Store> getStores();
        void setStores(List<Store.Store> stores);

        Dictionary<string, int> getBalances();
        void setBalances(Dictionary<string, int> balances);
    }
}
