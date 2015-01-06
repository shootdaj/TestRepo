using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Threading;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	/// <summary>
	/// Outputs a looping rainbow to the zone (currently only works with FadeCandy).
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "Rainbow")]
	public class Rainbow : LoopingZoneProgram
	{
		public int DelayTime { get; set; } = 1;
		public int Speed { get; set; } = 1;

		public override void Setup()
		{
			AddInput<int>("Speed", speed => Speed = (int)speed);
			AddInput<int>("DelayTime", delayTime => DelayTime = (int)delayTime);
		}

		public override void Loop(Barrier barrier)
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
				Color? endingColor;

				ProgramCommon.Fade(Lights[0].GetColor(), colors[i], Speed, DelayTime, false, (color) =>
				{
					Lights.SetColor(color);
					Lights.Send(LightingController);
				}, out endingColor, barrier);

				//barrier?.SignalAndWait();  //synchronize at the fade single color level
			}
		}
	}
}
