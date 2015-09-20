using System;
using System.Linq;
using System.Threading;
using Rocket.Core.Logging;
using Timer = System.Timers.Timer;

namespace DingusGaming.Events
{
    public class ScheduledEvent
    {
        private readonly Timer startTimer = new Timer(), endTimer = new Timer();
        private readonly uint[] countDownTimes, waitTimes;
        private Timer countdownTimer;
        private readonly Event e;
        private readonly ushort minimumPlayers;
        private readonly uint intervalMinutes, durationSeconds = 0;

    	public ScheduledEvent(Event e, uint intervalMinutes, ushort minimumPlayers, uint[] countDownTimes, uint durationSeconds) : this(e, intervalMinutes, minimumPlayers, countDownTimes)
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

        public ScheduledEvent(Event e, uint intervalMinutes, ushort minimumPlayers, uint[] countDownTimes)
        {
            this.e = e;
            this.intervalMinutes = intervalMinutes;
            this.minimumPlayers = minimumPlayers;

            startTimer.Interval = intervalMinutes*60000;
            startTimer.Elapsed += delegate
            {
                fireEvent();
            };

            if (countDownTimes != null)
            {
                //start countdown timer
                //sort the countdown times
                Array.Sort(countDownTimes, (i1, i2) => i2.CompareTo(i1));

                //take out times greater than interval
                if (countDownTimes[0] > intervalMinutes*60)
                    for (int i = 0; i < countDownTimes.Length; ++i)
                        if (countDownTimes[i] < intervalMinutes*60)
                        {
                            countDownTimes = countDownTimes.Skip(i).ToArray();
                            break;
                        }

                this.countDownTimes = countDownTimes;
                waitTimes = new uint[countDownTimes.Length];
                countDownTimes.CopyTo(waitTimes, 0);

                //convert list to wait times
                for (int i = waitTimes.Length - 1; i >= 0; --i)
                {
                    if (i == 0)
                        waitTimes[i] = intervalMinutes*60 - waitTimes[i];
                    else
                        waitTimes[i] = waitTimes[i - 1] - waitTimes[i];
                }
            }
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

            if (countDownTimes != null && waitTimes != null)
            {
                //create the countdown timer
                int index = 0;
                countdownTimer?.Close();
                countdownTimer = new Timer(waitTimes[0] * 1000);
                countdownTimer.Elapsed += delegate
                {
                    DGPlugin.broadcastMessage(e.countDown(countDownTimes[index]));
                    countdownTimer.Interval = waitTimes[++index % countDownTimes.Length] * 1000;

                    if (index == 0)
                        countdownTimer.Stop();
                };

                countdownTimer.Start();
            }
        }

        ~ScheduledEvent()
        {
            startTimer.Close();
            endTimer.Close();
            countdownTimer?.Close();
        }

    	public void beginRecurrence()
        {
            fireEvent();
            startTimer.Start();
        }

        public void stopRecurrence()
        {
            startTimer.Stop();
            countdownTimer?.Stop();
        }

    	public new string ToString()
        {
            return "["+e.ToString()+"] every "+intervalMinutes+"m"+
                (durationSeconds!=0 ? " for "+durationSeconds+"s" : "")+
                (minimumPlayers!=0 ? " if "+minimumPlayers+"+ players on" : "");
        }
    }
}