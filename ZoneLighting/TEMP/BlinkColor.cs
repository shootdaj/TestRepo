using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.TEMP
{
	/// <summary>
	/// Outputs a static color to the zone.
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "BlinkColor")]
	public class BlinkColor : ReactiveZoneProgram
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
					ProgramCommon.SoftBlink(this, new List<Tuple<Color, int>>
					{
						{color, time},
						{Color.Black, time}
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
			SetColor(colorToSet);
			SendLights();
		}


		protected override void StopCore(bool force)
		{
			RemoveInput("Color");
		}
	}
}