using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebController.ViewModels;
using ZoneLighting;

namespace WebController.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Index(ZoneLightingCommand command)
		{
			if (command.CommandName == "Start")
				ZoneLightingManager.Instance.Initialize();
			else if (command.CommandName == "Stop")
				ZoneLightingManager.Instance.Uninitialize();
			
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your app description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}
