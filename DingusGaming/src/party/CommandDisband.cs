namespace DingusGaming
{
	public class CommandDisband : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "disband"; }
		}

		public string Help
		{
			get { return "Disband your party."; }
		}

		public string Syntax
		{
			get { return ""; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "disbandparty" }; }
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
			Party party = Parties.getParty(caller);
			if (party != null)
			{
				if (party.isLeader(caller))
				{
					party.tellParty("Your party has been disbanded!");
					party.disband();
				}
				else
				{
					DGPlugin.messagePlayer(caller, "Only the party leader("+party.getLeader().CharacterName+") can disband that party.");
				}
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