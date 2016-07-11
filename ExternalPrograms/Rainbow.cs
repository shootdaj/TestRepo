using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.Graphics;
using ZoneLighting.ZoneNS;
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
		int DelayTime { get; set; } = 1;
		int Speed { get; set; } = 100;
		public override SyncLevel SyncLevel { get; set; } = RainbowSyncLevel.Fade;

		public override void Setup()
		{
			AddMappedInput<int>(this, "Speed", i => i.IsInRange(1, 100));
			AddMappedInput<int>(this, "DelayTime", i => i.IsInRange(1, 100));
			AddMappedInput<SyncLevel>(this, "SyncLevel");
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

				Animation.Fader.FadeStatic(GetColor(0), colors[i], Speed, DelayTime, false, (color) =>
				{
					SendColor(color);
				}, out endingColor, SyncLevel == RainbowSyncLevel.Fade ? SyncContext : null);

				if (SyncLevel == RainbowSyncLevel.Color)
					SyncContext?.SignalAndWait();	//synchronize at the color level
			}
		}

		/// <summary>
		/// This is like a fake enum.
		/// </summary>
		public static class RainbowSyncLevel
		{
			public static SyncLevel Fade = new SyncLevel("Fade");
			public static SyncLevel Color = new SyncLevel("Color");
		}
	}
}