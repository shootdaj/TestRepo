using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ZoneLighting;
using ZoneLighting.StockPrograms;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;

namespace ZoneLightingTests
{
	[TestFixture]
	[Category("Integration")]
	public class ZLMTests
	{
		[SetUp]
		public static void Setup()
		{
			ZLM.I.Initialize();
		}

		[TearDown]
		public static void TearDown()
		{
			ZLM.I.Uninitialize();
		}

		[Test]
		public void AvailableZones_ReturnsZonesOnWhichNoProgramSetWasCreated()
		{
			var leftWing = ZLM.I.Zones.Add(new Zone("LeftWing"));
			var center = ZLM.I.Zones.Add(new Zone("Center"));
			var rightWing = ZLM.I.Zones.Add(new Zone("RightWing"));

			//check if zones and availablezones are the same, since no program sets have been created yet
			Assert.AreEqual(ZLM.I.Zones, ZLM.I.AvailableZones);

			//create program set with all but one zone
			ZLM.I.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
			{
				leftWing,
				center
			});

			//the zone on which no program set was created should be the only one available
			Assert.True(ZLM.I.AvailableZones.Count == 1);
			Assert.True(ZLM.I.AvailableZones[0] == rightWing);
		}

		[Test]
		public void CreateProgramSet_CreatesAndRunsProgramOnZones()
		{
			var leftWing = ZLM.I.Zones.Add(new Zone("LeftWing"));
			var center = ZLM.I.Zones.Add(new Zone("Center"));
			var rightWing = ZLM.I.Zones.Add(new Zone("RightWing"));

			//create program set
			ZLM.I.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
			{
				leftWing,
				center,
				rightWing
			});

			//check that steppers are running
			TestHelpers.ValidateSteppersRunning(ZLM.I.Zones.Select(z => z.ZoneProgram).Cast<IStepper>(), 100);
		}

		[Test]
		public void CreateProgramSet_TwoZonesOutOfThree_TwoZonesRunning_OneZoneNotRunning()
		{
			var leftWing = ZLM.I.Zones.Add(new Zone("LeftWing"));
			var center = ZLM.I.Zones.Add(new Zone("Center"));
			var rightWing = ZLM.I.Zones.Add(new Zone("RightWing"));

			//create program set
			ZLM.I.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
			{
				leftWing,
				center,
			});

			//check that steppers are running
			Assert.False(rightWing.Initialized);
			TestHelpers.ValidateSteppersRunning(new BetterList<Zone> {leftWing, center}.Select(z => z.ZoneProgram).Cast<IStepper>(), 100);
		}

		[Test]
		public void CreateProgramSet_PassInUnavailableZone_ThrowsException()
		{
			var leftWing = ZLM.I.Zones.Add(new Zone("LeftWing"));
			var center = ZLM.I.Zones.Add(new Zone("Center"));

			//create program set
			ZLM.I.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
			{
				leftWing,
				center,
			});

			Assert.Throws<Exception>(() =>
				ZLM.I.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
				{
					leftWing
				}));
		}

		[Test]
		public void CreateTwoProgramSets_WorksWithSync()
		{
			var leftWing = ZLM.I.Zones.Add(new Zone("LeftWing"));
			var center = ZLM.I.Zones.Add(new Zone("Center"));
			var rightWing = ZLM.I.Zones.Add(new Zone("RightWing"));

			//create program set
			ZLM.I.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
			{
				leftWing,
				center,
			});

			//create program set
			ZLM.I.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
			{
				rightWing
			});

			TestHelpers.ValidateSteppersInSync(
				new BetterList<Zone> { leftWing, center }.Select(z => z.ZoneProgram).Cast<IStepper>(), 100);
			TestHelpers.ValidateSteppersRunning(
				new BetterList<Zone> { rightWing }.Select(z => z.ZoneProgram).Cast<IStepper>(), 10);
		}

		[Test]
		public void MoveZone_MovesZoneFromOneProgramSetToAnother()
		{
			//arrange
			var leftWing = ZLM.I.Zones.Add(new Zone("LeftWing"));
			var rightWing = ZLM.I.Zones.Add(new Zone("RightWing"));

			//create program set
			var stepperSet1 = ZLM.I.CreateProgramSet("StepperSet1", "Stepper", true, null, new List<Zone>()
			{
				leftWing
			});

			//create program set
			var stepperSet2 = ZLM.I.CreateProgramSet("StepperSet2", "Stepper", true, null, new List<Zone>()
			{
				rightWing
			});

			//act
			ZLM.I.MoveZone(rightWing, stepperSet1);

			//assert
			Assert.That(stepperSet2.Zones, Is.Empty);
			TestHelpers.ValidateSteppersInSync(
				new BetterList<Zone> { leftWing, rightWing }.Select(z => z.ZoneProgram).Cast<IStepper>(), 100);
		}

		[Test]
		public void MoveZone_SameProgramSet_ThrowException()
		{
			//arrange
			var leftWing = ZLM.I.Zones.Add(new Zone("LeftWing"));
			var rightWing = ZLM.I.Zones.Add(new Zone("RightWing"));

			//create program set
			var stepperSet1 = ZLM.I.CreateProgramSet("StepperSet1", "Stepper", true, null, new List<Zone>()
			{
				leftWing
			});

			//create program set
			var stepperSet2 = ZLM.I.CreateProgramSet("StepperSet2", "Stepper", true, null, new List<Zone>()
			{
				rightWing
			});

			//act and assert
			Assert.Throws<Exception>(() => ZLM.I.MoveZone(rightWing, stepperSet2));
		}
	}
}
