﻿using System.Collections.Generic;

namespace WebRemote
{
    public class Startup
    {
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddMvc();
		}

        public void Configure(IApplicationBuilder app)
        {
			app.UseMvc();

			var options = new StaticFileOptions
			{
				ContentTypeProvider = new FileExtensionContentTypeProvider()
			};
			((FileExtensionContentTypeProvider)options.ContentTypeProvider).Mappings.Add(
				new KeyValuePair<string, string>(".json", "application/json"));
			app.UseStaticFiles(options);
		}
    }
}