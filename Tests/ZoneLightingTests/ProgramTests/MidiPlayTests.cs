using System.Dynamic;
using System.Threading;
using NUnit.Framework;
using Refigure;
using ZoneLighting;
using ZoneLighting.StockPrograms;
using ZoneLighting.Usables;

namespace ZoneLightingTests.ProgramTests
{
	[Ignore("Manual Test")]
	public class MidiPlayTests
	{
		[TestCase(30)]
		[TestCase(30)]
		public void MidiPlay_Works(int sleepSeconds)
		{
			//act
			var zlm = new ZLM(false, false, false, zlmInner =>
			{
				var livingRoom = RunnerHelpers.CreateLivingRoomZone(zlmInner);
				dynamic startingParameters = new ExpandoObject();
				startingParameters.DeviceID = int.Parse(Config.Get("MIDIDeviceID", "Please set the value of MIDIDeviceID in configuration."));
				zlmInner.CreateSingularProgramSet("MidiPlaySet", new LivingRoomMidiPlay(), null, livingRoom, startingParameters);
				
			}, Config.Get("LivingRoomZone"));

			Thread.Sleep(sleepSeconds * 1000);

			//cleanup
			zlm.Dispose();
		}

		//[TestCase(10, true, 0.5, 1, 1, 0.5)]
		//[TestCase(10, false, 0.5, 1, 1, 0.5)]
		//[TestCase(10, true, 1.0, 1, 2, 0.5)]
		//[TestCase(10, true, 1.0, 1, 3, 0.5)]
		//[TestCase(10, true, 1.0, 1, 4, 0.5)]
		//[TestCase(10, true, 1.0, 50, 1, 0.5)]
		//[TestCase(10, true, 1.0, 50, 2, 0.5)]
		//[TestCase(10, true, 1.0, 50, 3, 0.5)]
		//[TestCase(10, true, 1.0, 50, 4, 0.5)]
		//[TestCase(10, true, 1.0, 127, 4, 0.5)]
		//[TestCase(10, true, 1.0, 127, 3, 0.5)]
		//[TestCase(10, true, 1.0, 127, 2, 0.5)]
		//[TestCase(10, true, 1.0, 127, 1, 0.5)]
		//[TestCase(10, true, 1.0, 127, 0, 0.5)]
		//[TestCase(10, true, 1.0, 1, 0, 0.5)]
		//[Ignore("Manual")]
		//public void Shimmer_ColorScheme_Works(int sleepSeconds, bool random, double density, int maxFadeSpeed, int maxFadeDelay, double brightness)
		//{
		//	//act
		//	var zlm = new ZLM(false, false, false, zlmInner =>
		//	{
		//		var isv = new ISV();
		//		isv.Add("MaxFadeSpeed", maxFadeSpeed);
		//		isv.Add("MaxFadeDelay", maxFadeDelay);
		//		isv.Add("Density", density);
		//		isv.Add("Brightness", brightness);
		//		isv.Add("Random", random);
		//		isv.Add("ColorScheme", ColorScheme.Secondaries);
		//		var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", PixelType.FadeCandyWS2812Pixel, 64, 1);
		//		zlmInner.CreateSingularProgramSet("", new Shimmer(), isv, neomatrix);
		//	}, Config.Get("NeoMatrixOneZone"));

		//	Thread.Sleep(sleepSeconds * 1000);

		//	//cleanup
		//	zlm.Dispose();
		//}
	}
}
