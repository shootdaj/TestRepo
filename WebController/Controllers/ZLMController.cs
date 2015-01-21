using System.Linq;
using System.Web.Mvc;
using WebController.Extensions;
using ZoneLighting;
using ZoneLighting.ZoneNS;

namespace WebController.Controllers
{
    public class ZLMController : Controller
    {
        public ActionResult Index()
        {
			return View(new ZLMViewModel());
        }

		[HttpPost]
	    public string UninitializeZLM()
	    {
		    ZoneLightingManager.Instance.Uninitialize();
			return "ZLM Uninitialized";
	    }

		[HttpPost]
		public ActionResult InitializeZLM()
		{
			ZoneLightingManager.Instance.Initialize(loadExternalZones: false);
			return View("Index", new ZLMViewModel());
		}

		[HttpPost]
	    public ActionResult StopZone(string zoneName)
	    {
		    ZoneLightingManager.Instance.Zones.First(z => z.Name == zoneName).Uninitialize(true);
			return View("Index", new ZLMViewModel());
	    }

		[HttpPost]
	    public ActionResult ZoneCommand(string Command)
		{
			var split = Command.Split(' ');
			var command = split[0];
			var zone = split[1];

			if (command == "Start")
				ZoneLightingManager.Instance.Zones.First(z => z.Name == zone).Resume();
			if (command == "Stop")
				ZoneLightingManager.Instance.Zones.First(z => z.Name == zone).Pause();

			return View("Index", new ZLMViewModel());
		}
    }
}