using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests.Programs
{
	/// <summary>
	/// Outputs a looping rainbow to the zone using the LightingController's built-in interpolation (currently only works with FadeCandy).
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "Rainbow")]
	[ExportMetadata("ParameterName", "RainbowParameter")]
	public class Rainbow : LoopingZoneProgram
	{
		public override void Setup(ZoneProgramParameter parameter)
		{
			
		}

		public override void Loop(ZoneProgramParameter parameter)
		{
			var colors = new List<Color>();
			colors.Add(Color.Violet);
			colors.Add(Color.Indigo);
			colors.Add(Color.Blue);
			colors.Add(Color.Green);
			colors.Add(Color.Yellow);
			colors.Add(Color.Orange);
			colors.Add(Color.Red);

			RainbowParameter rainbowParameter = (RainbowParameter)parameter;

			for (int i = 0; i < colors.Count; i++)
			{
				Color? endingColor;

				ProgramCommon.Fade(Lights[0].GetColor(), colors[i], rainbowParameter.Speed, rainbowParameter.DelayTime, false, (color) =>
				{
					Lights.SetColor(color);
					Lights.Send(LightingController);
				}, out endingColor);
			}
		}

		public override IEnumerable<Type> AllowedParameterTypes
		{
			get
			{
				return new List<Type>()
			{
				typeof (RainbowParameter),
			};
			}
		}
	}

	public class RainbowParameter : ZoneProgramParameter
	{
		public RainbowParameter(int speed, int delayTime)
		{
			Speed = speed;
			DelayTime = delayTime;
		}

		public RainbowParameter()
		{

		}

		public int Speed { get; set; }
		public int DelayTime { get; set; }
	}
}
