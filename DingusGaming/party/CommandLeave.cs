using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Party
{
    public class CommandLeave : IRocketCommand
    {
        private const string NAME = "leave";
        private const string HELP = "Leave your current party.";
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

        public List<string> Aliases { get; } = new List<string> {"pleave", "leaveparty", "quit", "pquit"};

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
            var party = Parties.getParty(caller);

            if (party != null)
            {
                party.removeMember(caller);
                party.tellParty(caller.CharacterName + " has left the party.");
                DGPlugin.messagePlayer(caller, "You have left the party.");
            }
            else
                DGPlugin.messagePlayer(caller, "You are not in a party.");
        }
    }
}