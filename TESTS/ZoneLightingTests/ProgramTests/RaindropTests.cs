using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using NUnit.Framework;
using Refigure;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.Drawing;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests.ProgramTests
{
	public class RaindropTests
	{
		[TestCase(30)]
		[Ignore]
		public void Raindrops_Works(int sleepSeconds)
		{
			var zlm = new ZLM(false, false, false, zlmInner =>
			{
				dynamic startingParams = new ExpandoObject();
				startingParams.TrailShapes = new List<TrailShape>();
				startingParams.TrailShapes.Add(new TrailShape(new Trail(4, ProgramCommon.GetRandomColor()),
					new Shape(0, 1, 2, 3, 4, 5, 6, 7)));
				//startingParams.TrailShapes.Add(new TrailShape(new Trail(4, ProgramCommon.GetRandomColor()),
				//	new Shape(8, 9, 10, 11, 12, 13, 14, 15)));
				//startingParams.TrailShapes.Add(new TrailShape(new Trail(4, ProgramCommon.GetRandomColor()),
				//	new Shape(16, 17, 18, 19, 20, 21, 22, 23)));
				//startingParams.TrailShapes.Add(new TrailShape(new Trail(4, ProgramCommon.GetRandomColor()),
				//	new Shape(24, 25, 26, 27, 28, 29, 30, 31)));
				//startingParams.TrailShapes.Add(new TrailShape(new Trail(4, ProgramCommon.GetRandomColor()),
				//	new Shape(32, 33, 34, 35, 36, 37, 38, 39)));
				//startingParams.TrailShapes.Add(new TrailShape(new Trail(4, ProgramCommon.GetRandomColor()),
				//	new Shape(40, 41, 42, 43, 44, 45, 46, 47)));
				//startingParams.TrailShapes.Add(new TrailShape(new Trail(4, ProgramCommon.GetRandomColor()),
				//	new Shape(48, 49, 50, 51, 52, 53, 54, 55)));
				//startingParams.TrailShapes.Add(new TrailShape(new Trail(4, ProgramCommon.GetRandomColor()),
				//	new Shape(56, 57, 58, 59, 60, 61, 62, 63)));

				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", PixelType.FadeCandyWS2812Pixel,
					64, 1);
				zlmInner.CreateSingularProgramSet("", new Raindrops(), null, neomatrix, startingParams);
			}, Config.Get("NeoMatrixOneZone"));

			//Thread.Sleep(Timeout.Infinite);
			Thread.Sleep((int)(sleepSeconds != null ? sleepSeconds * 1000 : Timeout.Infinite));

			//cleanup
			zlm.Dispose();
		}
	}
}
