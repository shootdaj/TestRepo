using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZoneLighting.ZoneProgramNS
{
	public abstract class ReactiveZoneProgram : ZoneProgram
	{
		protected abstract override void StartCore();

		public override void Stop(bool force)
		{
			
		}

		//protected Task RunProgram { get; set; }
		//protected Thread RunProgramThread { get; set; }
	}
}
