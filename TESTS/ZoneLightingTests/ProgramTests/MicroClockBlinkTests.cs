using System.Threading;
using NUnit.Framework;
using Refigure;
using ZoneLighting;
using ZoneLighting.StockPrograms;
using ZoneLighting.Usables;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests.ProgramTests
{
	[Ignore("Manual Test")]
	public class MicroClockBlinkTests
	{
        //test comment
		[TestCase(30, 100000)]
		[TestCase(30, 10000)]
		public void MicroClockBlink_Works(int sleepSeconds, int interval)
		{
			//act
			var zlm = new ZLM(false, false, false, zlmInner =>
			{
				var neomatrix = RunnerHelpers.CreateNeoMatrixZone(zlmInner);
				var isv = new ISV();
				isv.Add("Interval", interval);
				zlmInner.CreateSingularProgramSet("MicroClockBlinkSet", new MicroClockBlink(), isv, neomatrix);

			}, Config.Get("NeoMatrixOneZone"));

			Thread.Sleep(sleepSeconds * 1000);

			//cleanup
			zlm.Dispose();
		}
	}
}
