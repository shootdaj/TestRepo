using System.ComponentModel.Composition;
using System.Drawing;
using System.Threading;
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

		protected override void Setup()
		{
			AddInterruptingInput<Color>("Color", color =>
			{
				SendColor((Color)color);	//send frame
				Thread.Sleep(Timeout.Infinite);
			});
		}

		protected override void StopCore(bool force)
		{
			RemoveInput("Color");
		}
	}
}