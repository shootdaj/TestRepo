using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ZoneLighting.ZoneProgramNS
{
	[Serializable]
	public class InputStartingValues : Dictionary<string, object>
	{
		protected InputStartingValues(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			
		}

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
