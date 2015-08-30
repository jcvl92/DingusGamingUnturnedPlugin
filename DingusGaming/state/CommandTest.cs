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
        private const string SYNTAX = "<playerName>";
        private readonly List<string> ALIASES = new List<string> {};
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
            if (command.Length != 1)
                DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/test playerName\".");
            else
            {
                UnturnedPlayer subject = UnturnedPlayer.FromName(command[0]);

                PlayerState subjectState = PlayerState.getState(subject);
                PlayerState callerState = PlayerState.getState(caller);

                subjectState.setCompleteState(caller);
                callerState.setCompleteState(subject);

                DGPlugin.broadcastMessage("Swapped "+subject.CharacterName+" with "+caller.CharacterName+"!");
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Execute((UnturnedPlayer)caller, command);
        }
    }
}