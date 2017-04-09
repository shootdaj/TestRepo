using System.ComponentModel.Composition;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;

namespace ExternalZones
{
	[Export(typeof(Zone))]
	public class RightWing : OPCZone
	{
		public RightWing() : base(FadeCandyController.Instance, "RightWing")
		{
			AddOPCLights(OPCPixelType.OPCRGBPixel, 12, 2);
		}
	}
}
