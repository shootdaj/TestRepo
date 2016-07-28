using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Http;
using Owin;
using WebRemote.IoC;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace WebRemote
{
	public class Startup
	{
		// This code configures Web API. The Startup class is specified as a type
		// parameter in the WebApp.Start method.
		public void Configuration(IAppBuilder app)
		{
			// Configure Web API for self-host. 
			HttpConfiguration config = new HttpConfiguration();
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "{controller}/{action}",
				defaults: new { controller = "ZLM", action = "Index" }

			);

			//appBuilder.Use(new Func<AppFunc, AppFunc>(next => (async env =>
			//{
			//	Console.WriteLine("Begin Request");
			//	await next.Invoke(env);
			//	Console.WriteLine("End Request");
			//})));

			//app.UseJSONRPCMiddleware(new JSONRPCMiddlewareOptions
			//{
			//	OnIncomingRequest = (ctx) =>
			//	{
			//		var watch = new Stopwatch();
			//		watch.Start();
			//		ctx.Environment["DebugStopwatch"] = watch;
			//	},
			//	OnOutgoingRequest = (ctx) =>
			//	{
			//		var watch = (Stopwatch)ctx.Environment["DebugStopwatch"];
			//		watch.Stop();
			//		Debug.WriteLine("Request took: " + watch.ElapsedMilliseconds + " ms");
			//	}
			//});


			//config.MessageHandlers.

			config.Routes.IgnoreRoute("JSONRPC", "json.rpc");
			config.Routes.IgnoreRoute("Glimpse", "{resource}.axd/{*pathInfo}");

			app.UseWebApi(config);

			Container.CreateZLM();
			Container.CreateZLMRPC();
		}
	}
}
