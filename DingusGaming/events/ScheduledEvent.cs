using System.Timers;

namespace DingusGaming.Events
{
    public class ScheduledEvent
    {
        private readonly Timer startTimer = new Timer(), endTimer = new Timer();
        private readonly Event e;
        private readonly ushort minimumPlayers;
        private readonly uint intervalMinutes, durationSeconds = 0;

    	public ScheduledEvent(Event e, uint intervalMinutes, ushort minimumPlayers, uint durationSeconds) : this(e, intervalMinutes, minimumPlayers)
    	{
    	    this.durationSeconds = durationSeconds;

            endTimer.Interval = durationSeconds * 1000;
            endTimer.AutoReset = false;
            endTimer.Elapsed += delegate
            {
                e.stopEvent();

                //clear delay 1 minute after event
                Timer saveTimer = new Timer(60000);
                saveTimer.AutoReset = false;
                saveTimer.Elapsed += delegate
                {
                    DGPlugin.clearSaveDelay();
                    saveTimer.Close();
                };
                saveTimer.Start();
            };
        }

        public ScheduledEvent(Event e, uint intervalMinutes, ushort minimumPlayers)
        {
            this.e = e;
            this.intervalMinutes = intervalMinutes;
            this.minimumPlayers = minimumPlayers;

            startTimer.Interval = intervalMinutes * 60000;
            startTimer.Elapsed += delegate
            {
                fireEvent();
            };
        }

        private void fireEvent()
        {
            if (DGPlugin.getPlayersCount() >= minimumPlayers)
            {
                if (durationSeconds != 0)
                {
                    //disable server state saving during events and 1 minute after them
                    DGPlugin.delaySaving(durationSeconds + 60);

                    endTimer.Start();
                }

                e.startEvent();
            }
        }

        ~ScheduledEvent()
        {
            startTimer.Close();
            endTimer.Close();
        }

    	public void beginRecurrence()
        {
            fireEvent();
            startTimer.Start();
        }

        public void stopRecurrence()
        {
            startTimer.Stop();
        }

    	public new string ToString()
        {
            return "["+e.ToString()+"] every "+intervalMinutes+"m"+
                (durationSeconds!=0 ? " for "+durationSeconds+"s" : "")+
                (minimumPlayers!=0 ? " if "+minimumPlayers+"+ players on" : "");
        }
    }
}