using System.Web.Mvc;
using System.Web.Routing;

namespace WebController
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "ZLM", action = "Index", id = UrlParameter.Optional }
                //defaults: new { controller = "ZLMNew", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
