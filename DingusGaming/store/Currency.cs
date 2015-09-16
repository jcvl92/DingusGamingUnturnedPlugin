using System;
using System.Collections;
using System.Collections.Generic;
using DingusGaming.Events.Arena;
using DingusGaming.Party;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace DingusGaming.Store
{
    public class Currency
    {
        private static readonly int startingAmount = 50;
        private static Dictionary<string, int> balances;
        public static bool showCreditEarnings = true;
        private static readonly Dictionary<CSteamID, int> killsSinceSpawn = new Dictionary<CSteamID, int>();

        public static void init()
        {
            loadBalances();
            registerOnServerShutdown();
            registerPlayerOnConnected();
            registerOnPlayerDeath();
        }

        private static void loadBalances()
        {
            var temp = DGPlugin.readFromFile<List<DictionaryEntry>>("balances.xml");
            if (temp != null)
                balances = DGPlugin.convertToDictionary<string, int>(temp);
            else
                balances = new Dictionary<string, int>();
        }

        public static void saveBalances()
        {
            DGPlugin.writeToFile(DGPlugin.convertFromDictionary(balances), "balances.xml");
        }

        private static void registerOnPlayerDeath()
        {
            UnturnedPlayerEvents.OnPlayerDeath +=
                delegate(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
                {
                    // Grant the killing user 5 credits + 10% of their victim's credits
                    var killer = DGPlugin.getKiller(player, cause, murderer);
                    if (killer != null &&
                        (Parties.getParty(player) == null || !Parties.getParty(player).isMember(killer)))
                    {
                        var amount = valueOfPlayer(player);

                        //clear the victim's kills since spawn
                        killsSinceSpawn.Remove(player.CSteamID);

                        //add to the killer's kills since spawn
                        if (killsSinceSpawn.ContainsKey(killer.CSteamID))
                            killsSinceSpawn[killer.CSteamID]++;
                        else
                            killsSinceSpawn.Add(killer.CSteamID, 1);

                        changeBalance(killer, amount);

                        if (showCreditEarnings)
                        {
                            DGPlugin.messagePlayer(killer,
                                "You earned $" + amount + " from killing " + player.CharacterName + ".");
                            DGPlugin.messagePlayer(player,
                                killer.CharacterName + " got $" + amount + " from killing you.");
                        }
                    }
                };
        }

        public static int valueOfPlayer(UnturnedPlayer player)
        {
            int minutesAlive = (int) (Time.realtimeSinceStartup-player.Player.PlayerLife.lastRespawn)/60;
            int playersKilledSinceSpawn = (killsSinceSpawn.ContainsKey(player.CSteamID) ? killsSinceSpawn[player.CSteamID] : 0);
            int valueOfPlayer = Math.Min(minutesAlive, 10) + playersKilledSinceSpawn * 5;

            if (ArenaEvent.isOccurring)
                return Math.Max(valueOfPlayer, 10);

            return valueOfPlayer;

        }

        private static void registerOnServerShutdown()
        {
            Steam.OnServerShutdown += delegate { saveBalances(); };
        }

        private static void registerPlayerOnConnected()
        {
            U.Events.OnPlayerConnected += addPlayer;
        }

        public static void addPlayer(UnturnedPlayer player)
        {
            if (!balances.ContainsKey(DGPlugin.getConstantID(player)))
                balances.Add(DGPlugin.getConstantID(player), startingAmount);
        }

        public static void changeBalance(UnturnedPlayer player, int amount)
        {
            balances[DGPlugin.getConstantID(player)] += amount;
        }

        public static int getBalance(UnturnedPlayer player)
        {
            return balances[DGPlugin.getConstantID(player)];
        }

        public static bool transferCredits(UnturnedPlayer from, UnturnedPlayer to, int amount)
        {
            string src = DGPlugin.getConstantID(from), dest = DGPlugin.getConstantID(to);
            if (amount > 0 && balances[src] >= amount)
            {
                balances[src] -= amount;
                balances[dest] += amount;
                return true;
            }
            return false;
        }
    }
}