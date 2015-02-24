using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using NUnit.Framework;
using ZoneLighting.Communication;
using ZoneLighting.ConfigNS;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;
//using Xunit;

namespace ZoneLightingTests
{
	public class ZoneScaffolderTests
	{
		[Ignore]
		[Test]
		//TODO: Fix
		public void InitializeFromZoneConfiguration_Works()
		{
			//arrange
			var zones = new List<Zone>();
			var zone = new FadeCandyZone("TestZone");

			zones.Add(zone);
			zone.AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 6, 1);

			var zoneScaffolder = new ZoneScaffolder();
			zoneScaffolder.Initialize(ConfigurationManager.AppSettings["TestProgramModuleDirectory"]);

			//act
			zoneScaffolder.InitializeFromZoneConfiguration(zones, ConfigurationManager.AppSettings["InitializeFromZoneConfiguration_Works_TestFile"]);

			//assert
			Assert.AreEqual(zone.Name, "TestZone");
			Assert.AreEqual(zone.ZoneProgram.Name, "ScrollDot");

			var inputNames = zone.ZoneProgram.GetInputNames();

			foreach (var inputName in inputNames)
			{
				if (inputName == "DelayTime")
				{
					Assert.AreEqual(zone.ZoneProgram.GetInputValue(inputName), 30);
				}
				else if (inputName == "DotColor")
				{
					Assert.AreEqual(zone.ZoneProgram.GetInputValue(inputName), Color.Red);
				}
			}
		}

		/// <summary>
		/// This is jsut there to generate the test file for this test in case the code changes something
		/// fundamental that is reflected in the saved configuration.
		/// </summary>
		/// <param name="filename"></param>
		[Ignore]
		[TestCase(@"C:\Temp\test.config")] //NOTE: Insert file path in test case to generate to the path.
		public void GenerateTestConfiguration(string filename)
		{
			var zoneScaffolder = new ZoneScaffolder();
			zoneScaffolder.Initialize(ConfigurationManager.AppSettings["TestProgramModuleDirectory"]);

			var leftWing = new FadeCandyZone("TestZone");
			leftWing.AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 6, 1);

			dynamic scrollDotDictionary = new InputStartingValues();
			scrollDotDictionary.DelayTime = 30;
			scrollDotDictionary.DotColor = (Color?) Color.Red;

			zoneScaffolder.InitializeZone(leftWing, "ScrollDot", scrollDotDictionary);

			Config.SaveZones(new List<Zone>() {leftWing}, filename);
		}
	}
}
