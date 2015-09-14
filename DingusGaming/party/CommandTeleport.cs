using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using Steamworks;
using UnityEngine;

namespace DingusGaming.Party
{
    public class CommandTeleport : IRocketCommand
    {
        private const string NAME = "teleport";
        private const string HELP = "Teleport to a party member.";
        private const string SYNTAX = "<player>";
        private const bool ALLOW_FROM_CONSOLE = false;
        private const bool RUN_FROM_CONSOLE = false;
        private const uint cooldownTime = 60;
        private static readonly Dictionary<CSteamID, float> lastTP = new Dictionary<CSteamID, float>();

        public static void playerDied(UnturnedPlayer player)
        {
            //change their cooldown to half of the normal cooldown, unless their cooldown is already greater than half of the cooldown
            if (!lastTP.ContainsKey(player.CSteamID) || Time.realtimeSinceStartup - lastTP[player.CSteamID] < cooldownTime/2)
                lastTP[player.CSteamID] = Time.realtimeSinceStartup - cooldownTime/2;
        }

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

        public List<string> Aliases { get; } = new List<string> {"tp", "pteleport", "ptp", "tpa"};

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
            //check for parameter vaidity
            if (command.Length == 0)
            {
                DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/tp PlayerName\".");
                return;
            }

            var playerName = string.Join(" ", command);

            //check for player existence
            var player = DGPlugin.getPlayer(playerName);
            if (player == null)
            {
                DGPlugin.messagePlayer(caller, "Failed to find player named \"" + playerName + "\"");
                return;
            }

            var party = Parties.getParty(caller);
            if (party != null)
            {
                if (party.isMember(player))
                {
                    if (!player.Dead)
                    {
                        //add them to the CD list if they aren't in it
                        if (!lastTP.ContainsKey(caller.CSteamID) || Time.realtimeSinceStartup - lastTP[caller.CSteamID] > cooldownTime)
                        {
                            if(DGPlugin.teleportPlayer(caller, player))
                            {
                                lastTP[caller.CSteamID] = Time.realtimeSinceStartup;
                                DGPlugin.messagePlayer(player, caller.CharacterName + " has teleported to you!");
                            }
                            else
                            {
                                DGPlugin.messagePlayer(player, "Could not teleport to " + caller.CharacterName + " because their vehicle is full.");
                            }
                        }
                        //if the cooldown has not passed
                        else
                            DGPlugin.messagePlayer(caller, "Teleport is on cooldown for "+(int)(cooldownTime-(Time.realtimeSinceStartup - lastTP[caller.CSteamID]))+" more seconds.");
                    }
                    else
                        DGPlugin.messagePlayer(caller, player.CharacterName + " is dead. You can't teleport to dead players.");
                }
                else
                    DGPlugin.messagePlayer(caller, player.CharacterName + " is not in your party. You can only teleport to party members.");
            }
            else
                DGPlugin.messagePlayer(caller, "You are not in a party. You can only teleport to party members.");
        }
    }
}