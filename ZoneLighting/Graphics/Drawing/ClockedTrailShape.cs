using System;
using ZoneLighting.ZoneProgramNS.Clock;

namespace ZoneLighting.Graphics.Drawing
{
	public class ClockedTrailShape
	{
		public double Interval
		{
			get { return Clock.Interval; }
			set { Clock.Interval = value; }
		}

		public TimerClock Clock { get; set; }

		public TrailShape TrailShape { get; set; }

		public Func<int> GetNewInterval { get; set; }
		
		public void Start()
		{
			Clock.Start();
		}

		public void Stop()
		{
			Clock.Stop();
		}
	}
}
