using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Store
{
    public class CommandStore : IRocketCommand
    {
        private const string NAME = "store";
        private const string HELP = "Access the store.";
        private const string SYNTAX = "(<storeNumber>)";
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

        public List<string> Aliases { get; } = new List<string> {"viewstore", "s", "shop"};

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
            if (command.Length == 0)
                DGPlugin.messagePlayer(caller, Stores.listSubstores());
            else if (command.Length == 1)
            {
                int num;
                if (int.TryParse(command[0], out num))
                    DGPlugin.messagePlayer(caller, Stores.viewSubstore(num));
                else
                    DGPlugin.messagePlayer(caller, "Invalid storeNumber.");
            }
            else
                DGPlugin.messagePlayer(caller,
                    "Invalid amount of parameters. Format is \"/store\" or \"/store storeNumber\".");
        }
    }
}