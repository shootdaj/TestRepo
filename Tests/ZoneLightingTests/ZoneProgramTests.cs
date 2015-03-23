using System.Configuration;
using System.Drawing;
using NUnit.Framework;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;
using ZoneLightingTests.Resources.Programs;

namespace ZoneLightingTests
{
	public class ZoneProgramTests
	{
		[Test]
		public void ForceStop_Works()
		{
			//arrange
			var zoneScaffolder = new ZoneScaffolder();
			zoneScaffolder.Initialize(ConfigurationManager.AppSettings["TestProgramModuleDirectory"]);

			var leftWing = new FadeCandyZone("LeftWing");
			leftWing.AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 6, 1);

			dynamic scrollDotDictionary = new ISV();
			scrollDotDictionary.DelayTime = 30;
			scrollDotDictionary.DotColor = (Color?)Color.Red;

			FadeCandyController.Instance.Initialize();	//needs to be faked somehow

			leftWing.Initialize(new ScrollDot(), scrollDotDictionary);

			//act
			leftWing.ZoneProgram.Stop(true);

			//assert
			var result = leftWing.ZoneProgram.StopTestingTrigger.WaitForFire(1000);

			//cleanup
			leftWing.Dispose();
			FadeCandyController.Instance.Dispose();

			Assert.True(result);
		}

		[Test]
		[Timeout(10000)]
		public static void CooperativeStop_Works()
		{
			DebugTools.AddEvent("Test.CooperativeStop_Works", "START CooperativeStop_Works Test");

			//arrange
			var zoneScaffolder = new ZoneScaffolder();
			zoneScaffolder.Initialize(ConfigurationManager.AppSettings["TestProgramModuleDirectory"]);

			var leftWing = new FadeCandyZone("LeftWing");
			leftWing.AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 6, 1);

			dynamic scrollDotDictionary = new ISV();
			scrollDotDictionary.DelayTime = 30;
			scrollDotDictionary.DotColor = (Color?)Color.Red;

			FadeCandyController.Instance.Initialize();	//needs to be faked somehow

			leftWing.Initialize(new ScrollDot(), scrollDotDictionary);

			//act -- cooperative stop
			leftWing.ZoneProgram.Stop(false);

			//assert
			var result = leftWing.ZoneProgram.StopTestingTrigger.WaitForFire(1000);

			//cleanup
			leftWing.Dispose();
			FadeCandyController.Instance.Dispose();

			DebugTools.AddEvent("Test.CooperativeStop_Works", "END CooperativeStop_Works Test");

			Assert.True(result);
		}
	}
}
