using System.Web.Mvc;
using ZoneLighting;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;

namespace WebController.Controllers
{
    public class ZonesController : Controller
    {
		[HttpPost]
		public ActionResult Zones()
		{
			BetterList<Zone> zones = new BetterList<Zone>();
			ZLMController.ZLMAction(zlm => zones = zlm.Zones);
			return Json(new SelectList(zones, "Name", "Name"));
		}
    }
}