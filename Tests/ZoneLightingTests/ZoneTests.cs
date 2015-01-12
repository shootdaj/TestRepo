using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using FakeItEasy;
using NUnit.Framework;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	public class ZoneTests
	{
		[Test]
		public void SetProgram_SetsZoneProgramOfZoneAndSetsZoneOfZoneProgram()
		{
			var zone = A.Fake<Zone>();
			var zoneProgram = A.Fake<ZoneProgram>();

			zone.SetProgram(zoneProgram);

			Assert.AreEqual(zone.ZoneProgram, zoneProgram);
			Assert.AreEqual(zoneProgram.Zone, zone);
		}

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

		[Test]
		public void StartProgram_CallsZoneProgramStart()
		{
			var zone = A.Fake<Zone>();
			zone.StartProgram();
			A.CallTo(() => zone.ZoneProgram.Start(null, A<ActionBlock<InterruptInfo>>.Ignored, A<Barrier>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
		}
	}
}
