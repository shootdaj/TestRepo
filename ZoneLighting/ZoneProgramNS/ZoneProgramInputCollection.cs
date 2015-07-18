using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ZoneLighting.ZoneProgramNS
{
	[DataContract]
	public class ZoneProgramInputCollection : KeyedCollection<string, ZoneProgramInput>
	{
		protected override string GetKeyForItem(ZoneProgramInput item)
		{
			return item.Name;
		}

		protected override void InsertItem(int index, ZoneProgramInput item)
		{
			if (item == null)
				throw new Exception("Cannot insert null values into this collection.");
			//try
			//{
				////override if it already exists
				//if (base.Contains(item.Name))
				//{
				//	//index = base.First(x => x.Name == i
				//	base.Remove(item.Name);
				//}

				base.InsertItem(index, item);
			//}
			//catch (Exception ex)
			//{
			//	// ignored
			//}
		}
	}
}
