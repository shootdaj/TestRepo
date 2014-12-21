using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Xunit;
using ZoneLighting;

namespace ZoneLightingTests
{
	public class LEDTests
	{
		[Fact]
		public void SetColor_Works()
		{
			var color = A.Dummy<Color>();
			var led = new LED();
			led.SetColor(color);
			Assert.Equal(led.Color, color);
		}

		[Fact]
		public void GetColor_Works()
		{
			var color = A.Dummy<Color>();
			var led = new LED();
			led.Color = color;
			Assert.Equal(led.GetColor(), color);
		}

		public void MapFadeCandyPixel_Works()
		{
			var channel = A.Dummy<byte>();
			var physicalIndex = A.Dummy<int>();
			var led = new LED();
			led.MapFadeCandyPixel(channel, physicalIndex);

			Assert.Equal(led.FadeCandyPixel.Channel, channel);
			Assert.Equal(led.FadeCandyPixel.PhysicalIndex, physicalIndex);
		}
	}
}
