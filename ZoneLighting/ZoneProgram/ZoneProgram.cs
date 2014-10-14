using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Communication;

namespace ZoneLighting.ZoneProgram
{
	public abstract class ZoneProgram : IZoneProgram
	{
		public abstract void Start();
		public abstract void Stop();
		public Zone Zone { get; set; }

		public ILightingController LightingController
		{
			get { return Zone.LightingController; }
		}

		public SortedList<int, ILight> Lights
		{
			get { return Zone.Lights;  }
		}
	}
}
