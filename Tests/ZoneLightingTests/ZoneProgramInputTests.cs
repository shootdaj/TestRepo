using System;
using NUnit.Framework;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	public class ZoneProgramInputTests
	{
		[Test]
		public void ZoneProgramInputCollection_CannotAddNullTypes()
		{
			Assert.Throws<Exception>(() => new ZoneProgramInputCollection().Add(null));
		}

		[Test]
		public void AddMappedInput_InputInRange_SetsInput()
		{
			var rainbow = new Rainbow();
			rainbow.SetInput("Speed", 50);
			Assert.AreEqual(rainbow.Speed, 50);
		}

		[Test]
		public void AddMappedInput_InputNotInRange_DoesNotSetInput()
		{
			var rainbow = new Rainbow();
			rainbow.SetInput("Speed", 101);
			Assert.AreNotEqual(rainbow.Speed, 101);
		}

		[Test]
		public void AddMappedInput_UnpredicatedInput_SetsInput()
		{
			var rainbow = new Rainbow() {SyncLevel = Rainbow.RainbowSyncLevel.Fade};
			rainbow.SetInput("SyncLevel", Rainbow.RainbowSyncLevel.Color);
			Assert.AreEqual(rainbow.SyncLevel, Rainbow.RainbowSyncLevel.Color);
		}
	}
}
