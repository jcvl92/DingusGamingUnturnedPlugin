using System;

namespace DingusGaming.Events
{
	public class TipsEvent : Event
	{
		private static readonly string[] tips = {
			"Visit DingusGaming.com for a list of available commands.",
			"Want to see new features? Donate at DingusGaming.com.",
			"Credits can be spent on items, vehicles, or experience.",
			"Donating and/or voting grants exclusive rewards.",
			"Report bugs on DingusGaming.com for bonus credits.",
			"/tp will put you in your partymember's vehicle, if they are in one.",
			"Arena happens every 30 minutes. Kill people to get credits during it.",
			"Your skills and inventory are restored after Arena events.",
			"/tp has a cooldown of 60 seconds, and 30 seconds after death.",
			"/buyvehicle has a cooldown of 5 minutes. Choose wisely.",
            "You only lose 10% of your experience/skills on this server instead of 25%.",
            "Experience can be sold for credits using /sellexp.",
            "You cannot hurt members of your party(replaces Steam groups).",
            "All structures on this server have 10x health."
		};
		private readonly Random rand = new Random();

		public void startEvent()
		{
			DGPlugin.broadcastMessage("TIP: "+tips[rand.Next(tips.Length)]);
		}

		public void stopEvent()
		{
			//Unused
		}

	    public string countDown(uint secondsLeft)
	    {
	        return "New tip in " + secondsLeft + " seconds!";
	    }

		public override string ToString()
		{
			return "Server Tips";
		}
	}
}