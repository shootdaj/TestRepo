using System.Collections.Generic;
using System.Drawing;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
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
		public void Run_Works()
		{
			var zone = new OPCZone(FadeCandyController.Instance, "TestZone");
			var program = new Rainbow();
			zone.AddOPCLights(6);
			Assert.DoesNotThrow(() => zone.Run(program));
			Assert.True(zone.Running);
			Assert.True(program.State == ProgramState.Started);
		}

		[Test]
		public void Run_WithSync_Works()
		{
			var zone = new OPCZone(FadeCandyController.Instance, "TestZone");
			var program = new Rainbow();
			var syncContext = new SyncContext();
			zone.AddOPCLights(6);
			Assert.DoesNotThrow(() => zone.Run(program, null, true, syncContext));
			Assert.True(zone.Running);
			Assert.True(program.State == ProgramState.Started);
		}

		[Test]
		public void Stop_Works()
		{
			var lightingController = A.Fake<ILightingController>();
			lightingController.CallsTo(controller => controller.SendLights(A.Fake<List<IPixel>>())).DoesNothing();
			var zone = new OPCZone(FadeCandyController.Instance, "TestZone");
			var program = new Rainbow();
			zone.AddOPCLights(6);
			zone.Run(program);
			Assert.DoesNotThrow(() => zone.Stop(true));
			Assert.False(zone.Running);
			Assert.True(program.State == ProgramState.Stopped);
		}
	}
}
