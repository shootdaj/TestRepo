using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading;
using ZoneLighting.Communication;
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
		//protected override void StartCore()
		//{
		//	//AddInput<Color>("Color", color =>
		//	//{
		//	//	Lights.SetColor((Color)color);
		//	//	LightingController.SendLights(Lights.Cast<ILightingControllerPixel>().ToList());	//send frame
		//	//});

		//	AddInterruptingInput<Color>("Color", color =>
		//	{
		//		Lights.SetColor((Color)color);
		//		LightingController.SendLights(Lights.Cast<ILightingControllerPixel>().ToList());	//send frame
		//	});


		//}

		protected override void StartCore()
		{
			AddInterruptingInput<Color>("Color", color =>
			{
				Lights.SetColor((Color)color);
				LightingController.SendLights(Lights.Cast<ILightingControllerPixel>().ToList());	//send frame
				Thread.Sleep(Timeout.Infinite);
			});
		}

		public override void Stop(bool force)
		{
			RemoveInput("Color");
		}
	}
}