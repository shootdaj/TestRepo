using System;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using ZoneLighting;
using ZoneLighting.ConfigNS;
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
		public ActionResult DisposeProgramSets()
		{
			ZLMAction(zlm => zlm.DisposeProgramSets());
			//MvcApplication.ZLM.Uninitialize();
			return View("Index", new ZLMViewModel());
		}

		//[HttpPost]
		public ActionResult CreateZLM()
		{
			bool firstRun;
			if (!bool.TryParse(Config.Get("FirstRun"), out firstRun))
				firstRun = true;

			bool loadZoneModules;
			if (!bool.TryParse(Config.Get("LoadZoneModules"), out loadZoneModules))
				loadZoneModules = false;

			Action<ZLM> initAction = null;
			if (typeof(RunnerHelpers).GetMethods().Select(method => method.Name).Contains(Config.Get("InitAction")))
			{
				var initActionInfo = typeof(RunnerHelpers).GetMethods().First(method => method.Name == Config.Get("InitAction"));
				initAction = (Action<ZLM>)Delegate.CreateDelegate(typeof(Action<ZLM>), initActionInfo);
			}

			System.Web.HttpContext.Current.Application.Lock();

			System.Web.HttpContext.Current.Application["ZLM"] = new ZLM(loadZonesFromConfig: !firstRun,
				loadProgramSetsFromConfig: !firstRun,
				loadZoneModules: loadZoneModules, initAction: initAction);

			System.Web.HttpContext.Current.Application.UnLock();
			
			return View("Index", new ZLMViewModel());
		}

		public ActionResult DisposeZLM()
		{
			System.Web.HttpContext.Current.Application.Lock();
			((ZLM)System.Web.HttpContext.Current.Application["ZLM"]).Dispose();
			System.Web.HttpContext.Current.Application.UnLock();

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

		//[HttpPost]
		public ActionResult StopZone(string zoneName)
		{
			ZLMAction(zlm =>
			{
				zlm.Zones.First(z => z.Name == zoneName).Stop(true);
			});

			return View("Index", new ZLMViewModel());
		}


		//[HttpPost]
		public ActionResult ZLMCommand(string Command)
		{
			var split = Command.Split(' ');
			var command = split[0];
			var zoneString = split[1];

			if (command == "Start")
			{
				if (zoneString == "All")
					ZLMAction(zlm =>
					{
						Parallel.ForEach(zlm.ProgramSets, programSet =>
						{
							programSet.Start();
						});
					});
				else
					ZLMAction(zlm =>
					{
						zlm.ProgramSets.First(z => z.Name == zoneString).Start();
					});
			}
			else if (command == "Stop")
			{
				if (zoneString == "All")
					ZLMAction(zlm =>
					{
						Parallel.ForEach(zlm.ProgramSets, programSet =>
						{
							programSet.Stop();
						});
					});
				else
					ZLMAction(zlm =>
					{
						zlm.ProgramSets.First(z => z.Name == zoneString).Stop();
					});
			}

			return View("Index", new ZLMViewModel());
		}

		//[HttpPost]
		//public ActionResult ZoneCommand(string Command)
		//{
		//	var split = Command.Split(' ');
		//	var command = split[0];
		//	var zoneString = split[1];

		//	if (command == "Start")
		//	{
		//		if (zoneString == "All")
		//			ZLMAction(zlm =>
		//			{ zlm.ProgramSets["RainbowSet"].StartAllPrograms(); });
		//		//ZoneLightingManager.Instance.Zones.ToList().ForEach(zone => zone.ZoneProgram.Start(sync: true));
		//		else
		//			ZLMAction(zlm =>
		//			{
		//				zlm.Zones.First(z => z.Name == zoneString).ZoneProgram.Start(sync: true);
		//			});
		//	}
		//	else if (command == "Stop")
		//	{
		//		if (zoneString == "All")
		//			ZLMAction(zlm =>
		//			{ zlm.ProgramSets["RainbowSet"].StopAllPrograms(); });
		//		else
		//			ZLMAction(zlm =>
		//			{
		//				zlm.Zones.First(z => z.Name == zoneString).ZoneProgram.Stop(true);
		//			});

		//	}

		//	return View("Index", new ZLMViewModel());
		//}

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