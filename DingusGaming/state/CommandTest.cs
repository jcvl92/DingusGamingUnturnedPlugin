using System.Collections.Generic;
using DingusGaming.helper;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Store
{
    public class CommandTest : IRocketCommand
    {
        private const string NAME = "test";
        private const string HELP = "";
        private const string SYNTAX = "";
        private const bool ALLOW_FROM_CONSOLE = false;
        private const bool RUN_FROM_CONSOLE = false;
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
            if (command.Length != 1)
                DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/test playerName\".");
            else
            {
                var subject = UnturnedPlayer.FromName(command[0]);

                var subjectState = PlayerState.getState(subject);
                var callerState = PlayerState.getState(caller);

                subjectState.setCompleteState(caller);
                callerState.setCompleteState(subject);

                DGPlugin.broadcastMessage("Swapped " + subject.CharacterName + " with " + caller.CharacterName + "!");
            }
        }
    }
}