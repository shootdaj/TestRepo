//using System.Collections.Generic;
//using System.ComponentModel.Composition;
//using System.Drawing;
//using ZoneLighting.ZoneNS;
//using ZoneLighting.ZoneProgramNS;

//namespace ExternalPrograms
//{
//	/// <summary>
//	/// Outputs a looping rainbow to the zone (currently only works with FadeCandy).
//	/// </summary>
//	[Export(typeof(ZoneProgram))]
//	[ExportMetadata("Name", "Shimmer")]
//	public class Shimmer : LoopingZoneProgram
//	{
//		public int Speed { get; set; } = 100;
//		public int Intensity { get; set; } = 100;
//		public override SyncLevel SyncLevel { get; set; } = ShimmerSyncLevel.None;

//		public override void Setup()
//		{
//			AddMappedInput<int>(this, "SyncLevel");

//		}

//		public override void Loop()
//		{
//			var colors = new List<Color>();
//			colors.Add(Color.Violet);
//			colors.Add(Color.Indigo);
//			colors.Add(Color.Blue);
//			colors.Add(Color.Green);
//			colors.Add(Color.Yellow);
//			colors.Add(Color.Orange);
//			colors.Add(Color.Red);

//			for (int i = 0; i < colors.Count; i++)
//			{
//				Color? endingColor;

//				ProgramCommon.Fade(GetColor(0), colors[i], Speed, DelayTime, false, (color) =>
//				{
//					SendColor(color);
//				}, out endingColor, SyncLevel == RainbowSyncLevel.Fade ? SyncContext : null);

//				if (SyncLevel == RainbowSyncLevel.Color)
//					SyncContext?.SignalAndWait();	//synchronize at the color level
//			}
//		}

//		/// <summary>
//		/// This is like a fake enum.
//		/// </summary>
//		public static class ShimmerSyncLevel
//		{
//			public static SyncLevel None = new SyncLevel("None");
//		}
//	}
//}
