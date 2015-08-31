using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Party
{
    public class CommandChat : IRocketCommand
    {
        private const string NAME = "p";
        private const string HELP = "Send a message to your party.";
        private const string SYNTAX = "<message>";
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

        public List<string> Aliases { get; } = new List<string> {"party", "pchat", "partychat"};

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
            //check message
            if (command.Length > 0)
            {
                Parties.toggleChat(caller, true);

                var message = string.Join(" ", command);
                var party = Parties.getParty(caller);
                if (party != null)
                    party.chat(caller, message);
                else
                    DGPlugin.messagePlayer(caller, "You are not in a party.");
            }
            else
                Parties.toggleChat(caller);
        }
    }
}