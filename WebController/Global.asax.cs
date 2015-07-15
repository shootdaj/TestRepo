using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ZoneLighting;
using ZoneLighting.ConfigNS;
using ZoneLighting.Usables;

namespace WebController
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
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

			//put ZLM into Application state
			System.Web.HttpContext.Current.Application.Lock();

			System.Web.HttpContext.Current.Application["ZLM"] = new ZLM(loadZonesFromConfig: !firstRun,
				loadProgramSetsFromConfig: !firstRun,
				loadZoneModules: loadZoneModules, initAction: initAction);

			System.Web.HttpContext.Current.Application.UnLock();
			
			AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
    }
}
