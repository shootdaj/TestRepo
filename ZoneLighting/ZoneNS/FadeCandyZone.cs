using ZoneLighting.Communication;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.ZoneNS
{
	public class FadeCandyZone : Zone
	{
		public FadeCandyZone(string name = "", ZoneProgram program = null, ZoneProgramParameter programParameter = null) : base(FadeCandyController.Instance, name, program, programParameter)
		{
		}
	}
}
