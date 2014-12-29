using System.Drawing;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using Xunit;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	public class ZoneTests
	{
		[Fact]
		public void SetProgram_SetsZoneProgramOfZoneAndSetsZoneOfZoneProgram()
		{
			var zone = A.Fake<Zone>();
			var zoneProgram = A.Fake<ZoneProgram>();

			zone.SetProgram(zoneProgram);

			Assert.Equal(zone.ZoneProgram, zoneProgram);
			Assert.Equal(zoneProgram.Zone, zone);
		}

		[Fact]
		public void SetAllLightsColor_Works()
		{
			var zone = A.Fake<Zone>();
			var color = A.Dummy<Color>();
			zone.SetAllLightsColor(color);

			zone.Lights.ToList().ForEach(light => Assert.Equal(light.GetColor(), color));
		}

		[Fact]
		public void StartProgram_CallsZoneProgramStart()
		{
			var zone = A.Fake<Zone>();
			zone.StartProgram();
			A.CallTo(() => zone.ZoneProgram.Start(null, A<ActionBlock<InterruptInfo>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
		}
	}
}
