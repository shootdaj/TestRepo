using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
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
		protected override void SetupInterruptingInputs()
		{
			AddInterruptingInput<Color>("Blink", colorTimeTuple =>
			{
				var color = ((Tuple<Color, int>)colorTimeTuple).Item1;
				var time = ((Tuple<Color, int>)colorTimeTuple).Item2;

				////if sync is requested, go into synchronizable state
				//if (IsSyncStateRequested)
				//{
				//	IsSynchronizable.Fire(this, null);
				//	WaitForSync.WaitForFire();
				//	IsSyncStateRequested = false;
				//}

				ProgramCommon.Blink(new List<Tuple<Color, int>>
				{
					{ color, time},
					{ Color.Empty, time}
				}, (colorToSet) =>
				{
					SetColor(colorToSet);
					SendLights();
				}, SyncContext, this);
			});
		}

		protected override void StopCore(bool force)
		{
			RemoveInput("Color");
		}
	}
}