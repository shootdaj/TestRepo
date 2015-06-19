using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ZoneLighting;
using ZoneLighting.StockPrograms;
using ZoneLighting.Usables;
using ZoneLighting.Usables.TestInterfaces;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	[TestFixture]
	[Category("Integration")]
	public class ZLMStateTests
	{
		[Test]
		public void Initialize_JustInitActionProvided_Works()
		{
			//arrange
			var zlm = new ZLM();

			//act
			zlm.Initialize(false, false, false, TestHelpers.AddFourZonesAndStepperProgramSetWithSyncToZLM(zlm));

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
		public void Uninitialize_Works()
		{
			//arrange
			var zlm = new ZLM();
			zlm.Initialize(false, false, false, TestHelpers.AddFourZonesAndStepperProgramSetWithSyncToZLM(zlm));

			//act
			zlm.Uninitialize();

			//assert
			Assert.That(zlm.ProgramSets, Is.Empty);

			zlm.Zones.ForEach(zone =>
			{
				Assert.That(zone.Running, Is.False);
				Assert.That(zone.ZoneProgram, Is.Null);
			});

			//cleanup
			zlm.Dispose();
		}
	}
}
