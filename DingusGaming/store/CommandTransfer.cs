using Rocket.RocketAPI;
using System.Collections.Generic;
using System.Linq;

namespace DingusGaming.Store
{
    public class CommandTransfer : IRocketCommand
    {
        private const string NAME = "transfer";
        private const string HELP = "Transfer credits to another player";
        private const string SYNTAX = "<amount> <player name>";
        private readonly List<string> ALIASES = new List<string> { "gift", "giftcredits", "transfercredits", "sendcredits" };
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
            if (command.Length < 2)
                DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/transfer amount playerName\".");
            else
            {
                int amount;
                if (!int.TryParse(command[0], out amount))
                    DGPlugin.messagePlayer(caller, "Invalid amount.");
                else
                {
                    string playerName = string.Join(" ", command.Skip(1).ToArray());
                    RocketPlayer player;
                    if ((player = DGPlugin.getPlayer(playerName)) == null)
                        DGPlugin.messagePlayer(caller, "Failed to find player named \"" + playerName + "\"");
                    else
                    {
                        if (Currency.transferCredits(caller, player, amount))
                        {
                            DGPlugin.messagePlayer(caller, "You sent " + amount + " credits to " + player.CharacterName + ".");
                            DGPlugin.messagePlayer(player, player.CharacterName + " just gave you $" + amount + "!");
                        }
                        else
                            DGPlugin.messagePlayer(caller, "You cannot send negative credits!");
                    }
                }
            }
        }

//        public void Execute(IRocketPlayer caller, string[] command)
//        {
//            Execute((UnturnedPlayer)caller, command);
//        }
    }
}