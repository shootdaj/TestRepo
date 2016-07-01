using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		
        public MicroTimerClockBlink()
        {
			Clock = new MicroTimerClock(Interval, () => SendColor(Color), DriftThreshold);
        }
		
		protected override void SetupInputs()
		{
			AddMappedInput<int>(this, "Interval", i => i > 0);
			AddMappedInput<Color>(this, "Color");
			AddMappedInput<long>(this, "DriftThreshold", i => i > 0);
		}
    }
}
