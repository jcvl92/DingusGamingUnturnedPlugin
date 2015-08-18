using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned.Player;

namespace DingusGaming.Party
{
    public class CommandLeave : IRocketCommand
    {
        private const string NAME = "leave";
        private const string HELP = "Leave your current party.";
        private const string SYNTAX = "";
        private readonly List<string> ALIASES = new List<string> { "pleave", "leaveparty", "quit", "pquit" };
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
            Party party = Parties.getParty(caller);

            if (party != null)
            {
                party.removeMember(caller);
                party.tellParty(caller.CharacterName + " has left the party.");
                DGPlugin.messagePlayer(caller, "You have left the party.");
            }
            else
                DGPlugin.messagePlayer(caller, "You are not in a party.");
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Execute((UnturnedPlayer)caller, command);
        }
    }
}