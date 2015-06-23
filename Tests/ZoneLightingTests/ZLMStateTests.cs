using System.Linq;
using NUnit.Framework;
using ZoneLighting;
using ZoneLighting.StockPrograms;
using ZoneLighting.Usables.TestInterfaces;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests
{
	[TestFixture]
	[Category("Integration")]
	public class ZLMStateTests
	{
		[Test]
		public void Constructor_JustInitActionProvided_Works()
		{
			//act
			var zlm = new ZLM(false, false, false, TestHelpers.AddFourZonesAndStepperProgramSetWithSyncToZLM);

			//assert
			TestHelpers.ValidateSteppersInSync(
				zlm.ProgramSets.SelectMany(ps => ((ITestProgramSet) ps).ZoneProgramsTest).Cast<IStepper>(), 100);

			zlm.Zones.ForEach(zone =>
			{
				Assert.That(zone.Running, Is.True);
				Assert.That(zone.ZoneProgram.State == ProgramState.Started, Is.True);
			});

			//cleanup
			zlm.Dispose();
		}

		[Test]
		public void Dispose_Works()
		{
			//arrange
			var zlm = new ZLM(false, false, false, TestHelpers.AddFourZonesAndStepperProgramSetWithSyncToZLM);

			//act
			zlm.Dispose();

			//assert
			Assert.That(zlm.ProgramSets, Is.Null);
			Assert.That(zlm.Zones, Is.Null);
			//TODO: Assert that all lighting controllers are unintialized
			Assert.That(ZoneScaffolder.Instance.Initialized, Is.False);
		}
	}
}
