using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Refigure;
using WebRemote.Models;
using ZoneLighting;
using ZoneLighting.Usables;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace WebController.Controllers
{
	public class ZLMController : Controller
	{
	    public ZLMController(ZLM zlm)
	    {
	        ZLM = zlm;
	    }

	    public static ZLM ZLM;

		public static void ZLMAction(Action<ZLM> action)
		{
            //do locks need to be added back here?
			action.Invoke(ZLM);
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
            
			ZLM = new ZLM(loadZonesFromConfig: !firstRun,
				loadProgramSetsFromConfig: !firstRun,
				loadZoneModules: loadZoneModules, initAction: initAction);

			return View("Index", new ZLMViewModel());
		}

		public ActionResult DisposeZLM()
		{
            ZLM.Dispose();
			
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
		public ActionResult ZLMCommand(string Command, string programSetName, string programName)
		{
			var command = Command;
			ProcessZLMCommand(command, programSetName, programName);
			return PartialView("ProgramSet", new ZLMViewModel().ZLM);
		}

		private void ProcessZLMCommand(string command, string programSetName, string programName)
		{
			if (command.ToLower().Trim() == "start")
			{
				ZLMAction(zlm =>
				{
					zlm.DisposeProgramSets(programSetName);
					
					var isv = new ISV();
					isv.Add("MaxFadeSpeed", 1);
					isv.Add("MaxFadeDelay", 20);
					isv.Add("Density", 1.0);
					isv.Add("Brightness", 1.0);
					isv.Add("Random", true);

					zlm.CreateProgramSet(programSetName, programName, false, isv, zlm.Zones);
				});
			}
			else if (command.ToLower().Trim() == "stop")
			{
				ZLMAction(zlm => zlm.ProgramSets.First(z => z.Name == programSetName).Stop());
			}
		}
		

		public ActionResult SetZoneColor(string zoneName, string color, float brightness)
		{
			ZLMAction(zlm =>
			{
				zlm.Zones[zoneName].SetColor(Color.FromName(color).Darken(brightness));
				zlm.Zones[zoneName].SendLights(zlm.Zones[zoneName].LightingController);
			});

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



		public ActionResult ProgramSetDetails(string name)
		{
			return View(ZLM.ProgramSets[name]);
		}
	}
}