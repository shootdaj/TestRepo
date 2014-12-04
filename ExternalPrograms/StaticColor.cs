using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	/// <summary>
	/// Outputs a static color to the zone.
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "StaticColor")]
	public class StaticColor : ReactiveZoneProgram
	{
		protected override void Start()
		{
			AddInput("LiveColor", color =>
			{
				Lights.SetColor((Color)color);
				LightingController.SendLights(Lights);	//send frame
			});
		}

		public override void Stop(bool force)
		{
			RemoveInput("LiveColor");
		}
	}
}