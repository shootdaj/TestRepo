using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using Xunit;
using ZoneLighting;
using ZoneLighting.ConfigNS;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests
{
	public class ZoneScaffolderTests
	{
		[Fact]
		public void InitializeFromZoneConfiguration_Works()
		{
			//arrange
			var zones = new List<Zone>();
			var zone = new FadeCandyZone("TestZone");

			zones.Add(zone);
			zone.AddFadeCandyLights(6, 1);

			var zoneScaffolder = new ZoneScaffolder();
			zoneScaffolder.Initialize(ConfigurationManager.AppSettings["TestProgramModuleDirectory"]);

			//act
			zoneScaffolder.InitializeFromZoneConfiguration(zones, ConfigurationManager.AppSettings["TestZoneConfigurationSaveFile"]);

			//assert
			Assert.Equal(zone.Name, "TestZone");
			Assert.Equal(zone.ZoneProgram.Name, "ScrollDot");

			var inputNames = zone.ZoneProgram.GetInputNames();

			foreach (var inputName in inputNames)
			{
				if (inputName == "DelayTime")
				{
					Assert.Equal(zone.ZoneProgram.GetInputValue(inputName), 30);
				}
				else if (inputName == "DotColor")
				{
					Assert.Equal(zone.ZoneProgram.GetInputValue(inputName), Color.Red);
				}
			}
		}

		
	}
}
