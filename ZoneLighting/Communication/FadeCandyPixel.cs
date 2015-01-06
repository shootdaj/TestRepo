namespace ZoneLighting.Communication
{
	/// <summary>
	/// Represents a pixel that can be controlled by FadeCandy.
	/// </summary>
	public abstract class FadeCandyPixel : PhysicalRGBLight
	{
		protected FadeCandyPixel()
		{
			
		}

		protected FadeCandyPixel(byte channel, int physicalIndex)
		{
			Channel = channel;
			PhysicalIndex = physicalIndex;
		}

		public byte Channel { get; set; }
		public abstract int RedIndex { get; }
		public abstract int GreenIndex { get; }
		public abstract int BlueIndex { get; }
	}

	public class FadeCandyWS2812Pixel : FadeCandyPixel
	{
		public override int RedIndex => PhysicalIndex * 3;
		public override int GreenIndex => PhysicalIndex * 3 + 1;
		public override int BlueIndex => PhysicalIndex * 3 + 2;
	}

	public class FadeCandyWS2811Pixel : FadeCandyPixel
	{
		public override int RedIndex => PhysicalIndex * 3;
		public override int GreenIndex => PhysicalIndex * 3 + 2;
		public override int BlueIndex => PhysicalIndex * 3 + 1;
	}

	public enum PixelType
	{
		None,
		FadeCandyWS2812Pixel,
		FadeCandyWS2811Pixel,
	}
}