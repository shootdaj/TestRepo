using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	/// <summary>
	/// Scrolls a dot across the entire length of Lights
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "Scroll2Dots")]
	public class Scroll2Dots : LoopingZoneProgram
	{
		int DelayTime { get; set; } = 50;
		Color? DotColor { get; set; } = Color.Blue;
		public override SyncLevel SyncLevel { get; set; } = ScrollDotSyncLevel.Dot;

		public override void Setup()
		{
			AddMappedInput<int>(this, "DelayTime");
			AddMappedInput<Color?>(this, "DotColor");
		}

		public override void Loop()
		{
			//DebugTools.AddEvent("ScrollDot.Loop", "START Looping ScrollDot");

			var colors = new List<Color>();
			colors.Add(Color.Red);
			colors.Add(Color.Blue);
			colors.Add(Color.Yellow);
			colors.Add(Color.Green);
			colors.Add(Color.Purple);
			colors.Add(Color.RoyalBlue);
			colors.Add(Color.MediumSeaGreen);

			for (int i = 0; i < LightCount; i++)
			{
				//prepare frame
				var sendColors = new Dictionary<int, Color>();
				Zone.SortedLights.Keys.ToList().ForEach(lightIndex => sendColors.Add(lightIndex, Color.Black));
				sendColors[i] = DotColor != null ? (Color) DotColor : colors[new Random().Next(0, colors.Count - 1)];
				if (i+1 < LightCount) sendColors[i+1] = DotColor != null ? (Color)DotColor : colors[new Random().Next(0, colors.Count - 1)];

				SendColors(sendColors);		//send frame
				ProgramCommon.Delay(DelayTime);											//pause before next iteration

				SyncContext?.SignalAndWait(100);
			}

			//DebugTools.AddEvent("ScrollDot.Loop", "START Looping ScrollDot");
		}

		public static class ScrollDotSyncLevel
		{
			public static SyncLevel Dot => new SyncLevel("Dot");
		}
	}
}