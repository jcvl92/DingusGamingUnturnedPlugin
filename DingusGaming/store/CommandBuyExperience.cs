using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Store
{
    public class CommandBuyExperience : IRocketCommand
    {
        private const int cost = 1;
        private const string NAME = "buyexp";
        private const string HELP = "Purchase experience points.";
        private const string SYNTAX = "<amount>";
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

        public List<string> Aliases { get; } = new List<string> {"buyexperience", "buyskill", "buyskills"};

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
                DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/buyskill amount\".");
            else
            {
                uint amount;

                if (!uint.TryParse(command[0], out amount))
                    DGPlugin.messagePlayer(caller, "Invalid amount.");
                else if (Currency.getBalance(caller) >= cost*amount)
                {
                    Currency.changeBalance(caller, -cost*(int) amount);
                    caller.Experience = caller.Experience + amount;
                    DGPlugin.messagePlayer(caller, amount + " experience purchased!");
                }
                else
                    DGPlugin.messagePlayer(caller,
                        "Insufficient funds($" + Currency.getBalance(caller) + "/$" + amount*cost + ").");
            }
        }
    }
}