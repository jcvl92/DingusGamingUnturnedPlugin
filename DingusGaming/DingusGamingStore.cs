using System;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using DingusGaming;

namespace Store
{
	public class Store
	{
		/*
		commands will be as follows:
		/store - view substores(weapons, building suppplies, ammo+attachments, medical+food+water, vehicle)
		/store <substore> - view substore items(list of less than 10) items have names and IDs
		/purchase <item_ID> (<quantity>) - purchase one or more items with currency
		/balance - shows a user thier current credit balance
		/gift <amount> <playerName> - gifts another player a whole-number amount of credits(round down in casting), cannot be 0 or negative.
		^displays a confirmation, then the user must do /gconfirm or /gcancel
		*/

		/*
		events will be as follows:
		onUnload - balances will be dumped to file
		onLoad - balances will be read from file
		*/
	}

	public class StorePlayerComponent : UnturnedPlayerComponent
    {
        private void FixedUpdate()
        {
        	//death rewards for killing players
            if (this.Player.Dead && !dead)
            {
                dead = true;
                
                //get the killing player
                killer = this.Player.Death.getCause().player

				//grant the killing user 5 credits + 10% of their victim's credits
                Store.addCredits(killer, 5 + Store.getCredits(this.Player)/10);
            }
            if (!this.Player.Dead && dead)
                dead = false;
        }
    }
}