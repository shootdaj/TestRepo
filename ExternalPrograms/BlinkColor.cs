using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Threading;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	/// <summary>
	/// Outputs a static color to the zone.
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "BlinkColor")]
	public class BlinkColor : ReactiveZoneProgram
	{
		protected override void StartCore(Barrier barrier)
		{
			AddInterruptingInput<Color>("Blink", colorTimeTuple =>
			{
				var color = ((Tuple<Color, int>)colorTimeTuple).Item1;
				var time = ((Tuple<Color, int>)colorTimeTuple).Item2;

				ProgramCommon.Blink(new List<Tuple<Color, int>>
				{
					{ color, time},
					{ Color.Empty, time}
				}, (colorToSet) =>
				{
					Lights.SetColor(colorToSet);
					Lights.Send(LightingController);
				});
			});
		}

		protected override void StopCore(bool force)
		{
			RemoveInput("Color");
		}
	}
}