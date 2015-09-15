namespace DingusGaming.Events
{
    public class EventScheduler
    {
        private static List<ScheduledEvent> scheduledEvents = new List<ScheduledEvent>();

    	public static int scheduleEvent(Event newEvent, int intervalMinutes, bool snapToHour=false, int durationSeconds=0)
    	{
            ScheduledEvent scheduledEvent = (durationSeconds <= 0 ? new ScheduledEvent(newEvent, intervalMinutes) : new ScheduledEvent(newEvent, intervalMinutes, durationSeconds));

            if(snapToHour)
            {
                int minuteToStart = (intervalMinutes * (DateTime.Now.Minute/intervalMinutes + 1))%60;
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

        public static string listEvents()
        {
            StringBuilder sb = new StringBuilder("Events:");
            for(int i=0; i<scheduledEvents.Length; ++i)
            {
                sb.Append(" "+(i+1)+":("+e.toString+")");
            }
            return sb.toString();
        }

        //TODO: make sure calls referencing listEvents do index-1 before calling this
        public static void removeEvent(int index)
        {
            scheduledEvents[index].stopRecurrence();
            scheduledEvents.removeAt(index);
        }
    }
}