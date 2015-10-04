#DingusGaming Unturned Rocket Plugin

This is my unfinished Rocket plugin for Unturned. I put a lot of work into it, but ultimately found that Unturned was not robust or powerful enough to handle any serious mods to the game. For that reason, I gave up on development after the Arena feature was finished. The Arena works, but will often crash or silently fail and cause people to lose their gear, or bug them in other ways. The codebase is solid, but Unturned simply is not able to handle the level of processing required to make Arena work well. I originally wanted to make Arena into Gun Game, but it was very clear that would not be possible.

**Some interesting developments that developers might be interested in:**
* Teleport functions remove players from cars before teleporting them.
* Teleporting to a player in a vehicle puts you in their vehicle instead of TPing and probably getting your run over. If vehicle has no space for you, it puts you behind the vehicle.
* teleportPlayerInRadius() will teleport players to a random point within a circle, leveling them with the ground so they do not get teleported into the ground/in the air.
* Skills reduction on death is reduced to -10% instead of -$25%. This was done by removing the normal update delegate, and putting my own copy in with the local variable changed.
* Structure health was programmatically changed to be 10x. This is scalable and much easier than editing the structure config files.
* Players can be respawned, bypassing the respawn timer, by using the respawnPlayer() function.
* Inventory/skills&experience/location/survival stats can all be saved and/or cleared out using the PlayerState class. These functions are fairly robust, but can operate a little weird at scale as the underlying Unturned operations are not scalable or robust, themselves.
* I used a lot of reflection to edit private variables and access invocation lists to replace delegates.
* In order to auto-respawn players on death, I made an onPlayerDeath delegate that registered an onPlayerDied delegate that would respawn the player. This is because there are two different events that happen when a player dies, onDeath and onDied. onDeath happens before the death has fully registered, and onDied happens after the death has registered, when a respawn call would be valid. A similar flow happens when respawning a player and modifying their state. See the CommandUnstuck and ArenaEvent classes for examples.

**Features include:**
* Lowered skills reduction on death.
* Disabled dropping of loadout items to reduce server clutter.
* Store/Currency system
  * When you kill people, you get credits based on how long they were alive and how many people they killed.
  * You can buy things from the store using credits.
    * The store is defined in an .xml file the lists the prices, names, and IDs of items that can be bought in configurable sub-stores.
  * Players can also buy vehicles.
  * Players can buy/sell experience points.
* Party system
  * Players can create/join parties.
  * Parties allow players to:
    * Privately chat with the party.
    * Teleport to other party members.
      * **Players are able to teleport in and out of vehicles with this.**
    * See when their party members are killed or when they kill someone.
    * Party members cannot damage each other.
      * This works by replacing Steam Groups with fake Steam Groups generated for each party. Therefore, Steam Groups do not work with this feature.
* Schedulable events
  * Server tips every 5 minutes.
    * These are hard coded in the TipsEvent class because I hadn't gotten around to reading them in from a file.
  * **Arena Event every 30 minutes.**
    * Players' complete states(skill/experience levels, inventory, current survival stats) are saved and cleared. Then they are all teleported to a random location in the Arena location pool. They are given katanas and a bunch of pistols are dropped in the middle of the Arena location(these items are configurable). There is a 3 second timer before players appear to each other. Arena lasts 1 minute(configurable). At the end of Arena, everyone's kills/deaths/rank is reported to them, and their states are completely restored.
    * The benefit of arena is that it allows players to farm a good amount of credits every 30 minutes.
    * If a lot of players are on the server(more than ~16) then players will timeout when the arena begins/ends. So the server timeout needs to be increased(I just maxed it out on my server).
    * Even with the controls in place, the Unturned code base is so unreliable that Arena will never be stable without seriously compromising it's functionality.
