using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.ConfigNS;
using ZoneLighting.StockPrograms;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using Rainbow = ZoneLightingTests.Resources.Programs.Rainbow;
using ScrollDot = ZoneLightingTests.Resources.Programs.ScrollDot;

namespace ZoneLightingTests
{
    public class ConfigTests
    {
		/// <summary>
		/// Sets up a zone configuration, saves it, and makes sure that it deserializes with the same
		/// properties (only some properties are checked).
		/// </summary>
		[Test]
		public void SerializeZones_SerializesZonesInCorrectFormat()
		{
            //arrange
			byte fcChannel = 1;
			var zones = new BetterList<Zone>();
			FadeCandyController.Instance.Initialize();
			((FadeCandyZone) zones.Add(new FadeCandyZone("TestZone1"))).AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 6, fcChannel);

			dynamic startingValuesOldTz1 = new ISV();
			startingValuesOldTz1.DelayTime = 1;
			startingValuesOldTz1.Speed = 1;

			zones[0].Run(new Rainbow(), startingValuesOldTz1);

			((FadeCandyZone)zones.Add(new FadeCandyZone("TestZone2"))).AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 12, fcChannel);
			dynamic startingValuesOldTz2 = new ISV();
			startingValuesOldTz2.DelayTime = 1;
			startingValuesOldTz2.DotColor = Color.BlueViolet;

			zones[1].Run(new ScrollDot(), startingValuesOldTz2);

			//act
			var serializedZones = Config.SerializeZones(zones);

			//assert
			Assert.AreEqual(serializedZones,
				"{\r\n  \"$id\": \"1\",\r\n  \"$type\": \"ZoneLighting.Usables.BetterList`1[[ZoneLighting.ZoneNS.Zone, ZoneLighting]], ZoneLighting\",\r\n  \"$values\": [\r\n    {\r\n      \"$id\": \"2\",\r\n      \"$type\": \"ZoneLighting.ZoneNS.FadeCandyZone, ZoneLighting\",\r\n      \"Name\": \"TestZone1\",\r\n      \"LightingController\": {\r\n        \"$id\": \"3\",\r\n        \"$type\": \"ZoneLighting.Communication.FadeCandyController, ZoneLighting\",\r\n        \"ServerURL\": \"ws://127.0.0.1:7890\",\r\n        \"PixelType\": \"ZoneLighting.Communication.IFadeCandyPixelContainer, ZoneLighting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\r\n        \"Initialized\": true,\r\n        \"FCServerRunning\": true,\r\n        \"ConnectionState\": 1\r\n      },\r\n      \"Brightness\": 1.0,\r\n      \"Lights\": {\r\n        \"$id\": \"4\",\r\n        \"$type\": \"System.Collections.Generic.List`1[[ZoneLighting.ILogicalRGBLight, ZoneLighting]], mscorlib\",\r\n        \"$values\": [\r\n          {\r\n            \"$id\": \"5\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"6\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 0,\r\n              \"GreenIndex\": 1,\r\n              \"BlueIndex\": 2,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 0\r\n          },\r\n          {\r\n            \"$id\": \"7\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"8\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 3,\r\n              \"GreenIndex\": 4,\r\n              \"BlueIndex\": 5,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 1\r\n          },\r\n          {\r\n            \"$id\": \"9\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"10\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 6,\r\n              \"GreenIndex\": 7,\r\n              \"BlueIndex\": 8,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 2\r\n          },\r\n          {\r\n            \"$id\": \"11\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"12\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 9,\r\n              \"GreenIndex\": 10,\r\n              \"BlueIndex\": 11,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 3\r\n          },\r\n          {\r\n            \"$id\": \"13\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"14\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 12,\r\n              \"GreenIndex\": 13,\r\n              \"BlueIndex\": 14,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 4\r\n          },\r\n          {\r\n            \"$id\": \"15\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"16\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 15,\r\n              \"GreenIndex\": 16,\r\n              \"BlueIndex\": 17,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 5\r\n          }\r\n        ]\r\n      }\r\n    },\r\n    {\r\n      \"$id\": \"17\",\r\n      \"$type\": \"ZoneLighting.ZoneNS.FadeCandyZone, ZoneLighting\",\r\n      \"Name\": \"TestZone2\",\r\n      \"LightingController\": {\r\n        \"$ref\": \"3\"\r\n      },\r\n      \"Brightness\": 1.0,\r\n      \"Lights\": {\r\n        \"$id\": \"18\",\r\n        \"$type\": \"System.Collections.Generic.List`1[[ZoneLighting.ILogicalRGBLight, ZoneLighting]], mscorlib\",\r\n        \"$values\": [\r\n          {\r\n            \"$id\": \"19\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"20\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 0,\r\n              \"GreenIndex\": 1,\r\n              \"BlueIndex\": 2,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 0\r\n          },\r\n          {\r\n            \"$id\": \"21\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"22\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 3,\r\n              \"GreenIndex\": 4,\r\n              \"BlueIndex\": 5,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 1\r\n          },\r\n          {\r\n            \"$id\": \"23\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"24\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 6,\r\n              \"GreenIndex\": 7,\r\n              \"BlueIndex\": 8,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 2\r\n          },\r\n          {\r\n            \"$id\": \"25\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"26\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 9,\r\n              \"GreenIndex\": 10,\r\n              \"BlueIndex\": 11,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 3\r\n          },\r\n          {\r\n            \"$id\": \"27\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"28\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 12,\r\n              \"GreenIndex\": 13,\r\n              \"BlueIndex\": 14,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 4\r\n          },\r\n          {\r\n            \"$id\": \"29\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"30\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 15,\r\n              \"GreenIndex\": 16,\r\n              \"BlueIndex\": 17,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 5\r\n          },\r\n          {\r\n            \"$id\": \"31\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"32\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 18,\r\n              \"GreenIndex\": 19,\r\n              \"BlueIndex\": 20,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 6\r\n          },\r\n          {\r\n            \"$id\": \"33\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"34\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 21,\r\n              \"GreenIndex\": 22,\r\n              \"BlueIndex\": 23,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 7\r\n          },\r\n          {\r\n            \"$id\": \"35\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"36\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 24,\r\n              \"GreenIndex\": 25,\r\n              \"BlueIndex\": 26,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 8\r\n          },\r\n          {\r\n            \"$id\": \"37\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"38\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 27,\r\n              \"GreenIndex\": 28,\r\n              \"BlueIndex\": 29,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 9\r\n          },\r\n          {\r\n            \"$id\": \"39\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"40\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 30,\r\n              \"GreenIndex\": 31,\r\n              \"BlueIndex\": 32,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 10\r\n          },\r\n          {\r\n            \"$id\": \"41\",\r\n            \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n            \"FadeCandyPixel\": {\r\n              \"$id\": \"42\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n              \"RedIndex\": 33,\r\n              \"GreenIndex\": 34,\r\n              \"BlueIndex\": 35,\r\n              \"Channel\": 1\r\n            },\r\n            \"LogicalIndex\": 11\r\n          }\r\n        ]\r\n      }\r\n    }\r\n  ]\r\n}");

