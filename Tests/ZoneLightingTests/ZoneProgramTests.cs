using System.Configuration;
using System.Drawing;
using Xunit;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests
{
	public class ZoneProgramTests
	{
		[Fact]
		public void ForceStop_Works()
		{
			//arrange
			var zoneScaffolder = new ZoneScaffolder();
			zoneScaffolder.Initialize(ConfigurationManager.AppSettings["TestProgramModuleDirectory"]);

			var leftWing = new FadeCandyZone("LeftWing");
			leftWing.AddFadeCandyLights(6, 1);

			var scrollDotDictionary = new InputStartingValues();
			scrollDotDictionary.Add("DelayTime", 30);
			scrollDotDictionary.Add("DotColor", (Color?)Color.Red);

			FadeCandyController.Instance.Initialize();	//needs to be faked somehow

			zoneScaffolder.InitializeZone(leftWing, "ScrollDot", scrollDotDictionary);

			//act
			leftWing.ZoneProgram.Stop(true);

			//assert
			Assert.True(leftWing.ZoneProgram.StopTestingTrigger.WaitForFire(1000));

			//cleanup
			leftWing.Dispose();

			
		}

		[Fact]
		public void CooperativeStop_Works()
		{
			//arrange
			var zoneScaffolder = new ZoneScaffolder();
			zoneScaffolder.Initialize(ConfigurationManager.AppSettings["TestProgramModuleDirectory"]);

			var leftWing = new FadeCandyZone("LeftWing");
			leftWing.AddFadeCandyLights(6, 1);

			var scrollDotDictionary = new InputStartingValues();
			scrollDotDictionary.Add("DelayTime", 30);
			scrollDotDictionary.Add("DotColor", (Color?)Color.Red);

			FadeCandyController.Instance.Initialize();	//needs to be faked somehow

			zoneScaffolder.InitializeZone(leftWing, "ScrollDot", scrollDotDictionary);

			//act
			leftWing.ZoneProgram.Stop(false);

			//assert
			Assert.True(leftWing.ZoneProgram.StopTestingTrigger.WaitForFire(1000));

			//cleanup
			leftWing.Dispose();
		}
	}
}
