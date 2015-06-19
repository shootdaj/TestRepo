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
			
		}

		[TearDown]
		public static void TearDown()
		{
			
		}

		[Test]
		public void AvailableZones_ReturnsZonesOnWhichNoProgramSetWasCreated()
		{
			var zlm = new ZLM();
			zlm.Initialize(loadZoneModules: false, loadZonesFromConfig: false, loadProgramSetsFromConfig: false);

			var leftWing = zlm.Zones.Add(new Zone("LeftWing"));
			var center = zlm.Zones.Add(new Zone("Center"));
			var rightWing = zlm.Zones.Add(new Zone("RightWing"));

			//check if zones and availablezones are the same, since no program sets have been created yet
			Assert.AreEqual(zlm.Zones, zlm.AvailableZones);

			//create program set with all but one zone
			zlm.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
			{
				leftWing,
				center
			});

			//the zone on which no program set was created should be the only one available
			Assert.True(zlm.AvailableZones.Count == 1);
			Assert.True(zlm.AvailableZones[0] == rightWing);

			//cleanup
			zlm.Dispose();
		}

		[Test]
		public void CreateProgramSet_CreatesAndRunsProgramOnZones()
		{
			var zlm = new ZLM();
			zlm.Initialize(loadZoneModules: false, loadZonesFromConfig: false, loadProgramSetsFromConfig: false);

			var leftWing = zlm.Zones.Add(new Zone("LeftWing"));
			var center = zlm.Zones.Add(new Zone("Center"));
			var rightWing = zlm.Zones.Add(new Zone("RightWing"));

			//create program set
			zlm.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
			{
				leftWing,
				center,
				rightWing
			});

			//check that steppers are running
			TestHelpers.ValidateSteppersRunning(zlm.Zones.Select(z => z.ZoneProgram).Cast<IStepper>(), 100);

			//cleanup
			zlm.Dispose();
		}

		[Test]
		public void CreateProgramSet_TwoZonesOutOfThree_TwoZonesRunning_OneZoneNotRunning()
		{
			var zlm = new ZLM();
			zlm.Initialize(loadZoneModules: false, loadZonesFromConfig: false, loadProgramSetsFromConfig: false);

			var leftWing = zlm.Zones.Add(new Zone("LeftWing"));
			var center = zlm.Zones.Add(new Zone("Center"));
			var rightWing = zlm.Zones.Add(new Zone("RightWing"));

			//create program set
			zlm.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
			{
				leftWing,
				center,
			});

			//check that steppers are running
			Assert.False(rightWing.Running);
			TestHelpers.ValidateSteppersRunning(new BetterList<Zone> {leftWing, center}.Select(z => z.ZoneProgram).Cast<IStepper>(), 100);

			//cleanup
			zlm.Dispose();
		}

		[Test]
		public void CreateProgramSet_PassInUnavailableZone_ThrowsException()
		{
			var zlm = new ZLM();
			zlm.Initialize(loadZoneModules: false, loadZonesFromConfig: false, loadProgramSetsFromConfig: false);

			var leftWing = zlm.Zones.Add(new Zone("LeftWing"));
			var center = zlm.Zones.Add(new Zone("Center"));

			//create program set
			zlm.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
			{
				leftWing,
				center,
			});

			Assert.Throws<Exception>(() =>
				zlm.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
				{
					leftWing
				}));

			//cleanup
			zlm.Dispose();
		}

		[Test]
		public void CreateTwoProgramSets_WorksWithSync()
		{
			var zlm = new ZLM();
			zlm.Initialize(loadZoneModules: false, loadZonesFromConfig: false, loadProgramSetsFromConfig: false);

			var leftWing = zlm.Zones.Add(new Zone("LeftWing"));
			var center = zlm.Zones.Add(new Zone("Center"));
			var rightWing = zlm.Zones.Add(new Zone("RightWing"));

			//create program set
			zlm.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
			{
				leftWing,
				center,
			});

			//create program set
			zlm.CreateProgramSet("", "Stepper", true, null, new List<Zone>()
			{
				rightWing
			});

			TestHelpers.ValidateSteppersInSync(
				new BetterList<Zone> { leftWing, center }.Select(z => z.ZoneProgram).Cast<IStepper>(), 100);
			TestHelpers.ValidateSteppersRunning(
				new BetterList<Zone> { rightWing }.Select(z => z.ZoneProgram).Cast<IStepper>(), 10);

			//cleanup
			zlm.Dispose();
		}
	}
}
