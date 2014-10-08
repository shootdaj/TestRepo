using ZoneLighting.Communication;

namespace ZoneLighting.MappingNS
{
	class Mapping
	{
		public Mapping(int zoneIndex, int lcIndex, ILightingController lc)
		{
			ZoneIndex = zoneIndex;
			LightingControllerIndex = lcIndex;
			LightingController = lc;
		}

		public int ZoneIndex { get; set; }
		public int LightingControllerIndex { get; set; }
		public ILightingController LightingController { get; set; }
	}
}
