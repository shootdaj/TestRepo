﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;

namespace ZoneLighting
{
	public static class Extensions
	{
		public static BetterList<T> ToBetterList<T>(this IEnumerable<T> list) where T : IBetterListType
		{
			var betterList = new BetterList<T>();
			betterList.AddRange(list);
			return betterList;
		}

		/// <summary>
		/// Takes an object and turns it into a list of its type. Basically puts the incoming object
		/// into a list.
		/// </summary>
		public static List<T> Listify<T>(this T input)
		{
			var list = new List<T> {input};
			return list;
		}
	}
}
