namespace DingusGaming
{
	public class CommandChat : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "p"; }
		}

		public string Help
		{
			get { return "Send a message to your party."; }
		}

		public string Syntax
		{
			get { return "<message>"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "party", "pchat", "partychat" }; }
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
				DGPlugin.messagePlayer(caller, "No message entered. Format is \"/p message\".");
				return;
			}

			string message = string.Join(" ", command);

			Party party = Parties.getParty(caller);
			if (party != null)
				party.chat(caller, message);
			else
				DGPlugin.messagePlayer(caller, "You are not in a party.");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}
}