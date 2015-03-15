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

		/// <summary>
		/// AddMappedInput can be called with a predicate, which determines whether or not the input will 
		/// be set. It's a filtering mechanism. This test is essentially calling that AddMappedInput without any
		/// predicates because SyncLevel is not set with any predicates (meaning it can be set to anything as long
		/// as the types match).
		/// </summary>
		[Test]
		public void AddMappedInput_UnpredicatedInput_SetsInput()
		{
			var rainbow = new Rainbow() {SyncLevel = Rainbow.RainbowSyncLevel.Fade};
			rainbow.SetInput("SyncLevel", Rainbow.RainbowSyncLevel.Color);
			Assert.AreEqual(rainbow.SyncLevel, Rainbow.RainbowSyncLevel.Color);
		}
	}
}
