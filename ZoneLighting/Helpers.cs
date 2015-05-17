using System;
using System.Collections.Generic;

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
	}
}
