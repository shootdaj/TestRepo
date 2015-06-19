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
		//[SetUp]
		//public static void Setup()
		//{
		//	ZLM.I.Initialize(loadZoneModules: false, loadZonesFromConfig:false, loadProgramSetsFromConfig:false);
		//}

		//[TearDown]
		//public static void TearDown()
		//{
		//	ZLM.I.Uninitialize();
		//}

		[Test]
		public void Initialize_JustInitActionProvided_Works()
		{
			//arrange
			var zlm = new ZLM();

			//act
			zlm.Initialize(false, false, false, TestHelpers.AddFourZonesAndStepperProgramSetWithSyncToZLM(zlm));

			//assert
			zlm.ProgramSets.ForEach(programSet =>
			{
				TestHelpers.ValidateSteppersInSync(((ITestProgramSet) programSet).ZoneProgramsTest.Cast<IStepper>(), 100);
			});

			zlm.Zones.ForEach(zone =>
			{
				Assert.That(zone.Running, Is.True);
				Assert.That(zone.ZoneProgram.State == ProgramState.Started, Is.True);
			});

			//cleanup
			zlm.Dispose();
		}

	}
}
