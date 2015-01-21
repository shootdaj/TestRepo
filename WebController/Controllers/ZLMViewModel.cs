using System.Web.Mvc;
using ZoneLighting;
using ZoneLighting.ZoneNS;

namespace WebController.Controllers
{
	public class ZLMViewModel
	{
		public ZoneLightingManager ZLM => ZoneLightingManager.Instance;

		public ZLMViewModel()
		{
			AvailableZones = new SelectList(ZoneLightingManager.Zones, "Name");
		}

		public ZoneLightingManager ZoneLightingManager
		{
			get { return ZoneLightingManager.Instance; }
		}

		public Zone Zone { get; set; }
		//public IZoneProgram ZoneProgram { get; set; }

		public SelectList AvailableZones { get; set; }
		//public SelectList AvailableZonePrograms { get; set; }

		//public IEnumerable<Zone> Zones
		//{
		//	get { return ZoneLightingManager.Zones; }
		//}

		//public IEnumerable<string> ZonePrograms
		//{
		//	get { return ZoneLightingManager.AvailableProgramNames; }
		//}
	}
}