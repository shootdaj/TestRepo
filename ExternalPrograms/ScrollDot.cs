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
	[ExportMetadata("Name", "ScrollDot")]
	public class ScrollDot : LoopingZoneProgram
	{
		int DelayTime { get; set; } = 50;
		Color? DotColor { get; set; }
		public override SyncLevel SyncLevel { get; set; } = ScrollDotSyncLevel.Dot;



		public override void Setup()
		{
			AddMappedInput<int>(this, "DelayTime");
			AddMappedInput<Color?>(this, "DotColor");
		}

		public override void Loop()
		{
			//DebugTools.AddEvent("ScrollDot.Loop", "START Looping ScrollDot");
			for (int i = 0; i < LightCount; i++)
			{
				//prepare frame
				var sendColors = new Dictionary<int, Color>();
				Zone.SortedLights.Keys.ToList().ForEach(lightIndex => sendColors.Add(lightIndex, Color.Black));
				sendColors[i] = DotColor ?? ProgramCommon.GetRandomColor();

				SendColors(sendColors);		//send frame
				ProgramCommon.Delay(DelayTime);											//pause before next iteration

				SyncContext?.SignalAndWait(100);
			}

			///DebugTools.AddEvent("ScrollDot.Loop", "START Looping ScrollDot");
		}

		public static class ScrollDotSyncLevel
		{
			public static SyncLevel Dot => new SyncLevel("Dot");
		}
	}
}