namespace DingusGaming.Events
{
    public class ScheduledEvent
    {
        Timer startTimer = new Timer(), endTimer = new Timer();
        Event e;

    	public ScheduledEvent(Event e, int intervalMinutes, int durationSeconds)
        {
            this.e = e;

            startTimer.Interval = intervalMinutes * 60000;
            startTimer.Elapsed += delegate
            {
                e.startEvent();
                endTimer.Start();
            };

            endTimer.Interval = durationSeconds * 1000;
            endTimer.AutoReset = false;
            endTimer.Elapsed += delegate
            {
                e.stopEvent();
            };
        }

        public ScheduledEvent(Event e, int intervalMinutes)
        {
            this.e = e;

            startTimer.Interval = intervalMinutes * 60000;
            startTimer.Elapsed += delegate
            {
                e.startEvent();
            };
        }

        ~ScheduledEvent()
        {
            startTimer.Close();
            endTimer.Close()
        }

    	public void beginRecurrence()
        {
            startTimer.Start();
        }

        public void stopRecurrence()
        {
            startTimer.Stop();
        }

    	public void toString()
        {
            return "["+e.toString()+"] every "+(startTimer.Interval/60000)+"m"+
                (!endTimer.AutoReset ? " for "+(endTimer.Interval/1000)+"s" : "");
        }
    }
}