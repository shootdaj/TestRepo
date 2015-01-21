using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZoneLighting;

namespace WebController.Controllers
{
    public class ZonesController : Controller
    {
		[HttpPost]
		public ActionResult Zones()
		{
			var zones = ZoneLightingManager.Instance.Zones;
			return Json(new SelectList(zones, "Name", "Name"));
		}
    }
}