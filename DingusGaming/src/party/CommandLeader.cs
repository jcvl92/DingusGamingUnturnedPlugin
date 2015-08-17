namespace DingusGaming
{
	public class CommandLeader : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "leader"; }
		}

		public string Help
		{
			get { return "Transfer party leader to another member."; }
		}

		public string Syntax
		{
			get { return "<player>"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "pleader" }; }
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
				DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/leader PlayerName\".");
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
				party.makeLeader(caller, player);
			else
				DGPlugin.messagePlayer(caller, "You are not in a party.");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}
}