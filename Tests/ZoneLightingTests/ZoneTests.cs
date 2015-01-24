using System.Drawing;
using FakeItEasy;
using NUnit.Framework;
using ZoneLighting.ZoneNS;

namespace ZoneLightingTests
{
	public class ZoneTests
	{
		//[Test]
		//public void SetProgram_SetsZoneProgramOfZoneAndSetsZoneOfZoneProgram()
		//{
		//	var zone = A.Fake<Zone>();
		//	var zoneProgram = A.Fake<ZoneProgram>();

		//	zone.SetProgram(zoneProgram);

		//	Assert.AreEqual(zone.ZoneProgram, zoneProgram);
		//	Assert.AreEqual(zoneProgram.Zone, zone);
		//}

		[Test]
		public void SetAllLightsColor_Works()
		{
			var zone = A.Fake<Zone>();
			var color = A.Dummy<Color>();
			zone.SetAllLightsColor(color);

			for (int i = 0; i < zone.LightCount; i++)
			{
				Assert.AreEqual(zone.GetColor(i), color);
            }
		}
	}
}
