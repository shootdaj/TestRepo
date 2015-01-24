using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using ZoneLighting;

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
				ZoneLightingManager.Instance.Zones.First(z => z.Name == zone).ZoneProgram.Start();//(liveSync: true);
			if (command == "Stop")
				ZoneLightingManager.Instance.Zones.First(z => z.Name == zone).ZoneProgram.Stop(true);

			return View("Index", new ZLMViewModel());
		}

	    public ActionResult Notify(string colorString)
	    {
			var color = Color.FromName(colorString);
		    if (color.IsKnownColor)
		    {
			    dynamic parameters = new ExpandoObject();
			    parameters.Color = color;
			    parameters.Time = 500;
			    parameters.Soft = true;

			    ZoneLightingManager.Instance.Zones.ToList().ForEach(z => z.InterruptingPrograms[0].SetInput("Blink", parameters));
			    ZoneLightingManager.Instance.Zones.ToList().ForEach(z => z.InterruptingPrograms[0].SetInput("Blink", parameters));
		    }

		    return View("Index", new ZLMViewModel());
		}
    }
}