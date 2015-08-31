using System.Collections.Generic;
using DingusGaming.Arena;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Party
{
    public class CommandArena : IRocketCommand
    {
        private const string NAME = "arena";
        private const string HELP = "Start an arena event.";
        private const string SYNTAX = "<location>";
        private readonly List<string> ALIASES = new List<string> { "startarena", "beginarena" };
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

        public void Execute(UnturnedPlayer caller, string[] command)
        {
            if (command.Length == 0)
                new ArenaEvent("Oulton's Isle", startItem:1036, eventLength:30).beginArena();
            else
                //TODO: check for valid location name before creating arena and add specification of startItem
                new ArenaEvent(string.Join(" ", command), startItem: 1036).beginArena();
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Execute((UnturnedPlayer)caller, command);
        }
    }
}