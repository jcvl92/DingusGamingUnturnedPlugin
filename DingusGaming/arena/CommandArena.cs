using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Arena
{
    public class CommandArena : IRocketCommand
    {
        private const string NAME = "arena";
        private const string HELP = "Start an arena event.";
        private const string SYNTAX = "<location>";
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

        public List<string> Aliases { get; } = new List<string> {"startarena", "beginarena"};

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
            {
                ArenaEvent ae = null; //new ArenaEvent("Oulton's Isle", startItem: 1036, eventLength: 30);
                ae.beginArena();
            }
            else
            {
                //TODO: check for valid location name before creating arena and add specification of startItem
                ArenaEvent ae = null; //new ArenaEvent(string.Join(" ", command), startItem: 1036);
                ae.beginArena();
            }
        }
    }
}