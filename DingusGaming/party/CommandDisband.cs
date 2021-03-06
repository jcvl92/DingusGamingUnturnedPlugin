using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Party
{
    public class CommandDisband : IRocketCommand
    {
        private const string NAME = "disband";
        private const string HELP = "Disband your party.";
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

        public List<string> Aliases { get; } = new List<string> {"disbandparty"};

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
                if (party.isLeader(caller))
                {
                    party.tellParty("Your party has been disbanded!");
                    party.disband();
                }
                else
                {
                    DGPlugin.messagePlayer(caller,
                        "Only the party leader(" + party.getLeader().CharacterName + ") can disband that party.");
                }
            }
            else
                DGPlugin.messagePlayer(caller, "You are not in a party.");
        }
    }
}