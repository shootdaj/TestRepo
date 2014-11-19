using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZoneLighting;

namespace WebController.Extensions
{
	public static class WebControllerExtensions
	{
		public static SelectList ToSelectList(this IList<Zone> zones)
		{
			var values = from Zone z in zones
						 select new { ID = z.Name, z.Name };
			return new SelectList(values, "ID", "Name", zones);
		}
	}
}