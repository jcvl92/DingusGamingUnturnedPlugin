using System;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using DingusGaming;

namespace Arena
{
	public class ArenaEvent
	{
		public Timer timer;
		public bool adminsIncluded;
		public ArenaEvent(ushort eventLength, bool adminsIncluded=false)
		{
			//ingest the location, time to start, event length, item to give everyone, admins included, and item to scatter on the floor

			this.adminsIncluded = adminsIncluded;

			//create the timer to stop the event if the max time has been reached
			timer = new Timer((double)eventLength*1000);
			timer.AutoReset = false;
			timer.Elapsed += new ElapsedEventHandler(
				delegate(object source, ElapsedEventArgs e) 
				{
					stopArena();
				});

			//hook in player death event
			UnturnedPlayerEvents.OnPlayerDeath += onPlayerDeath;
		}

		public ~ArenaEvent()
		{
			timer.Close();
		}

		private void onPlayerDeath()
		{
			//isolate player so they can watch and be out of the way
			//move them to a holding location(spawn location but in the sky a little bit - enough to not get in the way)
			//prevent them from moving(can still rotate)
			//give them god-mode and vanish-mode

			//remove from alive list
			//broadcast players left("X has died. Y players left!")

			//update score of killing player
			//notify killing player of kill and how many credits were earned


			//see if 1 or 0 people are left alive(to end the event)
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
			//give players god-mode and vanish-mode
			//give players starting item if present
			//teleport players to location

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

			//at the end of the arena, the top 3 people are allowed to choose a prize, scores are published to chat(and credits earned)
			//this way, you don't have to kill everyone and if someone just hides or leaves, they will only impact themselves

			//prizes chosen are announced
			// /prize will be used to claim a prize
		}

		//TODO: implement a prize system feature for arbitrary rewarding from admins, this feature, and other possible features

		/*
		controls during the event are as follows:
		1. People joining the server are placed in the holding cage.
		2. People who die are placed in the holding cage.
		3. People who go significantly far enough away from the event are placed in the holding cage.
		4. When people die, a winner-check function is run.
		*/
	}
}