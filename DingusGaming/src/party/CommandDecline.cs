namespace DingusGaming
{
	public class CommandDecline : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "decline"; }
		}

		public string Help
		{
			get { return "Decline a party invitation."; }
		}

		public string Syntax
		{
			get { return ""; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "pdecline", "invdecline" }; }
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
			Parties.declineInvite(caller);
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}
}