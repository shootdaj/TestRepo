using System;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Threading;
using NUnit.Framework;
using Sanford.Multimedia.Midi;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.ConfigNS;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests.ProgramTests
{
	public class MidiBlinkTests
	{
		

		[TestCase(60, true)]
		[Ignore("Manual")]
		public void MidiBlink_Works(int sleepSeconds, bool random)
		{
			ZLM zlm = null;

			try
			{
				//act
				zlm = new ZLM(false, false, false, zlmInner =>
				{
					var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix",
						PixelType.FadeCandyWS2812Pixel, 64, 1);
					var midiBlink = new MidiBlink();
					neomatrix.Run(midiBlink, null);					

				}, Config.Get("NeoMatrixOneZone"));

				Thread.Sleep(sleepSeconds*1000);

			}
			catch (Exception ex)
			{
				zlm.Dispose();
				Thread.Sleep(10000);
			}
		}

		[Test]
		public void etstet()
		{
			var midiInput = new InputDevice(0);

			midiInput.ChannelMessageReceived += (sender, args) =>
			{
				Debug.Print("nice");
			};

			midiInput.SysCommonMessageReceived += (sender, args) => { };
			midiInput.SysExMessageReceived += (sender, args) => { };
			midiInput.SysRealtimeMessageReceived += (sender, args) => { };
			midiInput.Error += (sender, args) => { };

			midiInput.StartRecording();
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
		[TestCase(10, true, 1.0, 0, 0, 0.5)]
		[Ignore("Manual")]
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
				var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix", PixelType.FadeCandyWS2812Pixel, 64, 1);
				zlmInner.CreateSingularProgramSet("", new Shimmer(), isv, neomatrix);
			}, Config.Get("NeoMatrixOneZone"));

			Thread.Sleep(sleepSeconds * 1000);

			//cleanup
			zlm.Dispose();
		}
	}
}
