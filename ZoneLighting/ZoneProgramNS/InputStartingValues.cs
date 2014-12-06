using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoneLighting.ZoneProgramNS
{
	public class InputStartingValues : Dictionary<string, object>
	{
		public void SetValues(string name, object data)
		{
			Add(name, data);
		}

		public InputStartingValues(string name, object data)
		{
			Add(name, data);
		}

		public InputStartingValues()
		{
		}
	}
}
