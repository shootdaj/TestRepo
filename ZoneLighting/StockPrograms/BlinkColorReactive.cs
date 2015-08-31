using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.StockPrograms
{
	/// <summary>
	/// Outputs a static color to the zone.
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "BlinkColorReactive")]
	public class BlinkColorReactive : ReactiveZoneProgram
	{
		protected override void SetupInterruptingInputs()
		{
			AddInterruptingInput<Color>("Blink", parametersObject =>
			{
				dynamic parameters = parametersObject;
				Color color = parameters.Color;
				int time = parameters.Time;
				bool soft = parameters.Soft;

				if (soft)
				{
					ProgramCommon.SoftBlink(new List<Tuple<Color, int>>
					{
						{color, time},
					}, OutputColor, SyncContext);
				}
				else
				{
					ProgramCommon.Blink(new List<Tuple<Color, int>>
					{
						{color, time},
						{Color.Empty, time}
					}, OutputColor, SyncContext);
				}
			});
		}

		private void OutputColor(Color colorToSet)
		{
			SendColor(colorToSet);
		}


		protected override void StopCore(bool force)
		{
			RemoveInput("Color");
		}
	}
}