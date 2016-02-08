using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AustinHarris.JsonRpc;
using Newtonsoft.Json;
using WebController.IoC;

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
