using Rocket.RocketAPI;
using System.Collections.Generic;

namespace DingusGaming.Party
{
	public class CommandInvite : IRocketCommand
	{
		private const string NAME = "invite"; 
		private const string HELP = "Invite a player to your party.";
		private const string SYNTAX = "<player>";
		private readonly List<string> ALIASES = new List<string> { "inv", "pinv", "pinvite" };
		private const bool ALLOW_FROM_CONSOLE = false;
		private const bool RUN_FROM_CONSOLE = false;
		private readonly List<string> REQUIRED_PERMISSIONS = new List<string>();
		
		public bool RunFromConsole
		{
			get { return false; }
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
			//check for parameter vaidity
			if (command.Length == 0)
			{
				DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/invite PlayerName\".");
				return;
			}

			string playerName = string.Join(" ", command);

            //check for player existence
            RocketPlayer player = DGPlugin.getPlayer(playerName);
			if (player == null)
			{
				DGPlugin.messagePlayer(caller, "Failed to find player named \"" + playerName + "\"");
				return;
			}

			//disable the ability of a user to invite themself to a party
			if(player.Equals(caller))
			{
				DGPlugin.messagePlayer(caller, "You cannot invite yourself to a party!");
				return;
			}

			//if caller is in a party, invite. otherwise create first
			Party party = Parties.getParty(caller);
			if (party != null)
			{
				if (party.isLeader(caller))
					Parties.invitePlayer(caller, player);
				else
					DGPlugin.messagePlayer(caller, "Only the party leader(" + party.getLeader().CharacterName + ") can invite members.");
			}
			else
			{
				Parties.createParty(caller);
				Parties.invitePlayer(caller, player);
			}
		}

//		public void Execute(IRocketPlayer caller, string[] command)
//		{
//			Execute((UnturnedPlayer)caller, command);
//		}
	}
}