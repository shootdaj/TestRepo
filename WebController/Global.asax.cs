using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Refigure;
using WebController.ContainerNS;
using WebController.Controllers;
using ZoneLighting;
using ZoneLighting.Usables;

namespace WebController
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			ZLMRPC.CreateZLM();
			Container.ZLMRPC = new ZLMRPC(Container.ZLM);

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
	}
}
