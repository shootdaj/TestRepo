using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.ZoneProgram;

namespace ExternalPrograms
{
	/// <summary>
	/// Outputs a looping rainbow to the zone using the LightingController's built-in interpolation (currently only works with FadeCandy).
	/// </summary>
	[Export(typeof(IZoneProgram))]
	[ExportMetadata("Name", "Rainbow")]
	[ExportMetadata("ParameterName", "RainbowParameter")]
	public class Rainbow : LoopingZoneProgram
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

	[Export(typeof(IZoneProgramParameter))]
	[ExportMetadata("Name", "RainbowParameter")]
	public class RainbowParameter : IZoneProgramParameter
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
