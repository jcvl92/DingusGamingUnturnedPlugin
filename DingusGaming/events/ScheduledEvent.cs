using System.Timers;

namespace DingusGaming.Events
{
    public class ScheduledEvent
    {
        private readonly Timer startTimer = new Timer(), endTimer = new Timer();
        private readonly Event e;
        private readonly ushort minimumPlayers;
        private readonly uint intervalMinutes, durationSeconds = 0;

    	public ScheduledEvent(Event e, uint intervalMinutes, ushort minimumPlayers, uint durationSeconds)
        {
            this.e = e;
    	    this.intervalMinutes = intervalMinutes;
            this.minimumPlayers = minimumPlayers;
    	    this.durationSeconds = durationSeconds;

            startTimer.Interval = 1;
            startTimer.Elapsed += delegate
            {
                if(startTimer.Interval == 1)
                    startTimer.Interval = intervalMinutes * 60000;
                
                if (DGPlugin.getPlayersCount() >= minimumPlayers)
                {
                    //disable server state saving during events and 1 minute after them
                    DGPlugin.delaySaving(durationSeconds + 60);
                    
                    e.startEvent();
                    endTimer.Start();
                }
            };

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

            startTimer.Interval = 1;
            startTimer.Elapsed += delegate
            {
                if (startTimer.Interval == 1)
                    startTimer.Interval = intervalMinutes * 60000;

                if (DGPlugin.getPlayersCount() >= minimumPlayers)
                {
                    e.startEvent();
                }
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
            return "["+e.ToString()+"] every "+intervalMinutes+"m"+
                (durationSeconds!=0 ? " for "+durationSeconds+"s" : "")+
                (minimumPlayers!=0 ? " if "+minimumPlayers+"+ players on" : "");
        }
    }
}