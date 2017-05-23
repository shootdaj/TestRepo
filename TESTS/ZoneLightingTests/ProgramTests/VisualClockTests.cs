using System.Threading;
using NUnit.Framework;
using Refigure;
using ZoneLighting;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests.ProgramTests
{
    [Explicit("Manual Test")]
    public class VisualClockTests
	{
        [TestCase(120)]
		public void VisualClock_Works(int sleepSeconds)
		{
			
			var zlm = new ZLM(false, false, false, zlmInner =>
			{
				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", 64);
				zlmInner.CreateSingularProgramSet("", new VisualClock(), null, neomatrix, null);
			}, Config.Get("NeoMatrixOneZone"));

			//Thread.Sleep(Timeout.Infinite);
			Thread.Sleep((int)(sleepSeconds != null ? sleepSeconds * 1000 : Timeout.Infinite));

			//cleanup
			zlm.Dispose();
		}
	}
}
