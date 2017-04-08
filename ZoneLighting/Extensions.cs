using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZoneLighting.Usables;

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


		//TODO: Remove this and use the Listify in Anshul.Utilities after updating it to the latest version
		/// <summary>
		/// Takes an object and turns it into a list of its type. Basically puts the incoming object
		/// into a list.
		/// </summary>
		public static List<T> Listify<T>(this T input)
		{
			var list = new List<T> {input};
			return list;
		}

		/// <summary>
		/// This runs the given actions for each item in the given list on separate threads simultaneously (TaskCreationOptions.LongRunning).
		/// </summary>
		public static void Parallelize<T>(this List<T> list, Action<T> action)
		{
			var tasks = new List<Task>();
			list.ForEach(t => tasks.Add(Task.Factory.StartNew(() => action(t), TaskCreationOptions.LongRunning)));
			Task.WaitAll(tasks.ToArray());
		}
	}
}
