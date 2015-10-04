#DingusGaming

This is my unfinished Rocket plugin for Unturned. I put a lot of work into it, but ultimately found that Unturned was not robust or powerful enough to handle any serious mods to the game. For that reason, I gave up on development after the Arena feature was finished. The Arena works, but will often crash or silently fail and cause people to lose their gear, or bug them in other ways. The codebase is solid, but Unturned simply is not able to handle the level of processing required to make Arena work well.

**Features include:**
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
    * These are hardcoded in the TipsEvent class because I hadn't gotten around to reading them in from a file.
  * **Arena Event every 30 minutes.**
    * Players' complete states(skill/experience levels, inventory, current survival stats) are saved and cleared. Then they are all teleported to a random location in the Arena location pool. They are given katanas and a bunch of pistols are dropped in the middle of the Arena location(these items are configurable). There is a 3 second timer before players appear to each other. Arena lasts 1 minute(configurable). At the end of Arena, everyone's kills/deaths/rank is reported to them, and their states are completely restored.
    * The benefit of arena is that it allows players to farm a good amount of credits every 30 minutes.
    * If a lot of players are on the server(more than ~16) than players will timeout when the arena begins/ends. So the server timeout needs to be increased(I just maxed it out on my server).
    * 
