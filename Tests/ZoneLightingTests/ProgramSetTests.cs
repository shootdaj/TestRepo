using System.Linq;
using NUnit.Framework;
using ZoneLighting.Communication;
using ZoneLighting.StockPrograms;
using ZoneLighting.Usables;
using ZoneLighting.Usables.TestInterfaces;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests
{
	[Category("Integration")]
	//[Ignore]
	public class ProgramSetTests
	{
		[Test]
		public void ProgramSetConstructor_WithSync_SteppersSynced_ZonesInit_ProgramsInit()
		{
			//arrange
			TestHelpers.InitializeZoneScaffolder();

			var zones = new BetterList<Zone>();
			FadeCandyController.Instance.Initialize();
			var leftWing = ZoneScaffolder.Instance.AddFadeCandyZone(zones, "LeftWing", PixelType.FadeCandyWS2812Pixel, 6, 1);
			var rightWing = ZoneScaffolder.Instance.AddFadeCandyZone(zones, "RightWing", PixelType.FadeCandyWS2811Pixel, 21, 2);

			//act
			var programSet = new ProgramSet("Stepper", zones.ToList(), true, null, "StepperSet");

			//assert
			TestHelpers.ValidateSteppersSync(((ITestProgramSet)programSet).ZoneProgramsTest.Cast<IStepper>(), 100);
			Assert.That(leftWing.Initialized, Is.True);
			Assert.That(rightWing.Initialized, Is.True);
			Assert.That(leftWing.ZoneProgram.State == ProgramState.Started, Is.True);
			Assert.That(rightWing.ZoneProgram.State == ProgramState.Started, Is.True);

			//cleanup
			programSet.Dispose();
			leftWing.Dispose();
			rightWing.Dispose();
		}

		[Test]
		public void ProgramSetConstructor_NoSync_ZonesInit_ProgramsInit()
		{
			//arrange
			TestHelpers.InitializeZoneScaffolder();

			var zones = new BetterList<Zone>();
			FadeCandyController.Instance.Initialize();
			var leftWing = ZoneScaffolder.Instance.AddFadeCandyZone(zones, "LeftWing", PixelType.FadeCandyWS2812Pixel, 6, 1);
			var rightWing = ZoneScaffolder.Instance.AddFadeCandyZone(zones, "RightWing", PixelType.FadeCandyWS2811Pixel, 21, 2);

			//act
			var programSet = new ProgramSet("Stepper", zones.ToList(), false, null, "StepperSet");

			//assert
			TestHelpers.ValidateSteppersRunning(((ITestProgramSet)programSet).ZoneProgramsTest.Cast<IStepper>(), 100);
            Assert.That(leftWing.Initialized, Is.True);
			Assert.That(rightWing.Initialized, Is.True);
			Assert.That(leftWing.ZoneProgram.State == ProgramState.Started, Is.True);
			Assert.That(rightWing.ZoneProgram.State == ProgramState.Started, Is.True);

			//cleanup
			programSet.Dispose();
			leftWing.Dispose();
			rightWing.Dispose();
		}
	}
}
