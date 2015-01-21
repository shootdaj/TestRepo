using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using ZoneLighting;
using ZoneLighting.ZoneNS;

namespace WebController.Extensions
{
	public static class WebControllerExtensions
	{
		public static SelectList ToSelectList(this IEnumerable<Zone> zones)
		{
			if (zones != null)
			{
				var zonesEnumerated = zones as IList<Zone> ?? zones.ToList();
				if (zonesEnumerated.Any())
				{
					var values = from Zone z in zonesEnumerated
						select new {ID = z.Name, z.Name};
					return new SelectList(values, "ID", "Name", zones);
				}
				else
				{
					return new SelectList(new List<string>() {}, "none", "none");
				}
			}
			else
			{
				return new SelectList(new List<string>() { }, "none", "none");
			}
		}
	}
}