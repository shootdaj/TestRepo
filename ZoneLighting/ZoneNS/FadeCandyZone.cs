using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Communication;
using ZoneLighting.ZoneProgram;

namespace ZoneLighting.ZoneNS
{
	public class FadeCandyZone : Zone
	{
		public FadeCandyZone(string name = "", IZoneProgram program = null, IZoneProgramParameter programParameter = null) : base(FadeCandyController.Instance, name, program, programParameter)
		{
		}
	}
}
