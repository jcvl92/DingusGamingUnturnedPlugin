using Rocket.RocketAPI;
using System.Collections.Generic;

namespace DingusGaming.Party
{
	public class CommandDisband : IRocketCommand
	{
		private const string NAME = "disband"; 
		private const string HELP = "Disband your party.";
		private const string SYNTAX = "";
		private readonly List<string> ALIASES = new List<string> { "disbandparty" };
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

//		public void Execute(IRocketPlayer caller, string[] command)
//		{
//			Execute((UnturnedPlayer)caller, command);
//		}
	}
}