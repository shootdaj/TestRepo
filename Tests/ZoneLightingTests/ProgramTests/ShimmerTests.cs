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
	public class ShimmerTests
	{
		[TestCase(30, true)]
		[TestCase(30, false)]
		[Ignore("Manual")]
		public void Shimmer_Works(int sleepSeconds, bool random)
		{
			//act
			var zlm = new ZLM(false, false, false, zlmInner =>
			{
				var isv = new ISV();
				isv.Add("MaxFadeSpeed", 1);
				isv.Add("MaxFadeDelay", 2);
				isv.Add("Density", 32);
				isv.Add("Brightness", 0.5);
				isv.Add("Random", random);
				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", PixelType.FadeCandyWS2812Pixel, 64, 1);
				zlmInner.CreateSingularProgramSet("", new Shimmer(), isv, neomatrix);
			}, Config.Get("NeoMatrixOneZone"));

			Thread.Sleep(sleepSeconds * 1000);

			//cleanup
			zlm.Dispose();
		}

		[TestCase(10, true, 32, 1, 1, 0.5)]
		[TestCase(10, false, 32, 1, 1, 0.5)]
		[TestCase(10, true, 64, 1, 2, 0.5)]
		[TestCase(10, true, 64, 1, 3, 0.5)]
		[TestCase(10, true, 64, 1, 4, 0.5)]
		[TestCase(10, true, 64, 50, 1, 0.5)]
		[TestCase(10, true, 64, 50, 2, 0.5)]
		[TestCase(10, true, 64, 50, 3, 0.5)]
		[TestCase(10, true, 64, 50, 4, 0.5)]
		[TestCase(10, true, 64, 127, 4, 0.5)]
		[TestCase(10, true, 64, 127, 3, 0.5)]
		[TestCase(10, true, 64, 127, 2, 0.5)]
		[TestCase(10, true, 64, 127, 1, 0.5)]
		[TestCase(10, true, 64, 127, 0, 0.5)]
		[TestCase(10, true, 64, 0, 0, 0.5)]
		[Ignore("Manual")]
		public void Shimmer_ColorScheme_Works(int sleepSeconds, bool random, int density, int maxFadeSpeed, int maxFadeDelay, double brightness)
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
				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", PixelType.FadeCandyWS2812Pixel, 64, 1);
				zlmInner.CreateSingularProgramSet("", new Shimmer(), isv, neomatrix);
			}, Config.Get("NeoMatrixOneZone"));

			Thread.Sleep(sleepSeconds * 1000);

			//cleanup
			zlm.Dispose();
		}
	}
}
