using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ZoneLighting
{
	public static class Helpers
	{
		/// <summary>
		/// Takes an object and turns it into a list of its type. Basically puts the incoming object
		/// into a list.
		/// </summary>
		public static List<T> Listify<T>(T[] inputs)
		{
			var list = new List<T>();
			Array.ForEach(inputs, input => list.Add(input));
			return list;
		}

		public static bool HasProperty(dynamic obj, string name)
		{
			Type objType = obj.GetType();

			if (objType == typeof(ExpandoObject))
			{
				return ((IDictionary<string, object>)obj).ContainsKey(name);
			}

			return objType.GetProperty(name) != null;
		}
	}
}
