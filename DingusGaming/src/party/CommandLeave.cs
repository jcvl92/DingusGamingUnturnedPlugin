namespace DingusGaming
{
	public class CommandLeave : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "leave"; }
		}

		public string Help
		{
			get { return "Leave your current party."; }
		}

		public string Syntax
		{
			get { return ""; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "pleave", "leaveparty", "quit", "pquit" }; }
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
				party.removeMember(caller);
				party.tellParty(caller.CharacterName + " has left the party.");
				DGPlugin.messagePlayer(caller, "You have left the party.");
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