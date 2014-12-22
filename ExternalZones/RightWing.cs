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
			AddFadeCandyLights(12, 2);
		}
	}
}
