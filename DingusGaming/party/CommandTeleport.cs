using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Party
{
    public class CommandTeleport : IRocketCommand
    {
        private const string NAME = "teleport";
        private const string HELP = "Teleport to a party member.";
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

        public List<string> Aliases { get; } = new List<string> {"tp", "pteleport", "ptp", "tpa"};

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
                DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/tp PlayerName\".");
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
            {
                if (party.isMember(player))
                {
                    if (!player.Dead)
                        caller.Teleport(player);
                    else
                        DGPlugin.messagePlayer(caller,
                            player.CharacterName + " is dead. You can't teleport to dead players.");
                }
                else
                    DGPlugin.messagePlayer(caller,
                        player.CharacterName + " is not in your party. You can only teleport to party members.");
            }
            else
                DGPlugin.messagePlayer(caller, "You are not in a party. You can only teleport to party members.");
        }
    }
}