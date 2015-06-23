using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	/// <summary>
	/// Scrolls a trail across the entire length of Lights
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "ScrollTrail")]
	public class ScrollTrail : LoopingZoneProgram
	{
		public int DelayTime { get; set; } = 65;
		public Color? DotColor { get; set; } = Color.Blue;
		public override SyncLevel SyncLevel { get; set; } = ScrollDotSyncLevel.Dot;

		private int _trailLength = 3;
		private double _darkenFactor = 0.8;

		public override void Setup()
		{
			AddMappedInput<int>(this, "DelayTime");
			AddMappedInput<Color?>(this, "DotColor");
			DotColor = ProgramCommon.GetRandomColor();
		}

		public override void Loop()
		{
			for (int i = 0; i < LightCount; i++)
			{
				//prepare frame
				var sendColors = new Dictionary<int, Color>();//ProgramCommon.BlankColors(Zone);
				for (int j = 0; j < LightCount; j++)
				{
					sendColors[j] = GetColor(j).Darken(_darkenFactor);
				}
				sendColors[i] = DotColor ?? ProgramCommon.GetRandomColor();
				
				SendColors(sendColors);		//send frame
				ProgramCommon.Delay(DelayTime);											//pause before next iteration

				SyncContext?.SignalAndWait(100);
			}
		}

		public static class ScrollDotSyncLevel
		{
			public static SyncLevel Dot => new SyncLevel("Dot");
		}
	}
}