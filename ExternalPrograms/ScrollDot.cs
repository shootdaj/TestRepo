using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	/// <summary>
	/// Scrolls a dot across the entire length of Lights
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "ScrollDot")]
	public class ScrollDot : LoopingZoneProgram
	{
		public int DelayTime { get; set; }
		public Color? DotColor { get; set; }

		public override void Setup()
		{
			AddMappedInput<int>(this, "DelayTime");
			AddMappedInput<Color?>(this, "DotColor");
		}

		public override void Loop(Barrier barrier)
		{
			var colors = new List<Color>();
			colors.Add(Color.Red);
			colors.Add(Color.Blue);
			colors.Add(Color.Yellow);
			colors.Add(Color.Green);
			colors.Add(Color.Purple);
			colors.Add(Color.RoyalBlue);
			colors.Add(Color.MediumSeaGreen);

			for (int i = 0; i < Zone.Lights.Count; i++)
			{
				Lights.SetColor(Color.FromArgb(0, 0, 0));											//set all lights to black
				Lights[i].SetColor(DotColor != null
					? (Color)DotColor
					: colors[new Random().Next(0, colors.Count - 1)]);								//set one to white
				LightingController.SendLEDs(Lights.Cast<ILightingControllerPixel>().ToList());		//send frame
				ProgramCommon.Delay(DelayTime);											//pause before next iteration

				barrier?.SignalAndWait();
			}
		}
	}
}