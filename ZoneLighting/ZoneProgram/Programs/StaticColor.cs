using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ZoneLighting.ZoneProgram.Programs
{
	/// <summary>
	/// Outputs a static color to the zone.
	/// </summary>
	public class StaticColor : LoopingZoneProgram
	{
		public override void Loop(IZoneProgramParameter parameter)
		{
			Lights.SetColor(((StaticColorParameter)parameter).Color);
			LightingController.SendLights(Lights);				//send frame
			ProgramCommon.Delay(1000);
		}

		public override IEnumerable<Type> AllowedParameterTypes
		{
			get
			{
				return new List<Type>()
				{
					typeof (StaticColorParameter)
				};
			}
		}
	}

	public class StaticColorParameter : IZoneProgramParameter
	{
		public StaticColorParameter(Color color)
		{
			Color = color;
		}
		public Color Color { get; set; }
	}
}