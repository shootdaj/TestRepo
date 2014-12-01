using System.ComponentModel.Composition;
using ZoneLighting;
using ZoneLighting.ZoneNS;

namespace ExternalZones
{
	[Export(typeof(Zone))]
	[Export("BasementLighting")]
	public class LeftWing : FadeCandyZone
	{
		public LeftWing() : base("LeftWing")
		{
			var numLights = 6;
			byte fcChannel = 1;

			for (int i = 0; i < numLights; i++)
			{
				AddLight(new LED(logicalIndex: i, fadeCandyChannel: fcChannel, fadeCandyIndex: i));
			}
		}
	}
}
