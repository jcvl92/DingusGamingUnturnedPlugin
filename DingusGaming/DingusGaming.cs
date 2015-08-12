using System;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;

namespace DingusGaming
{
	public class DGPlugin : Rocket.Core.Plugins.RocketPlugin
	{
		//contains helper functions for persisting data and centralizing system functions

		protected override void Load()
		{
			//is run after start by Rocket but still at initial load of the plugin
			Logger.LogWarning("\tPlugin loaded successfully!");
		}

		protected override void Unload()
		{
			
		}

		public void FixedUpdate()
		{
			//is called every game update
		}

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
			player.GiveItem(itemID, quantity);
		}

		public static string getConstantID(UnturnedPlayer player)
		{
			throw new UnimplementedException();
		}
	}

	public class GlobalPlayerComponent : UnturnedPlayerComponent
	{
		private void FixedUpdate()
		{
			//death messages
			if (this.Player.Dead && !dead)
			{
				dead = true;
				
				//get the killing player
				killer = this.Player.Death.getCause().player;

				DGPlugin.messagePlayer(this.Player, "You have been killed by "+killer+"!");
			}
			if (!this.Player.Dead && dead)
				dead = false;
		}
	}
}