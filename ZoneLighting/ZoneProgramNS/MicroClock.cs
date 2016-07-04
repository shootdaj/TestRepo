using System;
using ZoneLighting.ZoneProgramNS.MicroLibrary;

namespace ZoneLighting.ZoneProgramNS
{
    public class MicroClock
    {
	    public enum ClockState
	    {
		    Started,
			Stopped
	    }

	    public ClockState State { get; set; } = ClockState.Stopped;

		private MicroTimer Timer { get; set; } = new MicroTimer();

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
        /// Creates a new instance of the MicroTimerClock
        /// </summary>
        /// <param name="interval">Tick time defines how often OnTick() will be called</param>
        /// <param name="action">Action to perform for every tick of the clock</param>
        /// <param name="ignoreEventIfLateBy">.NET is inherently non-realtime, so there is a certain amount of
        /// drift that may occur during heavy processing. This drift can be ignored if it passed a certain threhold that is 
        /// provided by this parameter. More details are at http://www.codeproject.com/Articles/98346/Microsecond-and-Millisecond-NET-Timer</param>
        public MicroClock(long interval, Action<MicroTimerEventArgs> action, long ignoreEventIfLateBy)
        {
            Interval = interval;
            Timer.MicroTimerElapsed += Timer_MicroTimerElapsed;
	        Action = action;
	        IgnoreEventIfLateBy = ignoreEventIfLateBy;
	    }
        
        public void Timer_MicroTimerElapsed(object sender, MicroTimerEventArgs timerEventArgs)
        {
            if (Action == null)
                throw new Exception("Action cannot be null.");
            Action(timerEventArgs);
        }

		public void Start()
	    {
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