using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Refigure;
using WebController.Controllers;
using WebController.IoC;
using ZoneLighting;
using ZoneLighting.Usables;

namespace WebController
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			Container.CreateZLM();
			Container.CreateZLMRPC();

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
	}
}
