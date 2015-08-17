namespace DingusGaming
{
	public class CommandTeleport : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "teleport"; }
		}

		public string Help
		{
			get { return "Teleport to a party member."; }
		}

		public string Syntax
		{
			get { return "<player>"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "tp", "pteleport", "ptp", "tpa" }; }
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
			//check for parameter vaidity
			if (command.Length == 0)
			{
				DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/tp PlayerName\".");
				return;
			}

			string playerName = string.Join(" ", command);

			//check for player existence
			UnturnedPlayer player = DGPlugin.getPlayer(playerName);
			if (player == null)
			{
				DGPlugin.messagePlayer(caller, "Failed to find player named \"" + playerName + "\"");
				return;
			}

			Party party = Parties.getParty(caller);
			if (party != null)
			{
				if (party.isMember(player))
				{
					caller.Teleport(player);
				}
				else
					DGPlugin.messagePlayer(caller, player.CharacterName + " is not in your party. You can only teleport to party members.");
			}
			else
				DGPlugin.messagePlayer(caller, "You are not in a party. You can only teleport to party members.");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}
}