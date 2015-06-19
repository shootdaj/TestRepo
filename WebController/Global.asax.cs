using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ZoneLighting;

namespace WebController
{
    public class MvcApplication : HttpApplication
    {
		//public static ZLM ZLM { get; set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

			//ZLM = new ZLM();

			HttpContext.Current.Application.Lock();
			HttpContext.Current.Application["ZLM"] = new ZLM();
			HttpContext.Current.Application.UnLock();
		}
    }
}
