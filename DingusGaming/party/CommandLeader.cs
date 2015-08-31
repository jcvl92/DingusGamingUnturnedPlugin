using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Party
{
    public class CommandLeader : IRocketCommand
    {
        private const string NAME = "leader";
        private const string HELP = "Transfer party leader to another member.";
        private const string SYNTAX = "<player>";
        private const bool ALLOW_FROM_CONSOLE = false;
        private const bool RUN_FROM_CONSOLE = false;

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

        public List<string> Aliases { get; } = new List<string> {"pleader"};

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
                DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/leader PlayerName\".");
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

            var party = Parties.getParty(caller);
            if (party != null)
                party.makeLeader(caller, player);
            else
                DGPlugin.messagePlayer(caller, "You are not in a party.");
        }
    }
}