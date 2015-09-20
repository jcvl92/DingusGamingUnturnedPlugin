using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace DingusGaming.Store
{
    public class CommandBuyVehicle : IRocketCommand
    {
        private const int cost = 10;
        private const int cooldown = 5 * 60;
        private const string NAME = "buycar";
        private const string HELP = "Purchase a vehicle.";
        private const string SYNTAX = "<vehicleID>";
        private const bool ALLOW_FROM_CONSOLE = false;
        private const bool RUN_FROM_CONSOLE = false;
        private static readonly Dictionary<CSteamID, float> lastPurchase = new Dictionary<CSteamID, float>();

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
                    if (!lastPurchase.ContainsKey(caller.CSteamID) ||
                        Time.realtimeSinceStartup - lastPurchase[caller.CSteamID] > cooldown)
                    {
                        if (caller.GiveVehicle(vehicleID))
                        {
                            InteractableVehicle vehicle = VehicleManager.vehicles[VehicleManager.vehicles.Count - 1];
                            vehicle.askFill(ushort.MaxValue);
                            vehicle.askRepair(ushort.MaxValue);
                            Currency.changeBalance(caller, -cost);
                            DGPlugin.messagePlayer(caller,
                                "Purchased vehicle #" + vehicle.name + " for " + cost +
                                " credits. Your new balance is $" + Currency.getBalance(caller) + ".");

                            lastPurchase[caller.CSteamID] = Time.realtimeSinceStartup;
                        }
                        else
                            DGPlugin.messagePlayer(caller, "Invalid vehicleID.");
                    }
                    else
                    {
                        DGPlugin.messagePlayer(caller, "You cannot purchase another vehicle for "+ (int)(cooldown - (Time.realtimeSinceStartup - lastPurchase[caller.CSteamID]))+" more seconds!");
                    }
                }
                else
                    DGPlugin.messagePlayer(caller, "Insufficient funds($" + Currency.getBalance(caller) + "/$"+cost+")!");
            }
        }
    }
}