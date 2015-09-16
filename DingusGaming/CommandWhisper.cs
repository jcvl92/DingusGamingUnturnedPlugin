using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Party
{
    public class CommandChat : IRocketCommand
    {
        private const string NAME = "w";
        private const string HELP = "Send a private message to a player.";
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

        public List<string> Aliases { get; } = new List<string> {"whisper", "tell", "t"};

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
            if (command.Length >= 2)
            {
                UnturnedPlayer player = DGPlugin.getPlayer(command[0]);
                string message = string.Join(" ", command.Skip(1).ToArray());

                if(player != null)
                {
                    DGPlugin.messagePlayer(caller, player, message);
                    DGPlugin.messagePlayer(player, caller, "TO: "message);
                }
                else
                    DGPlugin.messagePlayer(caller, "No such player \""+command[0]+"\".");
            }
            else
                DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/w playerName message\".");
        }
    }
}