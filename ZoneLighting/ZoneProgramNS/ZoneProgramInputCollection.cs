using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoneLighting.ZoneProgramNS
{
	public class ZoneProgramInputCollection : KeyedCollection<string, ZoneProgramInput<dynamic>>
	{
		protected override string GetKeyForItem(ZoneProgramInput<dynamic> item)
		{
			return item.Name;
		}
	}
}
