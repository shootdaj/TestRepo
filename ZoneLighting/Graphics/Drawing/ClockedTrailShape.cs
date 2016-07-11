using ZoneLighting.ZoneProgramNS.Clock;

namespace ZoneLighting.Graphics.Drawing
{
	public class ClockedTrailShape
	{


		public MicroClock Clock { get; set; }

		public TrailShape TrailShape { get; set; }
		
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
