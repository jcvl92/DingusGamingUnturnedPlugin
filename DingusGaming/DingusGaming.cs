using System.Collections;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Steamworks;
using UnityEngine;

namespace DingusGaming
{
	public class DGPlugin : RocketPlugin
	{
		//contains helper functions for persisting data and centralizing common functions

		protected override void Load() 
		{
            //Initialize plugin components

            //Currency
		    Currency.init();

		    //Store
            Stores.init();

		    //Party
            Parties.init();

            Logger.LogWarning("DingusGaming Plugin Loaded!");
        }

		protected override void Unload()
		{
			//is called by Rocket before shutting down
		}

		public void FixedUpdate()
		{
			//is called every game update
		}

		/********** HELPER FUNCTIONS **********/

		public static void messagePlayer(UnturnedPlayer player, string text)
		{
			List<string> strs = UnturnedChat.wrapMessage(text);
			foreach (string str in strs)
			{
				UnturnedChat.Say(player, str);
                //TODO: debug code here V
                //SDG.Unturned.ChatManager.say(player.CSteamID, str, Color.white);
                //SDG.Unturned.ChatManager.Instance.tellChat(player.CSteamID, player.CSteamID, 0, Color.black, str);
            }
		}

        public static List<DictionaryEntry> convertFromDictionary(IDictionary dictionary)
        {
            List<DictionaryEntry> entries = new List<DictionaryEntry>(dictionary.Count);
            foreach (object key in dictionary.Keys)
                entries.Add(new DictionaryEntry(key, dictionary[key]));
            return entries;
        }

	    public static Dictionary<TKey, TValue> convertToDictionary<TKey, TValue>(List<DictionaryEntry> entries)
	    {
	        Dictionary<TKey, TValue> dictionary= new Dictionary<TKey, TValue>();
            foreach (DictionaryEntry entry in entries)
                dictionary[(TKey)entry.Key] = (TValue)entry.Value;
	        return dictionary;
	    }

        public static UnturnedPlayer getPlayer(string name)
		{
			return UnturnedPlayer.FromName(name);
		}

		public static void givePlayerItem(UnturnedPlayer player, ushort itemID, int quantity)
		{
			player.GiveItem(itemID, (byte)quantity);
		}

		public static string getConstantID(UnturnedPlayer player)
		{
			return player.CSteamID.ToString();
		}

		public static UnturnedPlayer getPlayer(CSteamID playerID)
		{
			return UnturnedPlayer.FromCSteamID(playerID);
		}

	    public static void writeToFile(object obj, string fileName)
	    {
            XmlSerializer serializer = new XmlSerializer(obj.GetType(), "");
	        XmlTextWriter xmlTextWriter = null;

	        try
	        {
	            xmlTextWriter = new XmlTextWriter("DGPLugin_" + fileName, Encoding.UTF8);
	            xmlTextWriter.Formatting = Formatting.Indented;
	            serializer.Serialize(xmlTextWriter, obj);
	        }
	        finally
	        {
                xmlTextWriter?.Close();
            }
	    }

	    public static T readFromFile<T>(string fileName)
	    {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
	        XmlTextReader xmlTextReader = null;

	        try
	        {
	            xmlTextReader = new XmlTextReader("DGPLugin_" + fileName);

	            if (serializer.CanDeserialize(xmlTextReader))
	                return (T) serializer.Deserialize(xmlTextReader);
	            return default(T);
	        }
	        catch (FileNotFoundException)
	        {
	            return default(T);
	        }
	        finally
	        {
                xmlTextReader?.Close();
	        }
        }
	}
}