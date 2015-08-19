using System.Collections;
using System.Collections.Generic;

namespace DingusGaming.DingusGaming.helper
{
    class Conversion
    {
        public static Dictionary<TKey, TValue> convertToDictionary<TKey, TValue>(List<DictionaryEntry> entries)
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            foreach (DictionaryEntry entry in entries)
                dictionary[(TKey)entry.Key] = (TValue)entry.Value;
            return dictionary;
        }

        public static List<DictionaryEntry> convertFromDictionary(IDictionary dictionary)
        {
            List<DictionaryEntry> entries = new List<DictionaryEntry>(dictionary.Count);
            foreach (object key in dictionary.Keys)
                entries.Add(new DictionaryEntry(key, dictionary[key]));
            return entries;
        }
    }
}
