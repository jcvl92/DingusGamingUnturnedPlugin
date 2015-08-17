namespace DingusGaming
{
	public class CommandKick : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "kick"; }
		}

		public string Help
		{
			get { return "Kick a player from your party."; }
		}

		public string Syntax
		{
			get { return "<player>"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "pkick", "remove" }; }
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
				DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/kick PlayerName\".");
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
		        party.kickMember(caller, player);
                DGPlugin.messagePlayer(player, "You have been removed from the party.");
		    }
		    else
		        DGPlugin.messagePlayer(caller, "You are not in a party.");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}
}