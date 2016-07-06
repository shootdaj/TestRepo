using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.Drawing
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
