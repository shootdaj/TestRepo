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
	[ExportMetadata("Name", "AndroidTrail")]
	public class AndroidTrail : LoopingZoneProgram
	{
		private readonly List<Color> _colors;

		public AndroidTrail()
		{
			_colors = new List<Color>()
			{
				Color.Red, Color.Blue, Color.Yellow, Color.Green, Color.Purple, Color.RoyalBlue, Color.MediumSeaGreen
			};
		}

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
			for (int i = 0; i < LightCount; i++)
			{
				//prepare frame
				var sendColors = new Dictionary<int, Color>();
				Blackout(sendColors);


				SetLeadingColor(sendColors, i, _colors);
				//SetTrailLeadingColor(sendColors, i, 3, true);
				if (i + 1 < LightCount) sendColors[i + 1] = DotColor != null ? (Color)DotColor : _colors[new Random().Next(0, _colors.Count - 1)];

				SendColors(sendColors);     //send frame
				ProgramCommon.Delay(DelayTime);                                         //pause before next iteration

				SyncContext?.SignalAndWait(1000);
			}
		}

		private Color SetColor(decimal brightness = (decimal)1.0)
		{
			return (Color)DotColor;
		}

		private void TrailLeadingColor(Dictionary<int, Color> sendColors, int leadIndex, int length, bool forward)
		{
			for (int i = 0; i < length; i++)
			{
				if (forward) sendColors[leadIndex + 1] = SetColor((decimal)length / (i + 1));
			}
		}

		private void SetLeadingColor(Dictionary<int, Color> sendColors, int i, List<Color> colors)
		{
			sendColors[i] = DotColor != null ? (Color)DotColor : colors[new Random().Next(0, colors.Count - 1)];
		}

		private void Blackout(Dictionary<int, Color> sendColors)
		{
			Zone.SortedLights.Keys.ToList().ForEach(lightIndex => sendColors.Add(lightIndex, Color.Black));
		}

		public static class ScrollDotSyncLevel
		{
			public static SyncLevel Dot => new SyncLevel("Dot");
		}
	}
}