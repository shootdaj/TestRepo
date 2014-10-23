using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ZoneLighting.ZoneProgram.Programs
{
	/// <summary>
	/// Outputs a looping rainbow to the zone.
	/// </summary>
	public class Rainbow : LoopingZoneProgram
	{
		public override void Loop(IZoneProgramParameter parameter)
		{
			var colors = new List<Color>();
			//colors.Add(Color.Violet);
			//colors.Add(Color.Indigo);
			colors.Add(Color.Blue);
			//colors.Add(Color.Green);
			//colors.Add(Color.Yellow);
			//colors.Add(Color.Orange);
			colors.Add(Color.Red);

			for (int i = 0; i < colors.Count; i++)
			{
				Lights.SetColor(colors[i]);
				Lights.Send(LightingController);
				ProgramCommon.Delay(((RainbowParameter)parameter).DelayTime);
			}
		}
	}

	public class RainbowParameter : IZoneProgramParameter
	{
		public RainbowParameter(int delayTime)
		{
			DelayTime = delayTime;
		}
		public int DelayTime { get; set; }
	}
}