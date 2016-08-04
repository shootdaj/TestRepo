using System;
using System.Dynamic;
using System.Threading;
using NUnit.Framework;
using Refigure;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests.ProgramTests
{
	[Ignore("Manual Test")]
	public class MidiTwoDimensionalFadeTests
    {
        [TestCase(60, true)]
        [Ignore("Manual")]
        public void MidiTwoDimensionalFade_Works(int sleepSeconds, bool random)
        {
            ZLM zlm = null;
            var deviceID = 0;

            try
            {
                //act
                zlm = new ZLM(false, false, false, zlmInner =>
                {
                    var neomatrix = ZoneScaffolder.Instance.AddFadeCandyZone(zlmInner.Zones, "NeoMatrix",
                        PixelType.FadeCandyWS2812Pixel, 64, 1);
                    var midiTwoDimensionalFade = new MidiTwoDimensionalFade();
                    dynamic startingParameters = new ExpandoObject();
                    startingParameters.DeviceID = deviceID;
                    neomatrix.Run(midiTwoDimensionalFade, startingParameters: startingParameters);

                }, Config.Get("NeoMatrixOneZone"));

                Thread.Sleep(sleepSeconds*1000);

            }
            catch (Exception ex)
            {
                zlm?.Dispose();
                Thread.Sleep(10000);
            }
        }
    }
}
