using System;
using System.Collections.Generic;
using System.Drawing;

namespace ZoneLighting.ZoneProgram.Programs
{
	/// <summary>
	/// Outputs a looping rainbow to the zone using the LightingController's built-in interpolation (currently only works with FadeCandy).
	/// </summary>
	public class RainbowInterpolating : LoopingZoneProgram
	{
		public override void Loop(IZoneProgramParameter parameter)
		{
			var colors = new List<Color>();
			colors.Add(Color.Violet);
			colors.Add(Color.Indigo);
			colors.Add(Color.Blue);
			colors.Add(Color.Green);
			colors.Add(Color.Yellow);
			colors.Add(Color.Orange);
			colors.Add(Color.Red);

			for (int i = 0; i < colors.Count; i++)
			{
				Lights.SetColor(colors[i]);
				Lights.Send(LightingController);
				ProgramCommon.Delay(((RainbowInterpolatingParameter)parameter).DelayTime);
			}
		}

		public override IEnumerable<Type> AllowedParameterTypes
		{
			get
			{
				return new List<Type>()
				{
					typeof(RainbowInterpolatingParameter)
				};
			}
		}
	}

	public class RainbowInterpolatingParameter : IZoneProgramParameter
	{
		public RainbowInterpolatingParameter(int delayTime)
		{
			DelayTime = delayTime;
		}
		public int DelayTime { get; set; }
	}
}