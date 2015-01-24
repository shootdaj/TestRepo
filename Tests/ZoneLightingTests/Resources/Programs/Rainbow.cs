using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests.Resources.Programs
{
	/// <summary>
	/// Outputs a looping rainbow to the zone (currently only works with FadeCandy).
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "Rainbow")]
	public class Rainbow : LoopingZoneProgram
	{
		public int DelayTime { get; set; } = 1;
		public int Speed { get; set; } = 100;
		public override SyncLevel SyncLevel { get; set; } = RainbowSyncLevel.Fade;

		public override void Setup()
		{
			AddInput<int>("Speed", speed => Speed = (int)speed);
			AddInput<int>("DelayTime", delayTime => DelayTime = (int)delayTime);
			AddMappedInput<int>(this, "SyncLevel");
		}

		public override void Loop()
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

				ProgramCommon.Fade(GetColor(0), colors[i], Speed, DelayTime, false, (color) =>
				{
					SetColor(color);
					SendLights();
				}, out endingColor, SyncLevel == RainbowSyncLevel.Fade ? SyncContext : null);

				if (SyncLevel == RainbowSyncLevel.Color)
					SyncContext?.SignalAndWait();   //synchronize at the color level
			}
		}

		public static class RainbowSyncLevel
		{
			public static SyncLevel Fade = new SyncLevel("Fade");
			public static SyncLevel Color = new SyncLevel("Color");
		}
	}
}
