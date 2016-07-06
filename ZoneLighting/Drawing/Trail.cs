using System.Drawing;

namespace ZoneLighting.Drawing
{
	public class Trail
	{
		public Trail(int length, Color color)
		{
			Length = length;
			Color = color;
		}

		public int LeadIndex { get; set; } = 0;

		public int Length { get; }

		public Color? Color { get; }
	}
}