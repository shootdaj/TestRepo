using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using ZoneLighting;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests.Programs
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
			//AddInput("DotColor", typeof(Color?), dotColor => DotColor = (Color?)dotColor);
			//AddInput("DelayTime", typeof(int), delayTime => DelayTime = (int)delayTime);
			AddMappedInput(this, "DotColor");
			AddMappedInput(this, "DelayTime");
		}

		public override void Loop()
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
				Lights.SetColor(Color.FromArgb(0, 0, 0));								//set all lights to black
				Lights[i].SetColor(DotColor != null
					? (Color)DotColor
					: colors[new Random().Next(0, colors.Count - 1)]);					//set one to white
				LightingController.SendLEDs(Lights.Cast<LED>().ToList());				//send frame
				ProgramCommon.Delay(DelayTime);											//pause before next iteration
			}
		}
	}
}