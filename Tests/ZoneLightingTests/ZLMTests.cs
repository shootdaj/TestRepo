using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using NUnit.Framework;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.ConfigNS;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests
{
	public class ZLMTests
	{
		[Test]
		public void AvailableZones_Works()
		{
			//initialize
			ZLM.I.Initialize(false, true);

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

			//that one zone on which no program set was created should be the only one available
			Assert.True(ZLM.I.AvailableZones.Count == 1);
			Assert.True(ZLM.I.AvailableZones[0] == rightWing);
		}

		//[Test]
		public void CreateProgramSet_Works()
		{
			//todo: finish
		}

		//[Test]
		public void CreateProgramSet_PassInUnavailableZone_ThrowsException()
		{
			//todo: finish
		}
	}
}
