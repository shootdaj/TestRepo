using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
			base.InsertItem(index, item);
		}
	}
}
