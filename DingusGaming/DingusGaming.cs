using Rocket.Logging;
using Rocket.RocketAPI;
using SDG;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DingusGaming
{
    public class DGPlugin : RocketPlugin
    {
		//contains helper functions for persisting data and centralizing common functions

		protected override void Load()
		{
            //Initialize components
		    Store.Currency.init();
            Store.Stores.init();
            Party.Parties.init();

            Logger.LogWarning("DingusGaming Plugin Loaded!");
        }

		protected override void Unload()
		{
			//is called by Rocket before shutting down
            Steam.OnServerShutdown.Invoke();
		}

		public void FixedUpdate()
		{
			//is called every game update
		}

		/********** HELPER FUNCTIONS **********/

	    public static RocketPlayer getKiller(RocketPlayer player, EDeathCause cause, CSteamID murderer)
	    {
	        if (cause == EDeathCause.KILL)
	            //return !murderer.m_SteamID.Equals(90071992547409920) && !player.CSteamID.Equals(murderer);
                //get the last player that damaged them before they die(timeout is 30 seconds)
	            return getPlayer(murderer);
	        else
	            return null;
	    }
        
		public static void disableCommands()
		{

		}

		public static void enableCommands()
		{

		}

		public static void messagePlayer(RocketPlayer player, string message)
		{
			List<string> strs = RocketChatManager.wrapMessage(message);
			foreach (string str in strs)
                RocketChatManager.Say(player, "0"+str);
		}

		public static void broadcastMessage(string text)
		{
			List<string> strs = RocketChatManager.wrapMessage(text);
			foreach (string str in strs)
                RocketChatManager.Say(str);
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

        public static RocketPlayer getPlayer(string name)
		{
			return RocketPlayer.FromName(name);
		}

		public static void givePlayerItem(RocketPlayer player, ushort itemID, byte quantity)
		{
			player.GiveItem(itemID, quantity);
		}

		public static string getConstantID(RocketPlayer player)
		{
			return player.CSteamID.ToString();
		}

		public static RocketPlayer getPlayer(CSteamID playerID)
		{
			return RocketPlayer.FromCSteamID(playerID);
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