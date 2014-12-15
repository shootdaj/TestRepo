using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ZoneLighting;
using ZoneLighting.ConfigNS;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;
using ZoneLightingTests.Programs;

namespace ZoneLightingTests
{
	public class ZoneScaffolderTests
	{
		/// <summary>
		/// Makes sure that Initi
		/// </summary>
		[Fact]
		public void InitializeFromZoneConfiguration_Works()
		{
			//arrange
			var zones = new List<Zone>();
			var zone = new FadeCandyZone("TestZone");

			zones.Add(zone);
			var numLights = 6;
			byte fcChannel = 1;
			for (int i = 0; i < numLights; i++)
			{
				zone.AddLight(new LED(logicalIndex: i, fadeCandyChannel: fcChannel, fadeCandyIndex: i));
			}

			InputStartingValues startingValues = new InputStartingValues {{"DelayTime", 30}, {"DotColor", Color.Red}};
			var zoneProgram = new ScrollDot();

			//initialize zone
			ZoneScaffolder.Instance.InitializeZone(zone, zoneProgram, startingValues);

			//save zone
			Config.SaveZones(zones, ConfigurationManager.AppSettings["TestZoneConfigurationSaveFile"]);

			//uninitialize
			zones.ToList().ForEach(z => z.Uninitialize(true));

			//act
			ZoneScaffolder.Instance.InitializeFromZoneConfiguration(zones, ConfigurationManager.AppSettings["TestZoneConfigurationSaveFile"]);

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
