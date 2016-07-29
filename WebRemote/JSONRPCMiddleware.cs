using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using AppFunc = System.Func<
	System.Collections.Generic.IDictionary<string, object>,
	System.Threading.Tasks.Task
>;

namespace WebRemote
{
	public class JSONRPCMiddleware
	{
		AppFunc _next;
		JSONRPCMiddlewareOptions _options;

		public JSONRPCMiddleware(AppFunc next, JSONRPCMiddlewareOptions options)
		{
			_next = next;
			_options = options;

			if (_options.OnIncomingRequest == null)
				_options.OnIncomingRequest = (ctx) => { Debug.WriteLine("Incoming request: " + ctx.Request.Path); };

			if (_options.OnOutgoingRequest == null)
				_options.OnOutgoingRequest = (ctx) => { Debug.WriteLine("Outgoing request: " + ctx.Request.Path); };

		}

		public async Task Invoke(IDictionary<string, object> environment)
		{
			var ctx = new OwinContext(environment);

			_options.OnIncomingRequest(ctx);
			await _next(environment);
			_options.OnOutgoingRequest(ctx);
		}
	}

	public class JSONRPCMiddlewareOptions
	{
		public Action<IOwinContext> OnIncomingRequest { get; set; }
		public Action<IOwinContext> OnOutgoingRequest { get; set; }
	}

	public static class JSONRPCMiddlewareExtensions
	{
		public static void UseJSONRPCMiddleware(this IAppBuilder app, JSONRPCMiddlewareOptions options = null)
		{
			if (options == null)
				options = new JSONRPCMiddlewareOptions();

			app.Use<JSONRPCMiddleware>(options);
		}
	}
}