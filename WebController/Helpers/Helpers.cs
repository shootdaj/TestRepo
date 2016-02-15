using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebController.Helpers
{
	public class Helper
	{
		public static IEnumerable<Type> GetAllAssignableTypes<TBaseType>()
		{
			var baseType = typeof(TBaseType);
			var assembly = baseType.Assembly;

			return assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t));
		}
	}
}
