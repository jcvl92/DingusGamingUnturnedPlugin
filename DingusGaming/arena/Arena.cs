/*using System.Collections.Generic;
using System.Timers;
using DingusGaming;
using Steamworks;
using Rocket.API;
using SDG;
using Rocket.RocketAPI.Events;
using Rocket.Unturned;
using Rocket.Unturned.Events;

namespace Arena
{
    public class ArenaEvent
    {
        public Timer timer;
        public bool adminsIncluded;
        private string location, holdingArea;
        private List<TeleportInfo> teleports = new List<TeleportInfo>();

        public ArenaEvent(ushort eventLength = 120, byte startItem = 0,
            byte dropItem = 0, bool adminsIncluded = false)
        {
            this.location = "Oulton's Isle";
            holdingArea = "Pirate Cave";
            this.adminsIncluded = adminsIncluded;

            //newly connecting players are put in the holding area
            U.Events.OnPlayerConnected += delegate (UnturnedPlayer player)
            {
                // addToTeleports(player); // TODO
                moveToHoldingArea(player);
            };

            //disable all user commands during event
            DGPlugin.disableCommands();

            //create the timer to stop the event if the max time has been reached
            timer = new Timer((double)eventLength * 1000);
            timer.AutoReset = false;
            timer.Elapsed += delegate (object source, ElapsedEventArgs e)
            {
                stopArena();
            };

            //hook in player death event
            UnturnedPlayerEvents.OnPlayerDeath += onPlayerDeath;
        }

        ~ArenaEvent()
        {
            timer.Close();
        }

        private void addToTeleports(UnturnedPlayer player)
        {
            new UnturnedPlayer().Inventory.Items
        }

        private void moveToHoldingArea(UnturnedPlayer player)
        {
            //isolate player so they can watch and be out of the way
            //move them to a holding location(spawn location but in the sky a little bit - enough to not get in the way)
            //prevent them from moving(can still rotate)
            //give them god-mode and vanish-mode
        }

        /* TODO: Disabled because what is alive?
        private void onPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            //TODO: is this stuff thread safe?
            //TODO: have suiciding cause a decrement in score rather than increment(pending method evaluation in Currency)
            moveToHoldingArea(player);

            //remove from alive list
            alive.Remove(player.CSteamID);

            //update score of killing player
            ++score[murderer];

            DGPlugin.broadcastMessage(player.CharacterName + " has been killed by " +
                                      DGPlugin.getPlayer(murderer).CharacterName + ".");

            //see if 1 or 0 people are left alive(to end the event)
            if (alive.Count == 0)
            {
                DGPlugin.broadcastMessage("Everyone has died!");
                stopArena();
            }
            else if (alive.Count == 1)
            {
                DGPlugin.broadcastMessage(DGPlugin.getPlayer(murderer).CharacterName + " is the last man standing!");
                stopArena();
            }
            else
                DGPlugin.broadcastMessage(alive.Count + " players left!");
        }

        public void beginArena()
        {
            //remember to check the adminsIncluded flag
            //store player states(inventory, location, experience, and skill trees)
            //compile player alive list(unique ID, removed from list on death)
            //create player score list(unique ID, score(0))

            //register onDeath event handler

            //drop starting items on location

            //clear player inventories
            //heal up all survival stats on players
            //give players god-mode and vanish-mode
            //give players starting item if present
            //teleport players to location(remember to add them to the teleports list before TPing them)

            //start 10 second timer that will remove god-mod and vanish-mode

            //start event timer
            timer.Start();
        }

        private void stopArena()
        {
            //stop event timer if still going
            timer.Stop();

            //unhook player death event
            UnturnedPlayerEvents.OnPlayerDeath -= onPlayerDeath;

            //notify everyone of how many people they killed/credits they earned/what place they earned out of everyone(e.g. 4/10, 4th highest score)

            //at the end of the arena, the top 3 people are allowed to choose a prize, scores are published to chat(and credits earned)
            //this way, you don't have to kill everyone and if someone just hides or leaves, they will only impact themselves

            //prizes chosen are announced
            // /prize will be used to claim a prize

            //re-enable commands
            DGPlugin.enableCommands();
        }

        class TeleportInfo
        {

        }
    }
}*/