using System.Web.Mvc;
using ZoneLighting;
using ZoneLighting.ZoneNS;

namespace WebController.Controllers
{
	public class ZLMViewModel
	{
		//public ZoneLightingManager ZLM => ZoneLightingManager.Instance;

		public ZLMViewModel()
		{
			if (ZLM != null)
			ZLMController.ZLMAction(zlm => AvailableZones = new SelectList(zlm.Zones, "Name"));
		}

		public ZLM ZLM => (ZLM)System.Web.HttpContext.Current.Application["ZLM"];

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