using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.TriggerDependencyNS;
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
		public Trigger ChangeLightColorTrigger { get; } = new Trigger("ChangeLightColorTrigger");

		protected override void Setup()
		{
			AddInterruptingInput<Color>("Color", color =>
			{
				ChangeLightColorTrigger.Fire(this, null);
                SendColor((Color)color);
			});
		}

		protected override void StopCore(bool force)
		{
			RemoveInput("Color");
		}
	}
}
