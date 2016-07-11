using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.Graphics;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	/// <summary>
	/// Outputs a static color to the zone.
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "BlinkColorLoop")]
	public class BlinkColorLoop : LoopingZoneProgram
	{
		int HoldTime { get; set; } = 500;

		public BlinkColorLoop(SyncContext syncContext = null) : base(syncContext: syncContext)
		{
		}

		public override SyncLevel SyncLevel
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public override void Setup()
		{
			AddMappedInput<int>(this, "HoldTime", i => i.IsInRange(1, 100));
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
				Animation.Blink(new List<Tuple<Color, int>>() { new Tuple<Color, int>(colors[i], HoldTime) }, SendColor, SyncContext);
			}
		}
	}
}