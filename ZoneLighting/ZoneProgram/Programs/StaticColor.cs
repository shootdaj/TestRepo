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