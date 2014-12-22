using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using ZoneLighting.Communication;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests.Resources.Programs
{
	/// <summary>
	/// Outputs a static color to the zone.
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "StaticColor")]
	public class StaticColor : ReactiveZoneProgram
	{
		protected override void StartCore()
		{
			AddInput<Color>("Color", color =>
			{
				Lights.SetColor((Color)color);
				LightingController.SendLights(Lights.Cast<ILightingControllerPixel>().ToList());	//send frame
			});
		}

		public override void Stop(bool force)
		{
			RemoveInput("Color");
		}
	}
}
