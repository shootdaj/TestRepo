using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.ZoneProgram;

namespace ExternalPrograms
{
	/// <summary>
	/// Outputs a static color to the zone.
	/// </summary>
	[Export(typeof(IZoneProgram))]
	[ExportMetadata("Name", "StaticColor")]
	[ExportMetadata("ParameterName", "StaticColorParameter")]
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

	[Export(typeof(IZoneProgramParameter))]
	[ExportMetadata("Name", "StaticColorParameter")]
	public class StaticColorParameter : IZoneProgramParameter
	{
		public StaticColorParameter(Color color)
		{
			Color = color;
		}
		public Color Color { get; set; }
	}
}