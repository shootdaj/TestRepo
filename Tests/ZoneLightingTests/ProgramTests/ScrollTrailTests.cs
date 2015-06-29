using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using NUnit.Framework;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.ConfigNS;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests.ProgramTests
{
	public class ScrollTrailTests
	{
		[TestCase(null)]
		[Ignore("Manual")]
		public void ScrollTrail_Works(int? sleepSeconds)
		{
			//act
			var zlm = new ZLM(false, false, false, zlmInner =>
			{
				var isv = new ISV();
				isv.Add("DarkenFactor", (float)0.7);
				isv.Add("DelayTime", 70);
				isv.Add("ShareShape", false);
				isv.Add("TrailShapes", new List<TrailShape>()
				{
					new TrailShape(new Trail(4, Color.Blue.Darken(0.5)), new Shape(0, 1, 2, 3, 11, 19, 27, 26, 25, 24, 16, 8)),
					new TrailShape(new Trail(4, Color.Red.Darken(0.5)), new Shape(27, 26, 25, 24, 16, 8,0, 1, 2, 3, 11, 19)),
					new TrailShape(new Trail(4, Color.Blue.Darken(0.5)), new Shape(4,5,6,7,15, 23, 31, 30, 29, 28, 20, 12)),
					new TrailShape(new Trail(4, Color.Red.Darken(0.5)), new Shape(31, 30, 29, 28, 20, 12,4,5,6,7,15, 23)),
					new TrailShape(new Trail(4, Color.Blue.Darken(0.5)), new Shape(32, 33, 34, 35, 43, 51, 59, 58, 57, 56, 48, 40)),
					new TrailShape(new Trail(4, Color.Red.Darken(0.5)), new Shape(59, 58, 57, 56, 48, 40 ,32, 33, 34, 35, 43, 51)),
					new TrailShape(new Trail(4, Color.Blue.Darken(0.5)), new Shape(36, 37, 38, 39, 47, 55, 63, 62, 61, 60, 52, 44)),
					new TrailShape(new Trail(4, Color.Red.Darken(0.5)), new Shape(63, 62, 61, 60, 52, 44,36, 37, 38, 39, 47, 55)),
				});
				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", PixelType.FadeCandyWS2812Pixel,
					64, 1);
				zlmInner.CreateSingularProgramSet("", new ScrollTrail(), isv, neomatrix);
			}, Config.Get("NeoMatrixOneZone"));

			//Thread.Sleep(Timeout.Infinite);
			Thread.Sleep((int) (sleepSeconds != null ? sleepSeconds * 1000 : Timeout.Infinite));

			//cleanup
			zlm.Dispose();
		}

		[TestCase(30, (float)0.7, 40)]
		[Ignore("Manual")]
		public void ScrollTrail_FourBlueRedSquareTrails_Works(int? sleepSeconds, float darkenFactor, int delayTime)
		{
			//act
			var zlm = new ZLM(false, false, false, zlmInner =>
			{
				var isv = new ISV();
				isv.Add("DarkenFactor", (float)darkenFactor);
				isv.Add("DelayTime", delayTime);
				isv.Add("ShareShape", true);
				isv.Add("TrailShapes", new List<TrailShape>()
				{
					new TrailShape(new Trail(4, Color.Blue.Darken(0.5)), new Shape(0, 1, 2, 3, 11, 19, 27, 26, 25, 24, 16, 8)),
					new TrailShape(new Trail(4, Color.Red.Darken(0.5)), new Shape(27, 26, 25, 24, 16, 8,0, 1, 2, 3, 11, 19)),
					new TrailShape(new Trail(4, Color.Blue.Darken(0.5)), new Shape(4,5,6,7,15, 23, 31, 30, 29, 28, 20, 12)),
					new TrailShape(new Trail(4, Color.Red.Darken(0.5)), new Shape(31, 30, 29, 28, 20, 12,4,5,6,7,15, 23)),
					new TrailShape(new Trail(4, Color.Blue.Darken(0.5)), new Shape(32, 33, 34, 35, 43, 51, 59, 58, 57, 56, 48, 40)),
					new TrailShape(new Trail(4, Color.Red.Darken(0.5)), new Shape(59, 58, 57, 56, 48, 40 ,32, 33, 34, 35, 43, 51)),
					new TrailShape(new Trail(4, Color.Blue.Darken(0.5)), new Shape(36, 37, 38, 39, 47, 55, 63, 62, 61, 60, 52, 44)),
					new TrailShape(new Trail(4, Color.Red.Darken(0.5)), new Shape(63, 62, 61, 60, 52, 44,36, 37, 38, 39, 47, 55)),
				});
				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", PixelType.FadeCandyWS2812Pixel,
					64, 1);
				zlmInner.CreateSingularProgramSet("", new ScrollTrail(), isv, neomatrix);
			}, Config.Get("NeoMatrixOneZone"));

			//Thread.Sleep(Timeout.Infinite);
			Thread.Sleep((int)(sleepSeconds != null ? sleepSeconds * 1000 : Timeout.Infinite));

			//cleanup
			zlm.Dispose();
		}
	}
}
