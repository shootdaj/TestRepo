using System.Drawing;
using FakeItEasy;
using NUnit.Framework;
using ZoneLighting;
using ZoneLighting.Communication;

namespace ZoneLightingTests
{
	public class LEDTests
	{
		[Test]
		public void SetColor_Works()
		{
			var color = A.Dummy<Color>();
			var led = new LED();
			led.SetColor(color);
			Assert.AreEqual(led.Color, color);
		}

		[Test]
		public void GetColor_Works()
		{
			var color = A.Dummy<Color>();
			var led = new LED();
			led.Color = color;
			Assert.AreEqual(led.GetColor(), color);
		}

		[Test]
		public void MapFadeCandyPixel_Works()
		{
			byte channel = 1;//A.Dummy<byte>();
			int physicalIndex = 1;//A.Dummy<int>();
			var led = new LED(pixelType: PixelType.FadeCandyWS2812Pixel);
			led.MapFadeCandyPixel(channel, physicalIndex);

			Assert.AreEqual(led.FadeCandyPixel.Channel, channel);
			Assert.AreEqual(led.FadeCandyPixel.PhysicalIndex, physicalIndex);
		}
	}
}
