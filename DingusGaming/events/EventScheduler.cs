using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;

namespace DingusGaming.Events
{
    public class EventScheduler
    {
        private static readonly List<ScheduledEvent> scheduledEvents = new List<ScheduledEvent>();

    	public static int scheduleEvent(Event newEvent, uint intervalMinutes, bool snapToHour=false, ushort minimumPlayers=0, uint durationSeconds=0, uint[] countDownTimes=null)
    	{
            ScheduledEvent scheduledEvent = (durationSeconds == 0 ? new ScheduledEvent(newEvent, intervalMinutes, minimumPlayers, countDownTimes) : new ScheduledEvent(newEvent, intervalMinutes, minimumPlayers, countDownTimes, durationSeconds));

            if(snapToHour)
            {
                int minuteToStart = (int)(intervalMinutes * (DateTime.Now.Minute/intervalMinutes) + intervalMinutes);
                int timeToWait = (minuteToStart * 60000) - (DateTime.Now.Minute*60000 + DateTime.Now.Second*1000 + DateTime.Now.Millisecond);

                Timer waitTimer = new Timer(timeToWait);
                waitTimer.AutoReset = false;
                waitTimer.Elapsed += delegate
                {
                    scheduledEvent.beginRecurrence();

                    waitTimer.Close();
                };
                waitTimer.Start();
            }
            else
                scheduledEvent.beginRecurrence();

            scheduledEvents.Add(scheduledEvent);

            return scheduledEvents.IndexOf(scheduledEvent);
    	}

        public static void listEvents(UnturnedPlayer player)
        {
            DGPlugin.messagePlayer(player, "Events:");
            for(int i=0; i<scheduledEvents.Count; ++i)
                DGPlugin.messagePlayer(player, (i+1)+":("+scheduledEvents[i].ToString()+")");
        }

        //TODO: make sure calls referencing listEvents do index-1 before calling this
        public static void removeEvent(int index)
        {
            scheduledEvents[index].stopRecurrence();
            scheduledEvents.RemoveAt(index);
        }
    }
}