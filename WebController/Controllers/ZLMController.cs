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
	    public ActionResult UninitializeZLM()
	    {
		    ZoneLightingManager.Instance.Uninitialize();
			return View("Index", new ZLMViewModel());
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
			var zoneString = split[1];

			if (command == "Start")
			{
				if (zoneString == "All")
					ZoneLightingManager.Instance.Zones.ToList().ForEach(zone => zone.ZoneProgram.Start(sync: true));
				else
					ZoneLightingManager.Instance.Zones.First(z => z.Name == zoneString).ZoneProgram.Start(sync: true);
			}
			else if (command == "Stop")
			{
				if (zoneString == "All")
					ZoneLightingManager.Instance.Zones.ToList().ForEach(zone => zone.ZoneProgram.Stop(true));
				else
					ZoneLightingManager.Instance.Zones.First(z => z.Name == zoneString).ZoneProgram.Stop(true);
			}
			
			return View("Index", new ZLMViewModel());
		}

	    public ActionResult Notify(string colorString, int? time = 60, int? cycles = 2)
	    {
			var color = Color.FromName(colorString);
		    if (color.IsKnownColor)
		    {
			    dynamic parameters = new ExpandoObject();
			    parameters.Color = color;
			    parameters.Time = time;
			    parameters.Soft = true;

			    for (int i = 0; i < cycles; i++)
			    {
					ZoneLightingManager.Instance.Zones.ToList().ForEach(z => z.InterruptingPrograms[0].SetInput("Blink", parameters));
				}
		    }

		    return View("Index", new ZLMViewModel());
		}
    }
}