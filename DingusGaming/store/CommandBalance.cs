using Rocket.RocketAPI;
using System.Collections.Generic;

namespace DingusGaming.Store
{
    public class CommandBalance : IRocketCommand
    {
        private const string NAME = "balance";
        private const string HELP = "View your credit balance.";
        private const string SYNTAX = "";
        private readonly List<string> ALIASES = new List<string> { "bank", "wallet", "viewwallet", "viewbalance", "viewbank" };
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
            if (command.Length > 0)
                DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/balance\".");
            else
                DGPlugin.messagePlayer(caller, "You currently have " + Currency.getBalance(caller) + " credits.");
        }

        //        public void Execute(RocketPlayer caller, string[] command)
        //        {
        //            Execute((UnturnedPlayer)caller, command);
        //        }
    }
}