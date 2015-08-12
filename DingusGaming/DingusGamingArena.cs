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
		//arena event will be a timed event that consists of multiple sub-events

		//the winner gets to choose a prize(out of 5 prizes)
		//once the currency system is up, everyone will be getting coins for their arena kills, too

		//admins are not included in the event(after beta-testing)

		//a list is generated when the event started, it contains everyone on the server(minus admins).
		//on death, players are removed from the list, given god-mode and vanish-mode, teleported to a holding cell in the sky where they can watch,
		//and the list is checked for size 1 to determine if someone won

		/*
		event flow is as follows:
		0. Admin creates event by running command /arena <location>
		1. Server broadcasts the impending arena 1 minute before it happens(mentions it at 1 minute, 30 secondss, and then a countdown from 10 seconds)
		2. At the end of the countdown, the inventory, location, experience, and skill tress of everyone is saved in memory(maybe dump this to file for safety, too).
		3. Everyone's inventory is cleared out.
		4. Everyone is given god-mode and vanish-mode.
		5. Everyone is teleported to the arena location.
		6. Guns are spawned in front of the players.
		7. After 10 seconds, players will have god-mode and vanish-mode disabled.
		8. After 2 minutes, if there is no winner, the event will run again(just skipping to step 4)

		9. When a winner is determined(last man standing), everyone is teleported to their previous locations, given their gear, skills, and experience back.
		10. The winner is broadcast to all players on the server
		11. The winner is presented with 5 options to pick for a prize.(the options will be a random subset of a large pool of options)
		12. The winner will use command /prize <number> to pick a prize.(prizes will include lump sum currency, weapons, and weapon attachments)
		13. The prize the winner picked will be broadcast to all players on the server(e.g. "Congrats to X for winning Y!")
		*/

		/*
		controls during the event are as follows:
		1. People joining the server are placed in the holding cage.
		2. People who die are placed in the holding cage.
		3. People who go significantly far enough away from the event are placed in the holding cage.
		4. When people die, a winner-check function is run.
		*/
	}
}