
namespace DingusGaming
{
	public class CommandBuy : IRocketCommand
	{
		private const string NAME = "buy"; 
		private const string HELP = "Purchase an item from the store.";
		private const string SYNTAX = "<itemID> (<quantity>)";
		private const List<string> ALIASES = new List<string> { "purchase", "b", "buyitem", "purchaseitem" };
		private const bool ALLOW_FROM_CONSOLE = false;
		private const bool RUN_FROM_CONSOLE = false;
		private const List<string> REQUIRED_PERMISSIONS = new List<string>();
		
		public bool RunFromConsole
		{
			get { return RUN_FROM_CONSOLE; }
		}

		public string Name
		{
			get { return NAME; }
		}

		public string Help
		{
			get { return HELP; }
		}

		public string Syntax
		{
			get { return SYNTAX; }
		}

		public List<string> Aliases
		{
			get { return ALIASES; }
		}

		public bool AllowFromConsole
		{
			get { return ALLOW_FROM_CONSOLE; }
		}

		public List<string> Permissions
		{
			get { return REQUIRED_PERMISSIONS; }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			if(command.Length == 0 || command.Length > 2)
				DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/buy itemID\" or \"/buy itemID quantity\".");
			else
			{
			    int itemID;
                byte quantity =1;

				if(!int.TryParse(command[0], out itemID))
					DGPlugin.messagePlayer(caller, "Invalid itemID.");
				else if(command.Length == 2 && !byte.TryParse(command[1], out quantity))
					DGPlugin.messagePlayer(caller, "Invalid quantity.");
				else
					Stores.purchase(caller, (ushort)itemID, quantity);
			}
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}
}