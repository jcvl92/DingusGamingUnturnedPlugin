using Rocket.RocketAPI;
using System.Collections.Generic;

namespace DingusGaming.Party
{
	public class CommandAccept : IRocketCommand
	{
		private const string NAME = "accept"; 
		private const string HELP = "Accept a party invitation.";
		private const string SYNTAX = "";
		private readonly List<string> ALIASES = new List<string> { "paccept", "invaccept" };
		private const bool ALLOW_FROM_CONSOLE = false;
		private const bool RUN_FROM_CONSOLE = false;
		private readonly List<string> REQUIRED_PERMISSIONS = new List<string>();
		
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

		public void Execute(RocketPlayer caller, string[] command)
		{
			Parties.acceptInvite(caller);
		}

//		public void Execute(IRocketPlayer caller, string[] command)
//		{
//			Execute((UnturnedPlayer)caller, command);
//		}
	}
}