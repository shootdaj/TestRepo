using System.Linq;
using NUnit.Framework;
using WebController.Controllers;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace WebControllerTests
{
    public class ZLMRPCTests
    {
		[Test]
		[Category("Integration")]
	    public void CreateZLM_Works()
	    {
			
	    }

		[TestCase("FadeCandyZone", PixelType.FadeCandyWS2812Pixel, 8, (byte)1)]
	    public void AddFadeCandyZone_Works(string name, PixelType pixelType, int numberOfLights, byte channel)
		{
			var zlm = new ZLM(false, false, false);
			var zlmrpc = new ZLMRPC(zlm);

			var zone = zlmrpc.AddFadeCandyZone(name, pixelType, numberOfLights, channel);

			Assert.That(zlm.Zones.First(), Is.EqualTo(zone));
			Assert.That(zlm.Zones.First().Name, Is.EqualTo(name));
			Assert.That(zlm.Zones.First().LightingController, Is.EqualTo(FadeCandyController.Instance));
			Assert.That(zlm.Zones.First().LightCount, Is.EqualTo(numberOfLights));
			Assert.That(((FadeCandyZone)zlm.Zones.First()).Channel, Is.EqualTo(channel));

			zlm.Dispose();
		}

		[Test]
		[Category("Integration")]
		[TestCase("StepperSet", "Stepper", true)]
	    public void CreateProgramSet_Works(string programSetName, string programName, bool sync = true)
		{
			var zlm = new ZLM(false, false, false);
			var zlmrpc = new ZLMRPC(zlm);
			zlm.AddFadeCandyZone("FadeCandyZone", PixelType.FadeCandyWS2812Pixel, 8, 1);

			zlmrpc.CreateProgramSet(programSetName, programName, zlm.Zones.Select(z => z.Name), sync);
			
			Assert.That(zlm.ProgramSets.First().Name, Is.EqualTo(programSetName));
			Assert.That(zlm.ProgramSets.First().ProgramName, Is.EqualTo(programName));
			Assert.That(zlm.ProgramSets.First().Sync, Is.EqualTo(true));
			Assert.That(zlm.ProgramSets.First().Zones, Is.EqualTo(zlm.Zones));
			Assert.That(zlm.ProgramSets.First().Zones.First().ZoneProgram.State, Is.EqualTo(ProgramState.Started));
			
			zlm.Dispose();
		}

		[Test]
		[Category("Integration")]
		[TestCase("StepperSet", "Stepper", true)]
		public void GetStatus_Works(string programSetName, string programName, bool sync = true)
		{
			//failure
			var zlm = new ZLM(false, false, false);
			var zlmrpc = new ZLMRPC(zlm);
			zlm.AddFadeCandyZone("FadeCandyZone", PixelType.FadeCandyWS2812Pixel, 8, 1);
			zlm.CreateProgramSet(programSetName, programName, zlm.Zones.Select(z => z.Name), sync);

			Assert.That(zlmrpc.GetStatus(), Is.EqualTo(""));

			zlm.Dispose();
		}
	}
}
