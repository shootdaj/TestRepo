using System;
using NUnit.Framework;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	public class ZoneProgramInputTests
	{
		//[Fact]
		[Test]
		public void ZoneProgramInputCollection_CannotAddNullTypes()
		{
			Assert.Throws<Exception>(() => new ZoneProgramInputCollection().Add(null));
		}
	}
}
