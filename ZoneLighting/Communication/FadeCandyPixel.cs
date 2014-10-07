using System.Collections;
using System.Collections.Generic;

namespace ZoneLighting.Communication
{
	/// <summary>
	/// Represents a pixel that can be controlled by FadeCandy.
	/// </summary>
	public class FadeCandyPixel
	{
		public int Index { get; set; }
		public int RedIndex { get { return Index * 3; }}
		public int GreenIndex { get { return Index * 3 + 1; }}
		public int BlueIndex { get { return Index * 3 + 2; }}
	}

	/// <summary>
	/// This MI (multiple inheritance) class allows multiple inheritance from FadeCandyPixel.
	/// </summary>
	public class FadeCandyPixelMI : FadeCandyPixel
	{
		private LED LEDPart;

		public static implicit operator LED(FadeCandyPixelMI fadeCandyPixel)
		{
			return fadeCandyPixel.LEDPart;
		}
	}
}