			zones.ForEach(zone => zone.Dispose(true));
		}

		[Test]
	    public void DeserializeZones_DeserializesZonesAndBringsThemToInitialConditions()
		{
			//arrange
			byte fcChannel = 1;
			var zones = new BetterList<Zone>();
			FadeCandyController.Instance.Initialize();
			((FadeCandyZone)zones.Add(new FadeCandyZone("TestZone1"))).AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 6, fcChannel);

			dynamic startingValuesOldTz1 = new ISV();
			startingValuesOldTz1.DelayTime = 1;
			startingValuesOldTz1.Speed = 1;

			zones[0].Run(new Rainbow(), startingValuesOldTz1);

			((FadeCandyZone)zones.Add(new FadeCandyZone("TestZone2"))).AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 12, fcChannel);
			dynamic startingValuesOldTz2 = new ISV();
			startingValuesOldTz2.DelayTime = 1;
			startingValuesOldTz2.DotColor = Color.BlueViolet;

			zones[1].Run(new ScrollDot(), startingValuesOldTz2);
			var serializedZones = Config.SerializeZones(zones);

			//act
			var deserializedZones = Config.DeserializeZones(serializedZones);

			//assert
			Assert.AreEqual(zones.Count, deserializedZones.Count());
			deserializedZones.ForEach(deserializedZone =>
			{
				Assert.AreEqual(deserializedZone.LightingController, zones[deserializedZone.Name].LightingController);
				Assert.AreEqual(deserializedZone.LightingController, zones[deserializedZone.Name].LightingController);
				Assert.AreEqual(deserializedZone.Brightness, zones[deserializedZone.Name].Brightness);
				Assert.AreEqual(deserializedZone.LightCount, zones[deserializedZone.Name].LightCount);
				Assert.AreEqual(deserializedZone.Running, false);
				Assert.AreEqual(deserializedZone.IsProgramLooping, false);
				Assert.AreEqual(deserializedZone.ZoneProgram, null);
			});

			zones.ForEach(zone => zone.Dispose(true));
	    }

        [Test]
        public void SerializeProgramSets_SerializesProgramSetsInCorrectFormat()
        {
            //arrange
            TestHelpers.InitializeZoneScaffolder();

            byte fcChannel = 1;
            var zones = new BetterList<Zone>();
            FadeCandyController.Instance.Initialize();
            ((FadeCandyZone)zones.Add(new FadeCandyZone("TestZone1"))).AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 6, fcChannel);
            ((FadeCandyZone)zones.Add(new FadeCandyZone("TestZone2"))).AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 12, fcChannel);

            var programSets = new BetterList<ProgramSet>
            {
                new ProgramSet("Stepper", zones["TestZone1"].Listify(), true, null, "StepperSet1"),
                new ProgramSet("Stepper", zones["TestZone2"].Listify(), true, null, "StepperSet2")
            };

            //act
            var serializedProgramSets = Config.SerializeProgramSets(programSets);

            //assert
            Assert.AreEqual(serializedProgramSets,
                "{\r\n  \"$id\": \"1\",\r\n  \"$type\": \"ZoneLighting.Usables.BetterList`1[[ZoneLighting.ZoneProgramNS.ProgramSet, ZoneLighting]], ZoneLighting\",\r\n  \"$values\": [\r\n    {\r\n      \"$id\": \"2\",\r\n      \"$type\": \"ZoneLighting.ZoneProgramNS.ProgramSet, ZoneLighting\",\r\n      \"Name\": \"StepperSet1\",\r\n      \"Zones\": {\r\n        \"$id\": \"3\",\r\n        \"$type\": \"System.Collections.Generic.List`1[[ZoneLighting.ZoneNS.Zone, ZoneLighting]], mscorlib\",\r\n        \"$values\": [\r\n          {\r\n            \"$id\": \"4\",\r\n            \"$type\": \"ZoneLighting.ZoneNS.FadeCandyZone, ZoneLighting\",\r\n            \"Name\": \"TestZone1\",\r\n            \"LightingController\": {\r\n              \"$id\": \"5\",\r\n              \"$type\": \"ZoneLighting.Communication.FadeCandyController, ZoneLighting\",\r\n              \"ServerURL\": \"ws://127.0.0.1:7890\",\r\n              \"PixelType\": \"ZoneLighting.Communication.IFadeCandyPixelContainer, ZoneLighting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\r\n              \"Initialized\": true,\r\n              \"FCServerRunning\": true,\r\n              \"ConnectionState\": 1\r\n            },\r\n            \"Brightness\": 1.0,\r\n            \"Lights\": {\r\n              \"$id\": \"6\",\r\n              \"$type\": \"System.Collections.Generic.List`1[[ZoneLighting.ILogicalRGBLight, ZoneLighting]], mscorlib\",\r\n              \"$values\": [\r\n                {\r\n                  \"$id\": \"7\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"8\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 0,\r\n                    \"GreenIndex\": 1,\r\n                    \"BlueIndex\": 2,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 0\r\n                },\r\n                {\r\n                  \"$id\": \"9\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"10\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 3,\r\n                    \"GreenIndex\": 4,\r\n                    \"BlueIndex\": 5,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 1\r\n                },\r\n                {\r\n                  \"$id\": \"11\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"12\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 6,\r\n                    \"GreenIndex\": 7,\r\n                    \"BlueIndex\": 8,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 2\r\n                },\r\n                {\r\n                  \"$id\": \"13\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"14\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 9,\r\n                    \"GreenIndex\": 10,\r\n                    \"BlueIndex\": 11,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 3\r\n                },\r\n                {\r\n                  \"$id\": \"15\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"16\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 12,\r\n                    \"GreenIndex\": 13,\r\n                    \"BlueIndex\": 14,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 4\r\n                },\r\n                {\r\n                  \"$id\": \"17\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"18\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 15,\r\n                    \"GreenIndex\": 16,\r\n                    \"BlueIndex\": 17,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 5\r\n                }\r\n              ]\r\n            }\r\n          }\r\n        ]\r\n      },\r\n      \"ProgramName\": \"Stepper\",\r\n      \"Sync\": true\r\n    },\r\n    {\r\n      \"$id\": \"19\",\r\n      \"$type\": \"ZoneLighting.ZoneProgramNS.ProgramSet, ZoneLighting\",\r\n      \"Name\": \"StepperSet2\",\r\n      \"Zones\": {\r\n        \"$id\": \"20\",\r\n        \"$type\": \"System.Collections.Generic.List`1[[ZoneLighting.ZoneNS.Zone, ZoneLighting]], mscorlib\",\r\n        \"$values\": [\r\n          {\r\n            \"$id\": \"21\",\r\n            \"$type\": \"ZoneLighting.ZoneNS.FadeCandyZone, ZoneLighting\",\r\n            \"Name\": \"TestZone2\",\r\n            \"LightingController\": {\r\n              \"$ref\": \"5\"\r\n            },\r\n            \"Brightness\": 1.0,\r\n            \"Lights\": {\r\n              \"$id\": \"22\",\r\n              \"$type\": \"System.Collections.Generic.List`1[[ZoneLighting.ILogicalRGBLight, ZoneLighting]], mscorlib\",\r\n              \"$values\": [\r\n                {\r\n                  \"$id\": \"23\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"24\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 0,\r\n                    \"GreenIndex\": 1,\r\n                    \"BlueIndex\": 2,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 0\r\n                },\r\n                {\r\n                  \"$id\": \"25\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"26\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 3,\r\n                    \"GreenIndex\": 4,\r\n                    \"BlueIndex\": 5,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 1\r\n                },\r\n                {\r\n                  \"$id\": \"27\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"28\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 6,\r\n                    \"GreenIndex\": 7,\r\n                    \"BlueIndex\": 8,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 2\r\n                },\r\n                {\r\n                  \"$id\": \"29\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"30\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 9,\r\n                    \"GreenIndex\": 10,\r\n                    \"BlueIndex\": 11,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 3\r\n                },\r\n                {\r\n                  \"$id\": \"31\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"32\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 12,\r\n                    \"GreenIndex\": 13,\r\n                    \"BlueIndex\": 14,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 4\r\n                },\r\n                {\r\n                  \"$id\": \"33\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"34\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 15,\r\n                    \"GreenIndex\": 16,\r\n                    \"BlueIndex\": 17,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 5\r\n                },\r\n                {\r\n                  \"$id\": \"35\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"36\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 18,\r\n                    \"GreenIndex\": 19,\r\n                    \"BlueIndex\": 20,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 6\r\n                },\r\n                {\r\n                  \"$id\": \"37\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"38\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 21,\r\n                    \"GreenIndex\": 22,\r\n                    \"BlueIndex\": 23,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 7\r\n                },\r\n                {\r\n                  \"$id\": \"39\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"40\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 24,\r\n                    \"GreenIndex\": 25,\r\n                    \"BlueIndex\": 26,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 8\r\n                },\r\n                {\r\n                  \"$id\": \"41\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"42\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 27,\r\n                    \"GreenIndex\": 28,\r\n                    \"BlueIndex\": 29,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 9\r\n                },\r\n                {\r\n                  \"$id\": \"43\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"44\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 30,\r\n                    \"GreenIndex\": 31,\r\n                    \"BlueIndex\": 32,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 10\r\n                },\r\n                {\r\n                  \"$id\": \"45\",\r\n                  \"$type\": \"ZoneLighting.LED, ZoneLighting\",\r\n                  \"FadeCandyPixel\": {\r\n                    \"$id\": \"46\",\r\n                    \"$type\": \"ZoneLighting.Communication.FadeCandyWS2812Pixel, ZoneLighting\",\r\n                    \"RedIndex\": 33,\r\n                    \"GreenIndex\": 34,\r\n                    \"BlueIndex\": 35,\r\n                    \"Channel\": 1\r\n                  },\r\n                  \"LogicalIndex\": 11\r\n                }\r\n              ]\r\n            }\r\n          }\r\n        ]\r\n      },\r\n      \"ProgramName\": \"Stepper\",\r\n      \"Sync\": true\r\n    }\r\n  ]\r\n}");

            zones.ForEach(zone => zone.Dispose(true));
        }

        [Test]
        public void DeserializeProgramSets_DeserializesProgramSetsAndBringsThemToInitialConditions()
        {
            //arrange
            TestHelpers.InitializeZoneScaffolder();

            byte fcChannel = 1;
            var zones = new BetterList<Zone>();
            FadeCandyController.Instance.Initialize();
            ((FadeCandyZone)zones.Add(new FadeCandyZone("TestZone1"))).AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 6, fcChannel);
            ((FadeCandyZone)zones.Add(new FadeCandyZone("TestZone2"))).AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 12, fcChannel);

            var programSets = new BetterList<ProgramSet>
            {
                new ProgramSet("Stepper", zones["TestZone1"].Listify(), true, null, "StepperSet1"),
                new ProgramSet("Stepper", zones["TestZone2"].Listify(), true, null, "StepperSet2")
            };
            
            var serializedProgramSets = Config.SerializeProgramSets(programSets);

            //act
            var deserializedProgramSets = Config.DeserializeProgramSets(serializedProgramSets, zones);

            //assert
            Assert.AreEqual(programSets.Count, deserializedProgramSets.Count());
            deserializedProgramSets.ForEach(deserializedProgramSet =>
            {
                Assert.AreEqual(deserializedProgramSet.ProgramName, programSets[deserializedProgramSet.Name].ProgramName);
                Assert.AreEqual(deserializedProgramSet.Sync, programSets[deserializedProgramSet.Name].Sync);
                Assert.AreEqual(deserializedProgramSet.Zones, programSets[deserializedProgramSet.Name].Zones);
            });

            TestHelpers.ValidateSteppersRunning(zones.Select(z => z.ZoneProgram).Cast<IStepper>(), 100);

            zones.ForEach(zone => zone.Dispose(true));
        }
    }
}
