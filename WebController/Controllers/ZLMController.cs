using System.Web.Mvc;
using WebController.IoC;
using WebController.Models;
using ZoneLighting;
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

		public static IZLM ZLM => Container.ZLM;

		private ActionResult ReturnZLMView(string viewName = null)
		{
			return viewName == null ? View(new ZLMViewModel()) : View(viewName, new ZLMViewModel());
		}

		private ActionResult ReturnZLMPartialView(string partialViewName)
		{
			return PartialView(partialViewName, new ZLMViewModel());
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
			Controllers.ZLMRPC.ZLMAction(zlm => zlm.DisposeProgramSets());
			//MvcApplication.ZLM.Uninitialize();
			return ReturnZLMView("Index");
		}

		public ActionResult CreateZLM()
		{
			ZLMRPC.CreateZLMInstance();
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
			ZLMRPC.StopZone(zoneName);
			return ReturnZLMView("Index");
		}

		[HttpPost]
		// ReSharper disable once InconsistentNaming
		public ActionResult ZLMCommand(string Command, string programSetName, string programName)
		{
			var command = Command;

			var isv = new ISV();
			if (command.ToLower() == "start" && programName == "Shimmer")
			{
				
				isv.Add("MaxFadeSpeed", 1);
				isv.Add("MaxFadeDelay", 20);
				isv.Add("Density", 1.0);
				isv.Add("Brightness", 0.3);
				isv.Add("Random", true);
			}

			ZLMRPC.ProcessZLMCommand(command, programSetName, programName, isv);
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