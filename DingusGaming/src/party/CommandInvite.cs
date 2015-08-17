namespace DingusGaming
{
	public class CommandInvite : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "invite"; }
		}

		public string Help
		{
			get { return "Invite a player to your party."; }
		}

		public string Syntax
		{
			get { return "<player>"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "inv", "pinv", "pinvite" }; }
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
				DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/invite PlayerName\".");
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

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}
}