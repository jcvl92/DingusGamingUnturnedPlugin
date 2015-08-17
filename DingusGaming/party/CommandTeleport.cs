using Rocket.RocketAPI;
using System.Collections.Generic;

namespace DingusGaming.Party
{
    public class CommandTeleport : IRocketCommand
    {
        private const string NAME = "teleport";
        private const string HELP = "Teleport to a party member.";
        private const string SYNTAX = "<player>";
        private readonly List<string> ALIASES = new List<string> { "tp", "pteleport", "ptp", "tpa" };
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
            //check for parameter vaidity
            if (command.Length == 0)
            {
                DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/tp PlayerName\".");
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

            Party party = Parties.getParty(caller);
            if (party != null)
            {
                if (party.isMember(player))
                {
                    caller.Teleport(player);
                }
                else
                    DGPlugin.messagePlayer(caller, player.CharacterName + " is not in your party. You can only teleport to party members.");
            }
            else
                DGPlugin.messagePlayer(caller, "You are not in a party. You can only teleport to party members.");
        }

        //		public void Execute(IRocketPlayer caller, string[] command)
        //		{
        //			Execute((UnturnedPlayer) caller, command);
        //		}
    }
}