using System;
using System.Timers;

namespace ZoneLighting.ZoneProgramNS.Clock
{
    public class TimerClock
    {
	    public enum ClockState
	    {
		    Started,
			Stopped
	    }

	    public ClockState State { get; set; } = ClockState.Stopped;

		private Timer Timer { get; } = new Timer();

        public double Interval
        {
            get { return Timer.Interval; }
            set { Timer.Interval = value; }
        }

		public Action<ElapsedEventArgs> Action { get; set; }

        /// <summary>
        /// Creates a new instance of the MicroTimerClock
        /// </summary>
        /// <param name="interval">Tick time defines how often OnTick() will be called</param>
        /// <param name="action">Action to perform for every tick of the clock</param>
        public TimerClock(double interval, Action<ElapsedEventArgs> action)
        {
            Interval = interval;
			Timer.Elapsed += Timer_Elapsed;
	        Action = action;
	    }

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (Action == null)
				throw new Exception("Action cannot be null.");
			Action(e);
		}
		
		public void Start()
	    {
		    Timer.Start();
            State = ClockState.Started; 
	    }

        public void Stop(int timeout=1000)
        {
            Timer.Stop();
            State = ClockState.Stopped;
        }
	}
}