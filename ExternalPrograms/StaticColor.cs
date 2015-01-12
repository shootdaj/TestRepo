using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading;
using ZoneLighting.Communication;
using ZoneLighting.TriggerDependencyNS;
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
		public Trigger ChangeLightColorTrigger { get; } = new Trigger("ChangeLightColorTrigger");

		protected override void StartCore(Barrier barrier)
		{
			AddInterruptingInput<Color>("Color", color =>
			{
				SetColor((Color)color);
				SendLights();	//send frame
				Thread.Sleep(Timeout.Infinite);
			});
		}

		protected override void StopCore(bool force)
		{
			RemoveInput("Color");
		}
	}
}