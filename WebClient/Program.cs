using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace WebClient
{
	class Program
	{
		static void Main(string[] args)
		{
			var url = "http://localhost:9001";

			var physicalFileSystem = new PhysicalFileSystem(@".\app");
			var options = new FileServerOptions
			{
				EnableDefaultFiles = true,
				FileSystem = physicalFileSystem
			};
			options.StaticFileOptions.FileSystem = physicalFileSystem;
			options.StaticFileOptions.ServeUnknownFileTypes = true;
			//options.DefaultFilesOptions.DefaultFileNames = new[] { "index.html" }; -- this line is not technically needed
																				//	-- because index.html is one of the default files
																				//	-- by default
			WebApp.Start(url, builder => builder.UseFileServer(options));

			Console.WriteLine("Listening at " + url);
			Console.ReadLine();
		}
	}
}
