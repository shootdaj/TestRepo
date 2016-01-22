using System.Web.Mvc;
using WebController.ContainerNS;
using WebController.Models;
using ZoneLighting;

namespace WebController.Controllers
{
	public class ZLMController : Controller
	{
		public ZLMController()
		{
			ZLMRPC = Container.ZLMRPC;
		}

		public IZLMRPC ZLMRPC { get; set; }

		public static IZLM ZLM => Container.ZLM;

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

		[HttpPost]
		public ActionResult CreateZLM()
		{
			Controllers.ZLMRPC.CreateZLM();
			return ReturnZLMView("Index");
		}

		[HttpPost]
		public ActionResult DisposeZLM()
		{
			ZLMRPC.DisposeZLM();
			return ReturnZLMView("Index");
		}

		private ActionResult ReturnZLMView(string viewName = null)
		{
			return viewName == null ? View(new ZLMViewModel()) : View(viewName, new ZLMViewModel());
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
			ZLMRPC.ProcessZLMCommand(command, programSetName, programName);
			return ReturnZLMPartialView("ProgramSet");
		}

		private ActionResult ReturnZLMPartialView(string partialViewName)
		{
			return PartialView(partialViewName, new ZLMViewModel());
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

		#region Internals

		#endregion
	}
}