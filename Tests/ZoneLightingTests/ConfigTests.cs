using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Xunit;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.ConfigNS;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLightingTests.Programs;

namespace ZoneLightingTests
{
    public class ConfigTests
    {
		[Fact]
	    public void SaveZone_SavesZoneInCorrectFormat()
	    {
			//arrange
			byte fcChannel = 1;
			var filename = @"C:\Temp\ZoneConfiguration.config";
			var zones = new List<Zone>();
			FadeCandyController.Instance.Initialize();

			zones.Add(new FadeCandyZone("TestZone1"));
			for (int i = 0; i < 6; i++)
			{
				zones[0].AddLight(new LED(logicalIndex: i, fadeCandyChannel: fcChannel, fadeCandyIndex: i));
			}
			InputStartingValues startingValuesTZ1 = new InputStartingValues();
			startingValuesTZ1.Add("DelayTime", 1);
			startingValuesTZ1.Add("Speed", 1);

			zones[0].Initialize(new Rainbow(), startingValuesTZ1);

			zones.Add(new FadeCandyZone("TestZone2"));
			for (int i = 0; i < 12; i++)
			{
				zones[1].AddLight(new LED(logicalIndex: i, fadeCandyChannel: fcChannel, fadeCandyIndex: i));
			}
			InputStartingValues startingValuesTZ2 = new InputStartingValues();
			startingValuesTZ2.Add("DelayTime", 1);
			startingValuesTZ2.Add("DotColor", Color.BlueViolet);

			zones[1].Initialize(new ScrollDot(), startingValuesTZ2);
			
			//act
			Config.SaveZones(zones, filename);
			
			//assert
			var deserializedZones = JsonConvert.DeserializeObject<IEnumerable<Zone>>(File.ReadAllText(filename), new JsonSerializerSettings
			{
				//PreserveReferencesHandling = PreserveReferencesHandling.All,
				TypeNameHandling = TypeNameHandling.All,
				Formatting = Formatting.Indented,
				//Converters = new List<JsonConverter>() { new ZonesJsonConverter() }
			}).ToList();

			Assert.Equal(zones.Count, deserializedZones.Count());
			for (var i = 0; i < zones.Count; i++)
			{
				Assert.Equal(zones[i].Name, deserializedZones[i].Name);
				Assert.Equal(zones[i].ZoneProgram.Name, deserializedZones[i].ZoneProgram.Name);
				//Assert.Equal(zones[i].ZoneProgram.ProgramParameter, deserializedZones[i].ZoneProgram.ProgramParameter);
				//TODO: Assert equality of starting input values
			}
	    }
    }
}
