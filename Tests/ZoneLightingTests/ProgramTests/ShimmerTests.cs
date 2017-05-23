using System.Threading;
using NUnit.Framework;
using Refigure;
using ZoneLighting;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests.ProgramTests
{
    [Explicit("Manual Test")]
    public class ShimmerTests
	{
		[TestCase(300, true)]
		[TestCase(30, false)]
		public void Shimmer_Works(int sleepSeconds, bool random)
		{
			//act
			var zlm = new ZLM(false, false, false, zlmInner =>
			{
				var isv = new ISV();
				isv.Add("MaxFadeSpeed", 1);
				isv.Add("MaxFadeDelay", 20);
				isv.Add("Density", 0.5);
				isv.Add("Brightness", 0.5);
				isv.Add("Random", random);
				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", 64);
				zlmInner.CreateSingularProgramSet("", new Shimmer(), isv, neomatrix);
			}, Config.Get("NeoMatrixOneZone"));

			Thread.Sleep(sleepSeconds * 1000);

			//cleanup
			zlm.Dispose();
		}

		[TestCase(10, true, 0.5, 1, 1, 0.5)]
		[TestCase(10, false, 0.5, 1, 1, 0.5)]
		[TestCase(10, true, 1.0, 1, 2, 0.5)]
		[TestCase(10, true, 1.0, 1, 3, 0.5)]
		[TestCase(10, true, 1.0, 1, 4, 0.5)]
		[TestCase(10, true, 1.0, 50, 1, 0.5)]
		[TestCase(10, true, 1.0, 50, 2, 0.5)]
		[TestCase(10, true, 1.0, 50, 3, 0.5)]
		[TestCase(10, true, 1.0, 50, 4, 0.5)]
		[TestCase(10, true, 1.0, 127, 4, 0.5)]
		[TestCase(10, true, 1.0, 127, 3, 0.5)]
		[TestCase(10, true, 1.0, 127, 2, 0.5)]
		[TestCase(10, true, 1.0, 127, 1, 0.5)]
		[TestCase(10, true, 1.0, 127, 0, 0.5)]
		[TestCase(10, true, 1.0, 1, 0, 0.5)]
		public void Shimmer_ColorScheme_Works(int sleepSeconds, bool random, double density, int maxFadeSpeed, int maxFadeDelay, double brightness)
		{
			//act
			var zlm = new ZLM(false, false, false, zlmInner =>
			{
				var isv = new ISV();
				isv.Add("MaxFadeSpeed", maxFadeSpeed);
				isv.Add("MaxFadeDelay", maxFadeDelay);
				isv.Add("Density", density);
				isv.Add("Brightness", brightness);
				isv.Add("Random", random);
				isv.Add("ColorScheme", ColorScheme.Secondaries);
				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", 64);
				zlmInner.CreateSingularProgramSet("", new Shimmer(), isv, neomatrix);
			}, Config.Get("NeoMatrixOneZone"));

			Thread.Sleep(sleepSeconds * 1000);

			//cleanup
			zlm.Dispose();
		}

		//TODO: Add tests for MIDI stuff.
	}
}
