using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.ConfigNS;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLightingTests.Resources.Programs;

namespace ZoneLightingTests
{
    public class ConfigTests
    {
		/// <summary>
		/// Sets up a zone configuration, saves it, and makes sure that it deserializes with the same
		/// properties (only some properties are checked).
		/// </summary>
		//[Fact]
		[Ignore]
		[Test]
		//TODO: FIX
		public void SaveZone_SavesZoneInCorrectFormat()
		{
			//arrange
			byte fcChannel = 1;
			var filename = @"ZoneConfiguration.config";
			var zones = new BetterList<Zone>();
			FadeCandyController.Instance.Initialize();
			((FadeCandyZone) zones.Add(new FadeCandyZone("TestZone1"))).AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 6, fcChannel);

			dynamic startingValuesOldTz1 = new ISV();
			startingValuesOldTz1.DelayTime = 1;
			startingValuesOldTz1.Speed = 1;

			zones[0].Initialize(new Rainbow(), startingValuesOldTz1);

			((FadeCandyZone)zones.Add(new FadeCandyZone("TestZone2"))).AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 12, fcChannel);
			dynamic startingValuesOldTz2 = new ISV();
			startingValuesOldTz2.DelayTime = 1;
			startingValuesOldTz2.DotColor = Color.BlueViolet;

			zones[1].Initialize(new ScrollDot(), startingValuesOldTz2);

			//act
			ZoneConfig.SaveZones(zones, filename);

			//assert
			var deserializedZones = JsonConvert.DeserializeObject<IEnumerable<Zone>>(File.ReadAllText(filename), new JsonSerializerSettings
			{
				//PreserveReferencesHandling = PreserveReferencesHandling.All,
				TypeNameHandling = TypeNameHandling.All,
				Formatting = Formatting.Indented,
				//Converters = new List<JsonConverter>() { new ZonesJsonConverter() }
			}).ToList();

			Assert.AreEqual(zones.Count, deserializedZones.Count());
			for (var i = 0; i < zones.Count; i++)
			{
				Assert.AreEqual(zones[i].Name, deserializedZones[i].Name);
				Assert.AreEqual(zones[i].ZoneProgram.Name, deserializedZones[i].ZoneProgram.Name);
				//Assert.Equal(zones[i].ZoneProgram.ProgramParameter, deserializedZones[i].ZoneProgram.ProgramParameter);
				//TODO: Assert equality of starting input values
			}

			foreach (var zone in zones)
			{
				zone.Dispose(true);
			}
		}
    }
}
