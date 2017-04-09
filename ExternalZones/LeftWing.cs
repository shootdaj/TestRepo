using System.ComponentModel.Composition;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;

namespace ExternalZones
{
	[Export(typeof(Zone))]
	public class LeftWing : FadeCandyZone
	{
		public LeftWing() : base("LeftWing")
		{
			AddFadeCandyLights(PixelType.OPCRGBPixel, 6, 1);
		}
	}
}
