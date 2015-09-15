namespace DingusGaming.Events
{
    //TODO: move time handling from arena to here
    public class EventScheduler
    {
        private List<Event> events = new List<Event>();
        private list<Timer> timers - new List<Timer>();

    	public static int addEvent(Event newEvent, int frequency, int duration=0)
    	{
            if(frequency<duration)
                return -1;

            //Add the event to the list
            events.Add(newEvent);

            //spawn a new timer for the event

    	}

        public static string listEvents()
        {

        }

        public static void removeEvent(int index)
        {
            
        }
    }
}