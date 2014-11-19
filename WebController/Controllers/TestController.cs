using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZoneLighting;

namespace WebController.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
		[HttpGet]
        public ActionResult Index()
        {
            return View();
        }
		
		public string InitializeZLM()
	    {
		    ZoneLightingManager.Instance.Initialize();
		    return "ZLM Initialized";
	    }

	    public string UninitializeZLM()
	    {
		    ZoneLightingManager.Instance.Uninitialize();
			return "ZLM Uninitialized";
	    }
    }
}