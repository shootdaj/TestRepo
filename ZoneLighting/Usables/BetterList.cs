using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoneLighting.Usables
{
	public class BetterList<T> : List<T> where T : IBetterListType
	{
		public T this[string name]
		{
			get { return this.First(t => t.Name == name); }
		}
	
		/// <summary>
		/// Adds and returns the object passed in for fluent interfacing.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public new T Add(T t)
		{
			base.Add(t);
			return t;
		}
    }
}
