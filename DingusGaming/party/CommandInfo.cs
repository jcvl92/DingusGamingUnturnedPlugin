using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Party
{
    public class CommandInfo : IRocketCommand
    {
        private const string NAME = "info";
        private const string HELP = "Get info on your party or a party member.";
        private const string SYNTAX = "(<player>)";
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

        public List<string> Aliases { get; } = new List<string> {"pinfo", "partyinfo", "inf", "pinf"};

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
            var playerName = string.Join(" ", command);

            var party = Parties.getParty(caller);
            if (party != null)
            {
                //get info on the whole party
                if (command.Length == 0)
                {
                    var info = party.getInfo();
                    DGPlugin.messagePlayer(caller, info);
                }

                //get info on a member
                else
                {
                    //check for player existence
                    var player = DGPlugin.getPlayer(playerName);
                    if (player == null)
                    {
                        DGPlugin.messagePlayer(caller, "Failed to find player named \"" + playerName + "\"");
                        return;
                    }

                    if (party.isMember(player))
                    {
                        var info = "Name: " + player.CharacterName + ", " +
                                   (player.Dead
                                       ? "Player is dead."
                                       : "Health: " + player.Health + ", " +
                                         "Hunger: " + player.Hunger + ", " +
                                         "Thirst: " + player.Thirst + ", " +
                                         "Infection: " + player.Infection);

                        DGPlugin.messagePlayer(caller, info);
                    }
                    else
                        DGPlugin.messagePlayer(caller,
                            player.CharacterName + " is not in your party. You can only get info on party members.");
                }
            }
            else
                DGPlugin.messagePlayer(caller, "You are not in a party. You can only get info on party members.");
        }
    }
}