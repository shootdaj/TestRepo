using System;
using System.Collections.ObjectModel;

namespace ZoneLighting.ZoneProgramNS
{
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
			try
			{
				base.InsertItem(index, item);
			}
			catch
			{
				// ignored
			}
		}
	}
}
