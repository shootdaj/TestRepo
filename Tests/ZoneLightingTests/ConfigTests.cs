using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using Xunit;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;
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
			zones[0].Initialize(new Rainbow(), new RainbowParameter() { DelayTime = 1, Speed = 1 });

			zones.Add(new FadeCandyZone("TestZone2"));
			for (int i = 0; i < 12; i++)
			{
				zones[1].AddLight(new LED(logicalIndex: i, fadeCandyChannel: fcChannel, fadeCandyIndex: i));
			}
			zones[1].Initialize(new ScrollDot(), new ScrollDotParameter() { DelayTime = 1, Color = Color.BlueViolet });
			
			//act
			ZoneLighting.Config.Config.SaveZone(zones, filename);
			
			//assert
			zones.ForEach(x => x.Dispose());

			var deserializedZones = JsonConvert.DeserializeObject<IEnumerable<Zone>>(File.ReadAllText(filename), new JsonSerializerSettings
			{
				PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All,
				TypeNameHandling = TypeNameHandling.All,
				Formatting = Formatting.Indented
			});

			

			//var fileContents = File.ReadAllText(filename);
			Assert.Equal(zones, deserializedZones);
	    }
    }
}
