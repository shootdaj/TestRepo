using System.ComponentModel.Composition;
using ZoneLighting;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;

namespace ExternalZones
{
	[Export(typeof(Zone))]
	public class RightWing : FadeCandyZone
	{
		public RightWing() : base("RightWing")
		{
			AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 12, 2);
		}
	}
}
