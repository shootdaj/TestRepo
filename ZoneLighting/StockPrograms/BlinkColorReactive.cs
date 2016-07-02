using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.StockPrograms
{
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "BlinkColorReactive")]
	public class BlinkColorReactive : ReactiveZoneProgram
	{
		private double Brightness { get; set; } = 1;

		protected override void Setup()
		{
			AddInterruptingInput<Color>("Blink", parametersObject =>
			{
				dynamic parameters = parametersObject;
				Color color = parameters.Color;
				int time = parameters.Time;
				bool soft = parameters.Soft;
				double brightness = parameters.Brightness ?? Brightness;

				if (soft)
				{
					ProgramCommon.SoftBlink(new List<Tuple<Color, int>>
					{
						{color, time},
					}, OutputColor, SyncContext, true, null, ForceStoppable, brightness);
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
			RemoveInput("Blink");
		}
	}
}