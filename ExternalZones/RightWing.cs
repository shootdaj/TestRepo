using System.ComponentModel.Composition;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;

namespace ExternalZones
{
	[Export(typeof(Zone))]
	public class RightWing : FadeCandyZone
	{
		public RightWing() : base("RightWing")
		{
			AddFadeCandyLights(PixelType.OPCRGBPixel, 12, 2);
		}
	}
}
