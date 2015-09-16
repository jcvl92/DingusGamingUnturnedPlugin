using System.Timers;

namespace DingusGaming.Events
{
    public class ScheduledEvent
    {
        private readonly Timer startTimer = new Timer(), endTimer = new Timer();
        private readonly Event e;

    	public ScheduledEvent(Event e, int intervalMinutes, int durationSeconds)
        {
            this.e = e;

            startTimer.Interval = intervalMinutes * 60000;
            startTimer.Elapsed += delegate
            {
                //disable server state saving during events and 2 minutes after them
                DGPlugin.delaySaving(durationSeconds+(2*60));
                Timer saveTimer = new Timer(2.5*60*1000);
                saveTimer.AutoReset = false;
                saveTimer.Elapsed += delegate
                {
                    DGPlugin.clearSaveDelay();
                    saveTimer.Close();
                };

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
            endTimer.Close();
        }

    	public void beginRecurrence()
        {
            startTimer.Start();
        }

        public void stopRecurrence()
        {
            startTimer.Stop();
        }

    	public new string ToString()
        {
            return "["+e+"] every "+(startTimer.Interval/60000)+"m"+
                (!endTimer.AutoReset ? " for "+(endTimer.Interval/1000)+"s" : "");
        }
    }
}