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
		[TestCase("FadeCandyZone", PixelType.FadeCandyWS2812Pixel, 8, (byte)1)]
	    public void AddFadeCandyZone_Works(string name, PixelType pixelType, int numberOfLights, byte channel)
		{
			var zlm = new ZLM(false, false, false);
			var zlmrpc = new ZLMRPC(zlm);

			var zone = zlmrpc.AddFadeCandyZone(name, pixelType, numberOfLights, channel);
			
			Assert.That(zone.Name, Is.EqualTo(name));
			Assert.That(zone.LightCount, Is.EqualTo(numberOfLights));
			Assert.That(((FadeCandyZone)zlm.Zones.First()).Channel, Is.EqualTo(channel));
			Assert.That(zone.Running, Is.EqualTo(false));
			Assert.That(zone.ZoneProgramName, Is.EqualTo(null));

			zlmrpc.Dispose();
		}

		[Test]
		[Category("Integration")]
		[TestCase("StepperSet", "Stepper", true)]
	    public void CreateProgramSet_Works(string programSetName, string programName, bool sync = true)
		{
			var zlm = new ZLM(false, false, false);
			var zlmrpc = new ZLMRPC(zlm);
			zlm.AddFadeCandyZone("FadeCandyZone", PixelType.FadeCandyWS2812Pixel, 8, 1);

			var zones = zlm.Zones.Select(z => z.Name).ToList();
			var programSet = zlmrpc.CreateProgramSet(programSetName, programName, zones, sync);
			
			Assert.That(programSet.Name, Is.EqualTo(programSetName));
			Assert.That(programSet.ProgramName, Is.EqualTo(programName));
			Assert.That(programSet.Sync, Is.EqualTo(true));
			Assert.That(programSet.Zones.Select(zone => zone.Name).ToList(), Is.EqualTo(zones));
			Assert.That(programSet.State, Is.EqualTo(ProgramState.Started));
			
			zlmrpc.Dispose();
		}

		[Test]
		[Category("Integration")]
		[TestCase("StepperSet", "Stepper", true)]
		public void GetStatus_Works(string programSetName, string programName, bool sync = true)
		{
			var zlm = new ZLM(false, false, false);
			var zlmrpc = new ZLMRPC(zlm);
			zlm.AddFadeCandyZone("FadeCandyZone", PixelType.FadeCandyWS2812Pixel, 8, 1);
			zlm.CreateProgramSet(programSetName, programName, zlm.Zones.Select(z => z.Name), sync);

			var status = zlmrpc.GetStatus();

			Assert.That(status, Is.EqualTo("--ZoneLighting Summary--\r\n=============================\r\n1 ProgramSet(s) currently running:\r\nStepperSet running Stepper on zone(s) FadeCandyZone in sync\r\n--End of Summary--\r\n"));

			zlmrpc.Dispose();
		}
		
		[TestCase("StepperSet", "Stepper", true)]
		public void DisposeProgramSet_Works(string programSetName, string programName, bool sync = true)
	    {
			var zlm = new ZLM(false, false, false);
			var zlmrpc = new ZLMRPC(zlm);
			zlm.AddFadeCandyZone("FadeCandyZone", PixelType.FadeCandyWS2812Pixel, 8, 1);
			zlm.CreateProgramSet(programSetName, programName, zlm.Zones.Select(z => z.Name), sync);
			zlm.Zones.ForEach(zone =>
			{
				Assert.That(zone.ZoneProgram, Is.Not.Null);
				Assert.That(zone.ZoneProgram.State, Is.EqualTo(ProgramState.Started));
			});

			zlmrpc.DisposeProgramSet(programSetName);

			zlm.Zones.ForEach(zone => Assert.That(zone.ZoneProgram, Is.Null));

			zlmrpc.Dispose();
		}
	}
}
