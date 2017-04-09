using System.ComponentModel.Composition;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;

namespace ExternalZones
{
	[Export(typeof(Zone))]
	public class LeftWing : OPCZone
	{
		public LeftWing() : base(FadeCandyController.Instance, "LeftWing")
		{
			AddOPCLights(OPCPixelType.OPCRGBPixel, 6, 1);
		}
	}
}
