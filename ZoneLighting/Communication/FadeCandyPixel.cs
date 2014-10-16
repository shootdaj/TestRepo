namespace ZoneLighting.Communication
{
	/// <summary>
	/// Represents a pixel that can be controlled by FadeCandy.
	/// </summary>
	public class FadeCandyPixel : PhysicalRGBLight
	{
		public FadeCandyPixel()
		{
			
		}

		public FadeCandyPixel(byte channel, int physicalIndex)
		{
			Channel = channel;
			PhysicalIndex = physicalIndex;
		}

		public byte Channel { get; set; }
		public int RedIndex { get { return PhysicalIndex * 3; }}
		public int GreenIndex { get { return PhysicalIndex * 3 + 1; }}
		public int BlueIndex { get { return PhysicalIndex * 3 + 2; }}
	}
}