using System;
using System.Drawing;

namespace ZoneLighting.Graphics.Drawing
{
	public class Trail
	{
		public Trail(int length, Color color)
		{
			Length = length;
			Color = color;
		}

		public int LeadIndex { get; set; } = 0;

		public int Length { get; set; }

		public Color? Color { get; set; }
	}
}