using System.ComponentModel.Composition;
using ZoneLighting;
using ZoneLighting.ZoneNS;

namespace ExternalZones
{
	[Export(typeof(Zone))]
	public class RightWing : FadeCandyZone
	{
		public RightWing() : base("RightWing")
		{
			var numLights = 12;
			byte fcChannel = 2;

			for (int i = 0; i < numLights; i++)
			{
				AddLight(new LED(logicalIndex: i, fadeCandyChannel: fcChannel, fadeCandyIndex: i));
			}
		}
	}
}
