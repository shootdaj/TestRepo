using System;

namespace ZoneLighting.ZoneProgramNS.Clock
{
    public class MilliClock
    {
	    public enum ClockState
	    {
		    Started,
			Stopped
	    }

	    public ClockState State { get; set; } = ClockState.Stopped;

		private MilliTimer Timer { get; set; }

        public long Interval
        {
            get { return Timer.Interval; }
            set { Timer.Interval = value; }
        }

        public long IgnoreEventIfLateBy
        {
            get { return Timer.IgnoreEventIfLateBy; }
            set { Timer.IgnoreEventIfLateBy = value; }
        }

        public Action<MicroTimerEventArgs> Action { get; set; }

        /// <summary>
        /// Creates a new instance of the MilliTimerClock
        /// </summary>
        /// <param name="interval">Tick time defines how often OnTick() will be called</param>
        /// <param name="action">Action to perform for every tick of the clock</param>
        /// <param name="ignoreEventIfLateBy">.NET is inherently non-realtime, so there is a certain amount of
        /// drift that may occur during heavy processing. This drift can be ignored if it passed a certain threhold that is 
        /// provided by this parameter. More details are at http://www.codeproject.com/Articles/98346/Microsecond-and-Millisecond-NET-Timer</param>
        public MilliClock(long interval, Action<MicroTimerEventArgs> action, long ignoreEventIfLateBy)
        {
			Timer = new MilliTimer(interval);
			Timer.MilliTimerElapsed += Timer_MilliTimerElapsed;
	        Action = action;
	        IgnoreEventIfLateBy = ignoreEventIfLateBy;
        }
        
        public void Timer_MilliTimerElapsed(object sender, MicroTimerEventArgs timerEventArgs)
        {
			Action(timerEventArgs);
        }

		public void Start()
	    {
			if (Action == null)
				throw new Exception("Action cannot be null.");
			Timer.Start();
            State = ClockState.Started; 
	    }

        public void Stop(int timeout=1000)
        {
            if (!Timer.StopAndWait(timeout))
            {
                Timer.Abort();
            }
            State = ClockState.Stopped;
        }
	}
}