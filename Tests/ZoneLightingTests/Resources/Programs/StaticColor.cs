using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading;
using ZoneLighting.Communication;
using ZoneLighting.TriggerDependencyNS;
using ZoneLighting.ZoneNS;
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

		protected override void StartCore()
		{
			AddInterruptingInput<Color>("Color", color =>
			{
				ChangeLightColorTrigger.Fire(this, null);
                SetColor((Color)color);
				SendLights();	//send frame
			});
		}

		protected override void StopCore(bool force)
		{
			RemoveInput("Color");
		}
	}
}
