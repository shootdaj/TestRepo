using System.Collections.Generic;
using System.Drawing;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using Moq;
using NUnit.Framework;
using ZoneLighting.Communication;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	public class ZoneTests
	{
		[Test]
		public void SetAllLightsColor_Works()
		{
			var zone = A.Fake<Zone>();
			var color = A.Dummy<Color>();
			zone.SetAllLightsColor(color);

			for (int i = 0; i < zone.LightCount; i++)
			{
				Assert.AreEqual(zone.GetColor(i), color);
            }
		}

		[Test]
		public void InitializeZone_Works()
		{
			var zone = new FadeCandyZone("TestZone");
			var program = new Rainbow();
			zone.AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 6, 1);
			Assert.DoesNotThrow(() => zone.Initialize(program));
			Assert.True(zone.Initialized);
			Assert.True(program.State == ProgramState.Started);
		}

		[Test]
		public void InitializeZone_WithSync_Works()
		{
			var zone = new FadeCandyZone("TestZone");
			var program = new Rainbow();
			var syncContext = new SyncContext();
			zone.AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 6, 1);
			Assert.DoesNotThrow(() => zone.Initialize(program, null, true, syncContext));
			Assert.True(zone.Initialized);
			Assert.True(program.State == ProgramState.Started);
		}

		[Test]
		public void UninitializeZone_Works()
		{
			var lightingController = A.Fake<LightingController>();
			lightingController.CallsTo(controller => controller.SendLEDs(A.Fake<List<ILightingControllerPixel>>())).DoesNothing();
			var zone = new FadeCandyZone("TestZone", null, lightingController);
			var program = new Rainbow();
			zone.AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 6, 1);
			zone.Initialize(program);
			Assert.DoesNotThrow(() => zone.Uninitialize(true));
			Assert.False(zone.Initialized);
			Assert.True(program.State == ProgramState.Stopped);
		}


	}
}
