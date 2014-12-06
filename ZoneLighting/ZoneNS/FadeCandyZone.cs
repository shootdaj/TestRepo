using System.Collections.Generic;
using ZoneLighting.Communication;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.ZoneNS
{
	public class FadeCandyZone : Zone
	{
		public FadeCandyZone(string name = "", ZoneProgram program = null, Dictionary<string, object> inputStartingValues = null) : base(FadeCandyController.Instance, name, program, inputStartingValues)
		{
		}
	}
}
