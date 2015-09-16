using System.Timers;

namespace DingusGaming.Events
{
    public class ScheduledEvent
    {
        private readonly Timer startTimer = new Timer(), endTimer = new Timer();
        private readonly Event e;
        private readonly ushort minimumPlayers;

    	public ScheduledEvent(Event e, uint intervalMinutes, ushort minimumPlayers, uint durationSeconds)
        {
            this.e = e;
            this.minimumPlayers = minimumPlayers;

            startTimer.Interval = intervalMinutes * 60000;
            startTimer.Elapsed += delegate
            {
                if(DGPlugin.getPlayersCount() >= minimumPlayers)
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
                }
            };

            endTimer.Interval = durationSeconds * 1000;
            endTimer.AutoReset = false;
            endTimer.Elapsed += delegate
            {
                e.stopEvent();
            };
        }

        public ScheduledEvent(Event e, uint intervalMinutes, ushort minimumPlayers)
        {
            this.e = e;

            startTimer.Interval = intervalMinutes * 60000;
            startTimer.Elapsed += delegate
            {
                if(DGPlugin.getPlayersCount() >= minimumPlayers)
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
            return "["+e+"] every "+(startTimer.Interval/60000)+"m"+
                (!endTimer.AutoReset ? " for "+(endTimer.Interval/1000)+"s" : "")+
                (minimumPlayers!=0 ? " if "+minimumPlayers" players" : "");
        }
    }
}