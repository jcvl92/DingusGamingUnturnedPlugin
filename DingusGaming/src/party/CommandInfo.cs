namespace DingusGaming
{
	public class CommandInfo : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "info"; }
		}

		public string Help
		{
			get { return "Get info on your party or a party member."; }
		}

		public string Syntax
		{
			get { return "(<player>)"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "pinfo", "partyinfo", "inf", "pinf" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			string playerName = string.Join(" ", command);

			Party party = Parties.getParty(caller);
			if (party != null)
			{
				//get info on the whole party
				if (command.Length == 0)
				{
					string info = party.getInfo();
					DGPlugin.messagePlayer(caller, info);
				}

				//get info on a member
				else
				{
					//check for player existence
					UnturnedPlayer player = DGPlugin.getPlayer(playerName);
					if (player == null)
					{
						DGPlugin.messagePlayer(caller, "Failed to find player named \"" + playerName + "\"");
						return;
					}

					if (party.isMember(player))
					{
						string info = "Name: " + player.CharacterName + ", " +
									(player.Dead ? "Player is dead." :
									"Health: " + player.Health + ", " +
									"Hunger: " + player.Hunger + ", " +
									"Thirst: " + player.Thirst + ", " +
									"Infection: " + player.Infection);

						DGPlugin.messagePlayer(caller, info);
					}
					else
						DGPlugin.messagePlayer(caller, player.CharacterName + " is not in your party. You can only get info on party members.");
				}
			}
			else
				DGPlugin.messagePlayer(caller, "You are not in a party. You can only get info on party members.");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}
}