using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Refigure;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.Graphics.Drawing;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests.ProgramTests
{
	[Ignore("Manual Test")]
	public class VisualClockTests
	{
        [TestCase(120)]
		public void VisualClock_Works(int sleepSeconds)
		{
			
			var zlm = new ZLM(false, false, false, zlmInner =>
			{
				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", PixelType.FadeCandyWS2812Pixel,
					64, 1);
				zlmInner.CreateSingularProgramSet("", new VisualClock(), null, neomatrix, null);
			}, Config.Get("NeoMatrixOneZone"));

			//Thread.Sleep(Timeout.Infinite);
			Thread.Sleep((int)(sleepSeconds != null ? sleepSeconds * 1000 : Timeout.Infinite));

			//cleanup
			zlm.Dispose();
		}
	}
}
