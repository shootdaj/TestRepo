using System;
using System.Web.Mvc;
using WebController.IoC;
using WebController.Models;
using ZoneLighting.ZoneProgramNS;

namespace WebController.Controllers
{
	public class ZLMController : Controller
	{
		#region Internals

		public ZLMController()
		{
			ZLMRPC = Container.ZLMRPC;
		}

		public IZLMRPC ZLMRPC { get; set; }

		private ActionResult ReturnZLMView(string viewName = null)
		{
			return viewName == null ? View(new ZLMViewModel(ZLMRPC)) : View(viewName, new ZLMViewModel(ZLMRPC));
		}

		private ActionResult ReturnZLMPartialView(string partialViewName)
		{
			return PartialView(partialViewName, new ZLMViewModel(ZLMRPC));
		}

		#endregion

		#region Actions

		public ActionResult Index()
		{
			return ReturnZLMView();
		}

		[HttpPost]
		public ActionResult DisposeProgramSets()
		{
			ZLMRPC.DisposeProgramSets();
			//MvcApplication.ZLM.Uninitialize();
			return ReturnZLMView("Index");
		}

		public ActionResult CreateZLM()
		{
			ZLMRPC.CreateZLM();
			return ReturnZLMView("Index");
		}

		public ActionResult DisposeZLM()
		{
			ZLMRPC.DisposeZLM();
			return ReturnZLMView("Index");
		}

		
		[HttpPost]
		public ActionResult Save()
		{
			ZLMRPC.Save();
			return ReturnZLMView("Index");
		}

		[HttpPost]
		public ActionResult StopZone(string zoneName)
		{
			ZLMRPC.StopZone(zoneName, true);
			return ReturnZLMView("Index");
		}

		[HttpPost]
		// ReSharper disable once InconsistentNaming
		public ActionResult ZLMCommand(string Command, string programSetName, string programName)
		{
			var command = Command;

			dynamic isv = new ISV();
			if (command.ToLower() == "start" && programName == "Shimmer")
			{
				isv.Add("MaxFadeSpeed", 1);
				isv.Add("MaxFadeDelay", 20);
				isv.Add("Density", 1.0);
				isv.Add("Brightness", 0.3);
				isv.Add("Random", true);
			}

			if (command.ToUpperInvariant() == "START")
			{
				//TODO: Uncomment and fix
				//ZLMRPC.RestartProgramSet(programSetName, programName, new List<string>() { "all"} , isv);
			}
			else if (command.ToUpperInvariant() == "STOP")
			{
				ZLMRPC.StopProgramSet(programSetName);
			}
			else
			{
				throw new ArgumentException("No.");
			}
			
			return ReturnZLMPartialView("ProgramSet");
		}
		
		[HttpPost]
		public ActionResult SetZoneColor(string zoneName, string color, float brightness)
		{
			ZLMRPC.SetZoneColor(zoneName, color, brightness);
			return ReturnZLMView("Index");
		}

		[HttpPost]
		public ActionResult Notify(string colorString, int? time = 60, int? cycles = 2)
		{
			ZLMRPC.Notify(colorString, time, cycles);
			return ReturnZLMView("Index");
		}

		#endregion
	}
}