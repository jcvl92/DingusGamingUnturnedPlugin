using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DingusGaming.Store;
using DingusGaming.DingusGaming.helper;
using System.Collections;

namespace DingusGaming.DingusGaming.data
{
    class Xml : DataAccess
    {
        public Dictionary<string, int> getBalances()
        {
            List<DictionaryEntry> temp = File.readFromXml<List<DictionaryEntry>>(Settings.getSettings()["balance.file"]);
            if (temp != null)
                return Conversion.convertToDictionary<string, int>(temp);
            else
                return new Dictionary<string, int>();
        }

        public List<Store.Store> getStores()
        {
            return File.readFromXml<List<Store.Store>>(Settings.getSettings()["stores.file"]); ;
        }

        public void setBalances(Dictionary<string, int> balances)
        {
            File.writeToXml(Conversion.convertFromDictionary(balances), Settings.getSettings()["balance.file"]);
        }

        public void setStores(List<Store.Store> stores)
        {
            throw new NotImplementedException();
        }
    }
}
