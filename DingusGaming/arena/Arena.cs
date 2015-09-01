using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using DingusGaming.helper;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace DingusGaming.Arena
{
    public class ArenaEvent
    {
        public static ArenaEvent currentEvent;
        private readonly bool adminsIncluded;
        private readonly Dictionary<CSteamID, int> scores = new Dictionary<CSteamID, int>();
        private readonly ushort startItem;
        private readonly Dictionary<CSteamID, PlayerState> states = new Dictionary<CSteamID, PlayerState>();
        private readonly Timer timer;
        private readonly Vector3 location;
        private readonly float rotation;

        public ArenaEvent(Vector3 location, float rotation, ushort eventLength = 120, ushort startItem = 0,
            byte dropItem = 0, bool adminsIncluded = true)
        {
            this.adminsIncluded = adminsIncluded;
            this.startItem = startItem;
            this.location = location;
            this.rotation = rotation;

            //disable all user commands during event
            DGPlugin.disableCommands();

            //create the timer to stop the event when the max time has been reached
            timer = new Timer((double) eventLength*1000);
            timer.AutoReset = false;
            timer.Elapsed += delegate { stopArena(); };
        }

        ~ArenaEvent()
        {
            timer.Close();
        }

        private void onPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            murderer = DGPlugin.getKiller(player, cause, murderer).CSteamID;
            //update score of killing player
            if (murderer != null)
                scores[murderer]++;

            //clear their inventory so that they don't drop anything
            PlayerState.clearInventory(player);

            //respawn player and teleport them back
            DGPlugin.respawnPlayer(player);
            player.Teleport(location);
        }

        public void beginArena()
        {
            //remember to check the adminsIncluded flag
            foreach (var plr in Steam.Players)
            {
                var player = DGPlugin.getPlayer(plr.playerID.CSteamID);

                //skip admins
                if (!player.IsAdmin && adminsIncluded)
                    continue;

                //add player to scores list
                scores.Add(player.CSteamID, 0);

                //save player state
                states.Add(player.CSteamID, PlayerState.getState(DGPlugin.getPlayer(player.CSteamID)));

                //clear player inventory
                PlayerState.clearInventory(player);

                //heal up all survival stats
                PlayerState.clearStats(player);

                //give players vanish-mode
                player.Features.VanishMode = true;

                //give players starting item if present
                if (startItem != 0)
                {
                    player.GiveItem(startItem, 1);
                    player.Player.Equipment.equip(2, 1, 1);
                }

                //teleport player to arena location
                player.Teleport(location, rotation);
            }

            //TODO: drop starting items on location

            //hook in player death event
            UnturnedPlayerEvents.OnPlayerDeath += onPlayerDeath;

            //start 10 second timer that will remove vanish-mode
            var vanishTimer = new Timer(5000);
            vanishTimer.AutoReset = false;
            vanishTimer.Elapsed += delegate
            {
                foreach (var score in scores)
                    DGPlugin.getPlayer(score.Key).Features.VanishMode = false;
            };
            vanishTimer.Start();

            //start event timer
            timer.Start();
        }

        private void stopArena()
        {
            //unhook player death event
            UnturnedPlayerEvents.OnPlayerDeath -= onPlayerDeath;

            //restore the player states
            foreach (var state in states)
            {
                if(player.Player.life.isDead())
                    DGPlugin.respawnPlayer(player);

                state.Value.setCompleteState(DGPlugin.getPlayer(state.Key));
            }

            //notify everyone of how many people they killed/what place they earned out of everyone(e.g. 4/10, 4th highest score)
            var list = scores.Values.ToList();
            list.Sort();
            foreach (var score in scores)
                DGPlugin.messagePlayer(DGPlugin.getPlayer(score.Key),
                    "Arena has finished. You killed " + score.Value + " people! You earned place " + getPlace(score.Value) + "/" + scores.Count + "!");

            //re-enable commands
            DGPlugin.enableCommands();
        }

        private int getPlace(int score)
        {
            for(int i=0; i<scores.Count; ++i)
                if(scores[i] == score)
                    return i+1;
            return -1;
        }
    }
}