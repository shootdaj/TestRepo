using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Clock;

namespace ExternalPrograms
{
    [Export(typeof(ZoneProgram))]
    [ExportMetadata("Name", "MicroTimerClockBlink")]
    public class MicroTimerClockBlink : ReactiveZoneProgram
    {
		private MicroTimerClock Clock { get; set; }

	    private int Interval { get; set; } = 1000;

	    private Color Color { get; set; } = Color.Red;

	    private long DriftThreshold { get; set; } = 500;
		
	    protected override void StartCore(dynamic parameters = null, bool forceStoppable = true)
	    {
			Clock = new MicroTimerClock(Interval, args => SendColor(Color), DriftThreshold);
			Clock.Start();
		}

	    protected override void StopCore(bool force)
	    {
		    Clock.Stop();
	    }

	    public override void Dispose(bool force)
	    {
		    Clock = null;
	    }
		
	    protected override void Setup()
		{
			AddMappedInput<int>(this, "Interval", i => i > 0);
			AddMappedInput<Color>(this, "Color");
			AddMappedInput<long>(this, "DriftThreshold", i => i > 0);
		}
    }
}
