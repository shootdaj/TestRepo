using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Graphics;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "StopWatchBlink")]
	public class StopWatchBlink : LoopingZoneProgram, IClock
	{
	    private Color _color = Color.Red;
	    private ColorScheme _colorScheme = ColorScheme.All;

	    private long ElapsedMS { get; set; }

        public Stopwatch StopWatch { get; set; }

	    public int Period { get; set; } = 1000;

	    public int BlinkTime { get; set; } = 100;

	    public bool UseColor { get; set; } = true;

	    public bool SoftBlink { get; set; } = true;

	    public ColorScheme ColorScheme
		{
			get { return _colorScheme; }
			set
			{
				UseColor = false;
				_colorScheme = value;
			}
		}

	    public Color Color
		{
			get { return _color; }
			set
			{
				UseColor = true;
				_color = value;
			}
		}

	    public int BPM
		{
			get { return (int) TimeSpan.FromMinutes(1).TotalMilliseconds/Period; }
			set { Period = (int)((60 / (float)value) * 1000); }
		}

	    public TimeSpan PeriodTimeSpan => TimeSpan.FromMilliseconds(Period);

	    public override void Setup()
		{
			AddMappedInput<int>(this, "Period", i => i > 0);
			AddMappedInput<int>(this, "BlinkTime", i => i > 0);
			AddMappedInput<int>(this, "BPM", i => i > 0);
			AddMappedInput<ColorScheme>(this, "ColorScheme");
			AddMappedInput<Color>(this, "Color");
			AddMappedInput<bool>(this, "SoftBlink");

			StopWatch.Start();
			ElapsedMS = StopWatch.ElapsedMilliseconds;
		}

	    public override void Loop()
		{
			var currentElapsedMS = StopWatch.ElapsedMilliseconds;
			var difference = currentElapsedMS - ElapsedMS;

			if (difference > Period)
			{
				ElapsedMS = currentElapsedMS;
				Blink();
			}
		}

	    private void Blink()
		{
			Animation.SoftBlink(UseColor ? Color : ColorScheme.GetRandomSchemeColor(), BlinkTime/50, SendColor);
			//ProgramCommon.SoftBlink(UseColor ? Color : ColorScheme.GetRandomSchemeColor(), BlinkTime, SendColor);
		}
	}
}
