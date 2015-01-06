using System.ComponentModel.Composition;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;

namespace ExternalZones
{
	[Export(typeof(Zone))]
	public class LeftWing : FadeCandyZone
	{
		public LeftWing() : base("LeftWing")
		{
			AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 6, 1);
		}
	}
}
