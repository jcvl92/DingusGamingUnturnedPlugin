using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Party
{
    public class CommandInvite : IRocketCommand
    {
        private const string NAME = "invite";
        private const string HELP = "Invite a player to your party.";
        private const string SYNTAX = "<player>";
        private const bool ALLOW_FROM_CONSOLE = false;
        private const bool RUN_FROM_CONSOLE = false;

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

        public List<string> Aliases { get; } = new List<string> {"inv", "pinv", "pinvite"};

        public bool AllowFromConsole
        {
            get { return ALLOW_FROM_CONSOLE; }
        }

        public List<string> Permissions { get; } = new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Execute((UnturnedPlayer) caller, command);
        }

        public void Execute(UnturnedPlayer caller, string[] command)
        {
            //check for parameter vaidity
            if (command.Length == 0)
            {
                DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/invite PlayerName\".");
                return;
            }

            var playerName = string.Join(" ", command);

            //check for player existence
            var player = DGPlugin.getPlayer(playerName);
            if (player == null)
            {
                DGPlugin.messagePlayer(caller, "Failed to find player named \"" + playerName + "\"");
                return;
            }

            //disable the ability of a user to invite themself to a party
            if (player.Equals(caller))
            {
                DGPlugin.messagePlayer(caller, "You cannot invite yourself to a party!");
                return;
            }

            //if caller is in a party, invite. otherwise create first
            var party = Parties.getParty(caller);
            if (party != null)
            {
                if (party.isLeader(caller))
                    Parties.invitePlayer(caller, player);
                else
                    DGPlugin.messagePlayer(caller,
                        "Only the party leader(" + party.getLeader().CharacterName + ") can invite members.");
            }
            else
                Parties.invitePlayer(caller, player);
        }
    }
}