using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Store
{
    public class CommandBalance : IRocketCommand
    {
        private const string NAME = "balance";
        private const string HELP = "View your credit balance.";
        private const string SYNTAX = "";
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

        public List<string> Aliases { get; } = new List<string>
        {
            "bank",
            "wallet",
            "viewwallet",
            "viewbalance",
            "viewbank"
        };

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
            if (command.Length > 0)
                DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/balance\".");
            else
                DGPlugin.messagePlayer(caller, "You currently have " + Currency.getBalance(caller) + " credits.");
        }
    }
}