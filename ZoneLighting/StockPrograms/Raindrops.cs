using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.StockPrograms
{
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "Raindrops")]
	public class Raindrops : ReactiveZoneProgram
	{
		private MicroClock Clock { get; set; }

		public long Interval
		{
			get { return Clock.Interval; }
			set { Clock.Interval = value; }
		}

		public long DriftThreshold
		{
			get { return Clock.IgnoreEventIfLateBy; }
			set { Clock.IgnoreEventIfLateBy = value; }
		}

		public int NumberOfDrops { get; set; } = 8;

		private Color _color = Color.Red;
		private ColorScheme _colorScheme = ColorScheme.All;
		
		public bool UseColor { get; set; } = true;

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
	}
}
