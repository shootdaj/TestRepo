using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Communication;

namespace ZoneLighting.ZoneProgram
{
	/// <summary>
	/// Represents a "program" that can be played on a zone. Something like a loop
	/// or a periodic notification, or anything else that can be represented by lighting 
	/// the zones in a certain way.
	/// </summary>
	public abstract class ZoneProgram : IZoneProgram
	{
		public abstract void Start(IZoneProgramParameter parameter);
		public abstract void Stop();

		public Zone Zone { get; set; }

		public LightingController LightingController
		{
			get { return Zone.LightingController; }
		}

		public IList<ILogicalRGBLight> Lights
		{
			get { return Zone.Lights;  }
		}
	}
}
