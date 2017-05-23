using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using NUnit.Framework;
using Refigure;
using ZoneLighting;
using ZoneLighting.Graphics.Drawing;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests.ProgramTests
{
	[Explicit("Manual Test")]
	public class ScrollTrailTests
	{
		[TestCase(3000)]
		public void ScrollTrail_Works(int? sleepSeconds)
		{
			//act
			var zlm = new ZLM(false, false, false, zlmInner =>
			{
				var isv = new ISV();
				isv.Add("DarkenFactor", (float)0.7);
				isv.Add("DelayTime", 20);
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
				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", 64);
				zlmInner.CreateSingularProgramSet("", new ScrollTrail(), isv, neomatrix);
			}, Config.Get("NeoMatrixOneZone"));

			//Thread.Sleep(Timeout.Infinite);
			Thread.Sleep((int) (sleepSeconds != null ? sleepSeconds * 1000 : Timeout.Infinite));

			//cleanup
			zlm.Dispose();
		}

		[TestCase(30, (float)0.7, 40)]
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
				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", 64);
				zlmInner.CreateSingularProgramSet("", new ScrollTrail(), isv, neomatrix);
			}, Config.Get("NeoMatrixOneZone"));

			//Thread.Sleep(Timeout.Infinite);
			Thread.Sleep((int)(sleepSeconds != null ? sleepSeconds * 1000 : Timeout.Infinite));

			//cleanup
			zlm.Dispose();
		}

		[TestCase(30, (float)0.7, 40)]
		public void ScrollTrail_FourRandomColorsSquareTrails_Works(int? sleepSeconds, float darkenFactor, int delayTime)
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
					new TrailShape(new Trail(4, ProgramCommon.GetRandomColor().Darken(0.5)), new Shape(0, 1, 2, 3, 11, 19, 27, 26, 25, 24, 16, 8)),
					new TrailShape(new Trail(4, ProgramCommon.GetRandomColor().Darken(0.5)), new Shape(27, 26, 25, 24, 16, 8,0, 1, 2, 3, 11, 19)),
					new TrailShape(new Trail(4, ProgramCommon.GetRandomColor().Darken(0.5)), new Shape(4,5,6,7,15, 23, 31, 30, 29, 28, 20, 12)),
					new TrailShape(new Trail(4, ProgramCommon.GetRandomColor().Darken(0.5)), new Shape(31, 30, 29, 28, 20, 12,4,5,6,7,15, 23)),
					new TrailShape(new Trail(4, ProgramCommon.GetRandomColor().Darken(0.5)), new Shape(32, 33, 34, 35, 43, 51, 59, 58, 57, 56, 48, 40)),
					new TrailShape(new Trail(4, ProgramCommon.GetRandomColor().Darken(0.5)), new Shape(59, 58, 57, 56, 48, 40 ,32, 33, 34, 35, 43, 51)),
					new TrailShape(new Trail(4, ProgramCommon.GetRandomColor().Darken(0.5)), new Shape(36, 37, 38, 39, 47, 55, 63, 62, 61, 60, 52, 44)),
					new TrailShape(new Trail(4, ProgramCommon.GetRandomColor().Darken(0.5)), new Shape(63, 62, 61, 60, 52, 44,36, 37, 38, 39, 47, 55)),
				});
				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", 64);
				zlmInner.CreateSingularProgramSet("", new ScrollTrail(), isv, neomatrix);
			}, Config.Get("NeoMatrixOneZone"));

			//Thread.Sleep(Timeout.Infinite);
			Thread.Sleep((int)(sleepSeconds != null ? sleepSeconds * 1000 : Timeout.Infinite));

			//cleanup
			zlm.Dispose();
		}

		[TestCase(30)]
		public void RaindropsOnScrollTrail_Works(int? sleepSeconds)
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
					new TrailShape(new Trail(4, Color.Blue.Darken(0.5)), new Shape(0, 1, 2, 3, 4, 5, 6, 7)),
					new TrailShape(new Trail(4, Color.Red.Darken(0.5)), new Shape(8, 9, 10, 11, 12, 13, 14, 15)),
					new TrailShape(new Trail(4, Color.Blue.Darken(0.5)), new Shape(16, 17, 18, 19, 20, 21, 22, 23)),
					new TrailShape(new Trail(4, Color.Red.Darken(0.5)), new Shape(24, 25, 26, 27, 28, 29, 30, 31)),
					new TrailShape(new Trail(4, Color.Blue.Darken(0.5)), new Shape(32, 33, 34, 35, 36, 37, 38, 39)),
					new TrailShape(new Trail(4, Color.Red.Darken(0.5)), new Shape(40, 41, 42, 43, 44, 45, 46, 47)),
					new TrailShape(new Trail(4, Color.Blue.Darken(0.5)), new Shape(48, 49, 50, 51, 52, 53, 54, 55)),
					new TrailShape(new Trail(4, Color.Red.Darken(0.5)), new Shape(56, 57, 58, 59, 60, 61, 62, 63)),
				});
				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", 64);
				zlmInner.CreateSingularProgramSet("", new ScrollTrail(), isv, neomatrix);
			}, Config.Get("NeoMatrixOneZone"));

			//Thread.Sleep(Timeout.Infinite);
			Thread.Sleep((int)(sleepSeconds != null ? sleepSeconds * 1000 : Timeout.Infinite));

			//cleanup
			zlm.Dispose();
		}
	}
}
