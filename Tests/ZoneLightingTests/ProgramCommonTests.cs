using System.Configuration;
using System.Drawing;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;
using ZoneLightingTests.Resources.Programs;

namespace ZoneLightingTests
{
	public class ProgramCommonTests
	{
		[TestCase(5, 1, 7, false, false, true)]
		[TestCase(1, 1, 7, true, false, true)]
		[TestCase(1, 1, 7, false, false, false)]
		[TestCase(7, 1, 7, false, true, true)]
		[TestCase(7, 1, 7, false, false, false)]
		[TestCase(0, 1, 7, false, false, false)]
		[TestCase(0, 1, 7, true, false, false)]
		[TestCase(0, 1, 7, false, true, false)]
		[TestCase(0, 1, 7, true, true, false)]
		[TestCase(8, 1, 7, false, false, false)]
		[TestCase(8, 1, 7, true, false, false)]
		[TestCase(8, 1, 7, false, true, false)]
		[TestCase(8, 1, 7, true, true, false)]
		public void IsInRange_Works(int input, int low, int high, bool lowInclusive, bool highInclusive, bool result)
		{
			Assert.AreEqual(input.IsInRange(low, high, lowInclusive, highInclusive), result);
		}
	}	
}
