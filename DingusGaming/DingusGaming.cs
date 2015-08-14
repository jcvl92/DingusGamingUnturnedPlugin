using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Steamworks;

namespace DingusGaming
{
	public class DGPlugin : RocketPlugin
	{
		//contains helper functions for persisting data, global event handling, and centralizing system functions

		protected override void Load() 
		{
			//is called after start by Rocket but still at initial load of the plugin
			Logger.LogWarning("\tPlugin loaded successfully!");
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
			}
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

		internal static UnturnedPlayer getPlayer(CSteamID playerID)
		{
			return UnturnedPlayer.FromCSteamID(playerID);
		}
	}
}