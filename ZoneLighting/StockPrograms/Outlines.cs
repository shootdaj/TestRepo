using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.StockPrograms
{
	/// <summary>
	/// Outlines
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "Outlines")]
	public class Outlines : LoopingZoneProgram
	{
		public List<ScrollTrail> ScrollTrails { get; set; }

		int DelayTime { get; set; } = 65;
		//public Color? DotColor { get; set; } = Color.Blue;
		
		public override void Setup()
		{
			AddMappedInput<int>(this, "DelayTime");
		}

		public override void Loop()
		{
			
		}

		public static class ScrollDotSyncLevel
		{
			public static SyncLevel Dot => new SyncLevel("Dot");
		}
	}
}