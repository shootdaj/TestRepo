using System;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using ZoneLighting;
using ZoneLighting.Usables;

namespace WebController.Controllers
{
	public class ZLMController : Controller
	{
		public static void ZLMAction(Action<ZLM> action)
		{
			System.Web.HttpContext.Current.Application.Lock();
			action.Invoke((ZLM)System.Web.HttpContext.Current.Application["ZLM"]);
			System.Web.HttpContext.Current.Application.UnLock();
		}

		public ActionResult Index()
		{
			return View(new ZLMViewModel());
		}

		[HttpPost]
		public ActionResult UninitializeZLM()
		{
			ZLMAction(zlm => zlm.Uninitialize());
			//MvcApplication.ZLM.Uninitialize();
			return View("Index", new ZLMViewModel());
		}

		[HttpPost]
		public ActionResult InitializeZLM()
		{
			ZLMAction(zlm =>
			{
				zlm.Initialize();
			});
			
			//ZLM.I.Initialize(false, false, false, initAction:RunnerHelpers.AddNeopixelZonesAndProgramsWithSync());
			return View("Index", new ZLMViewModel());
		}

		public ActionResult Save()
		{
			ZLMAction(zlm =>
			{
				zlm.SaveZones();
				zlm.SaveProgramSets();
			});

			return View("Index", new ZLMViewModel());
		}

		[HttpPost]
		public ActionResult StopZone(string zoneName)
		{
			ZLMAction(zlm =>
			{
				zlm.Zones.First(z => z.Name == zoneName).Stop(true);
			});

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
					ZLMAction(zlm =>
					{ zlm.ProgramSets["RainbowSet"].StartAllPrograms(); });
				//ZoneLightingManager.Instance.Zones.ToList().ForEach(zone => zone.ZoneProgram.Start(sync: true));
				else
					ZLMAction(zlm =>
					{
						zlm.Zones.First(z => z.Name == zoneString).ZoneProgram.Start(sync: true);
					});
			}
			else if (command == "Stop")
			{
				if (zoneString == "All")
					ZLMAction(zlm =>
					{ zlm.ProgramSets["RainbowSet"].StopAllPrograms(); });
				else
					ZLMAction(zlm =>
					{
						zlm.Zones.First(z => z.Name == zoneString).ZoneProgram.Stop(true);
					});

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
					ZLMAction(zlm =>
					{
						zlm.Zones.ToList().ForEach(z => z.InterruptingPrograms[0].SetInput("Blink", parameters));
					});
				}
			}

			return View("Index", new ZLMViewModel());
		}
	}
}