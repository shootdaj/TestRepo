using System;
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
		#region CONSTANTS

		//private static string[] InitStrings = { "initialize"
		//										,"init"
		//};

		//private static string[] UninitStrings = { "uninitialize"
		//										,"uninit"
		//};

		#endregion

		public static BetterList<T> ToBetterList<T>(this IEnumerable<T> list) where T : IBetterListType
		{
			var betterList = new BetterList<T>();
			betterList.AddRange(list);
			return betterList;
		}

		//public static bool (this string input)
		//{
			
		//}
	}
}
