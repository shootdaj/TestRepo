using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZoneLighting;
using ZoneLighting.ZoneProgram;

namespace WebController.Controllers
{
    public class ZLMController : Controller
    {
        public ActionResult Index()
        {
			return View(new ZLMViewModel());
        }
		
		public string InitializeZLM()
	    {
		    ZoneLightingManager.Instance.Initialize();
		    return "ZLM Initialized";
	    }

	    public string UninitializeZLM()
	    {
		    ZoneLightingManager.Instance.Uninitialize();
			return "ZLM Uninitialized";
	    }
    }

	public class ZLMViewModel
	{
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