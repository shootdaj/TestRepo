//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace ZoneLighting.Usables
//{
//	public class BetterList<T> : List<T> where T : IBetterListType<T>
//	{
//		public T this[string name]
//		{
//			get { return this.First(t => t.Name == name); }
//		}
	
//		/// <summary>
//		/// Adds and returns the object passed in for fluent interfacing.
//		/// Throws exception if an object with the same name is being added because 
//		/// name is unique across all items.
//		/// </summary>
//		/// <param name="t"></param>
//		/// <returns></returns>
//		public new T Add(T t)
//		{
//            if (this.Any(x => x.Name == t.Name))
//                throw new Exception($"An item with the name {t.Name} is already in the list.");

//			base.Add(t);
//			return t;
//		}
//    }
//}
