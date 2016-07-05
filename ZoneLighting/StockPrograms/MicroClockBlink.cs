using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.StockPrograms
{
    [Export(typeof(ZoneProgram))]
    [ExportMetadata("Name", "MicroClockBlink")]
    public class MicroClockBlink : ReactiveZoneProgram
    {
		private MicroClock Clock { get; set; }

	    private long Interval
	    {
		    get { return Clock.Interval; }
		    set { Clock.Interval = value; }
	    }

	    private Color Color { get; set; } = Color.Red;

	    private long DriftThreshold
	    {
		    get { return Clock.IgnoreEventIfLateBy; }
			set { Clock.IgnoreEventIfLateBy = value; }
	    } 

	    protected override void StartCore(dynamic parameters = null, bool forceStoppable = true)
	    {
		    Clock = new MicroClock(500000,
			    args => SendColor(Color.ToArgb() != Zone.SortedLights.First().Value.GetColor().ToArgb() ? Color : Color.Black), DriftThreshold);
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
			AddMappedInput<long>(this, "Interval", i => i > 0);
			AddMappedInput<Color>(this, "Color");
			AddMappedInput<long>(this, "DriftThreshold", i => i > 0);
		}
    }
}
