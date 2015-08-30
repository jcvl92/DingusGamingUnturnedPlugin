using Rocket.API;
using System.Collections.Generic;
using DingusGaming.helper;
using Rocket.Unturned.Player;

namespace DingusGaming.Store
{
    public class CommandTest : IRocketCommand
    {
        private const string NAME = "test";
        private const string HELP = "";
        private const string SYNTAX = "";
        private readonly List<string> ALIASES = new List<string> {};
        private const bool ALLOW_FROM_CONSOLE = false;
        private const bool RUN_FROM_CONSOLE = false;
        private readonly List<string> REQUIRED_PERMISSIONS = new List<string>();
        private static PlayerState state = null;

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

        public void Execute(UnturnedPlayer caller, string[] command)
        {
            if (command.Length != 0)
                DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/test\".");
            else if (state == null)
            {
                state = PlayerState.getState(caller);
                DGPlugin.broadcastMessage("Saved player state!");
            }
            else
            {
                state.setCompleteState(caller);
                state = null;
                DGPlugin.broadcastMessage("Loaded player state!");
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Execute((UnturnedPlayer)caller, command);
        }
    }
}