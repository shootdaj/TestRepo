using System.Collections.Generic;
using ZoneLighting.Communication;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.ZoneNS
{
	public class FadeCandyZone : Zone
	{
		public FadeCandyZone(string name = "", ZoneProgram program = null, InputStartingValues inputStartingValues = null) : base(FadeCandyController.Instance, name, program, inputStartingValues)
		{
		}
	}
}
