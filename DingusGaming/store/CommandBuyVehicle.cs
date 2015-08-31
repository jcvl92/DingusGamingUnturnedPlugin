using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace DingusGaming.Store
{
    public class CommandBuyVehicle : IRocketCommand
    {
        private const int cost = 25;
        private const string NAME = "buycar";
        private const string HELP = "Purchase a vehicle.";
        private const string SYNTAX = "<vehicleID>";
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

        public List<string> Aliases { get; } = new List<string> {"purchasecar", "buyvehicle", "purchasevehicle", "buyv"}
            ;

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
                DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/buycar vehicleID\".");
            else
            {
                ushort vehicleID;

                if (!ushort.TryParse(command[0], out vehicleID))
                    DGPlugin.messagePlayer(caller, "Invalid vehicleID.");
                else if (Currency.getBalance(caller) >= cost)
                {
                    if (caller.GiveVehicle(vehicleID))
                    {
                        VehicleManager.vehicles[VehicleManager.vehicles.Count - 1].askFill(ushort.MaxValue);
                        Currency.changeBalance(caller, -cost);
                    }
                    else
                        DGPlugin.messagePlayer(caller, "Invalid vehicleID.");
                }
                else
                    DGPlugin.messagePlayer(caller, "Insufficient funds($" + Currency.getBalance(caller) + "/$25).");
            }
        }
    }
}