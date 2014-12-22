using System.ComponentModel.Composition;
using ZoneLighting;
using ZoneLighting.ZoneNS;

namespace ExternalZones
{
	[Export(typeof(Zone))]
	public class LeftWing : FadeCandyZone
	{
		public LeftWing() : base("LeftWing")
		{
			AddFadeCandyLights(6, 1);
		}
	}
}
