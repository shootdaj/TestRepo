using System.Collections.Generic;
using System.Threading;
using ZoneLighting.Communication;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.ZoneNS
{
	public class FadeCandyZone : Zone
	{
		public FadeCandyZone(string name = "", ZoneProgram program = null, InputStartingValues inputStartingValues = null, Barrier barrier = null, double? brightness = null) : base(FadeCandyController.Instance, name, program, inputStartingValues, barrier, brightness)
		{
		}

		public void AddFadeCandyLights(PixelType pixelType, int numLights, byte fcChannel)
		{
			for (int i = 0; i < numLights; i++)
			{
				AddLight(new LED(logicalIndex: i, fadeCandyChannel: fcChannel, fadeCandyIndex: i, pixelType: pixelType));
			}
		}
	}
}
