using System;
using System.Collections.Generic;
using System.Dynamic;
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
    public class RaindropTests
	{
		public int GetNewInterval()
		{
			var value = ProgramCommon.RandomIntBetween(IntervalAvg - IntervalVariability, IntervalAvg + IntervalVariability);
			return value;
		}

		public int IntervalAvg { get; set; }

		public int IntervalVariability { get; set; }

		[TestCase(360, 4, 2, 200, 70)]
		[TestCase(360, 4, 2, 70, 70)]
		[TestCase(360, 4, 2, 40, 30)]
		public void Raindrops_Works(int sleepSeconds, int trailLengthAvg, int trailLengthVariability, int intervalAvg, int intervalVariability)
		{
			IntervalAvg = intervalAvg;
			IntervalVariability = intervalVariability;

			var zlm = new ZLM(false, false, false, zlmInner =>
			{
				dynamic startingParams = new ExpandoObject();
				startingParams.ClockedTrailShapes = new List<dynamic>();

				for (int i = 0; i < 64; i+=8)
				{
					dynamic clockedTrailShape = new ExpandoObject();
					var trailLength = ProgramCommon.RandomIntBetween(trailLengthAvg - trailLengthVariability, trailLengthAvg + trailLengthVariability);
					var interval = ProgramCommon.RandomIntBetween(intervalAvg - intervalVariability, intervalAvg + intervalVariability);

					var darkenFactor = (float)0.7;
					clockedTrailShape.TrailShape = new TrailShape(new Trail(trailLength, ProgramCommon.GetRandomColor().Darken(0.5)),
						new Shape(i, i+1, i+2, i+3, i+4, i+5, i+6, i+7));
					clockedTrailShape.TrailShape.DarkenFactor = darkenFactor;
					clockedTrailShape.Interval = interval;
					clockedTrailShape.GetNewInterval = (Func<int>) GetNewInterval;
					clockedTrailShape.AutoTrail = true; //todo: implement autotrail 
					clockedTrailShape.AutoSpeed = true;	//todo: implement autospeed
					startingParams.ClockedTrailShapes.Add(clockedTrailShape);
				}
				
				//startingParams.ClockedTrailShapes.Add(new ExpandoObject() )};
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

				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", 64);
				zlmInner.CreateSingularProgramSet("", new Raindrops(), null, neomatrix, startingParams);
			}, Config.Get("NeoMatrixOneZone"));

			//Thread.Sleep(Timeout.Infinite);
			Thread.Sleep((int)(sleepSeconds != null ? sleepSeconds * 1000 : Timeout.Infinite));

			//cleanup
			zlm.Dispose();
		}
	}
}
