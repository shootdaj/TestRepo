using System;
using System.Linq;

namespace ZoneLighting.Drawing
{
	public class Shape
	{
		public Shape(params int[] pixels)
		{
			Pixels = pixels;
		}

		public int[] Pixels { get; }

		public int GetNextIndex(int index) => index == Pixels.Length - 1 ? 0 : index + 1;
		public int GetPreviousIndex(int index, out bool overflow)
		{
			overflow = false;
			if (index == 0)
			{
				overflow = true;
				return Pixels.Length - 1;
			}
			else
				return index - 1;
		}

		public int GetNextPixel(int pixel) => Pixels.Last() == pixel ? Pixels.First() : Pixels[Array.IndexOf(Pixels, pixel) + 1];

		public int GetPreviousPixel(int pixel) => Pixels.First() == pixel ? Pixels.Last() : Pixels[Array.IndexOf(Pixels, pixel) - 1];

		public int PixelCount => Pixels.Length;
	}
}