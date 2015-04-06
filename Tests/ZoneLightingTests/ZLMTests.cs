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
			ZLM.I.Initialize(false, true);
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
			Assert.True(TestHelpers.ValidateSteppersRunning(ZLM.I.Zones.Select(z => z.ZoneProgram).Cast<IStepper>(), 100));
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
			Assert.True(
				TestHelpers.ValidateSteppersRunning(
					new BetterList<Zone> {leftWing, center}.Select(z => z.ZoneProgram).Cast<IStepper>(), 100));
			Assert.False(rightWing.Initialized);
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

			TestHelpers.ValidateSteppersSync(
				new BetterList<Zone> {leftWing, center}.Select(z => z.ZoneProgram).Cast<IStepper>(), 100);
			TestHelpers.ValidateSteppersRunning(
				new BetterList<Zone> { rightWing }.Select(z => z.ZoneProgram).Cast<IStepper>(), 10);
		}
	}
}
