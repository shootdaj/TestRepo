using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
//using Xunit;
using ZoneLighting;

namespace ZoneLightingTests
{
	public class LEDTests
	{
		//[Fact]
		[Test]
		public void SetColor_Works()
		{
			var color = A.Dummy<Color>();
			var led = new LED();
			led.SetColor(color);
			Assert.AreEqual(led.Color, color);
		}

		//[Fact]
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
			var channel = A.Dummy<byte>();
			var physicalIndex = A.Dummy<int>();
			var led = new LED();
			led.MapFadeCandyPixel(channel, physicalIndex);

			Assert.AreEqual(led.FadeCandyPixel.Channel, channel);
			Assert.AreEqual(led.FadeCandyPixel.PhysicalIndex, physicalIndex);
		}
	}
}
