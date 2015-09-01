using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Arena
{
    public class CommandArena : IRocketCommand
    {
        private const string NAME = "arena";
        private const string HELP = "Start/set the location for an arena event.";
        private const string SYNTAX = "<start|set>";
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

        public List<string> Aliases { get; } = new List<string>();

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
            if (command.Length != 1 || !(command[0].Equals("set") || command[0].Equals("start")))
            {
                DGPlugin.messagePlayer(caller, "Incorrect format. Format is \"arena set\" or \"arena start\".");
            }
            else
            {
                //TODO: add specification of drop item/start item
                if(command[0].Equals("set"))
                    ArenaEvent.currentEvent = new ArenaEvent(caller.Position, caller.Rotation, startItem: 519/*1036*/, /*TODO: remove this*/eventLength: 30);
                else
                    ArenaEvent.currentEvent.beginArena();
            }
        }
    }
}