using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned.Player;

namespace DingusGaming.Store
{
    public class CommandBuyVehicle : IRocketCommand
    {
        private const int cost = 25;
        private const string NAME = "buycar";
        private const string HELP = "Purchase a vehicle.";
        private const string SYNTAX = "<vehicleID>";
        private readonly List<string> ALIASES = new List<string> { "purchasecar", "buyvehicle", "purchasevehicle", "buyv" };
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
                DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/buycar vehicleID\".");
            else
            {
                ushort vehicleID;

                if (!ushort.TryParse(command[0], out vehicleID))
                    DGPlugin.messagePlayer(caller, "Invalid vehicleID.");
                else if (Currency.getBalance(caller) >= cost)
                {
                    if(caller.GiveVehicle(vehicleID))
                        Currency.changeBalance(caller, -cost);
                    else
                        DGPlugin.messagePlayer(caller, "Invalid vehicleID.");
                }
                else
                    DGPlugin.messagePlayer(caller, "Insufficient funds($"+Currency.getBalance(caller)+"/$25).");
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Execute((UnturnedPlayer)caller, command);
        }
    }
}