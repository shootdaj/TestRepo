using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ZoneLighting.ZoneProgramNS
{
	/// <summary>
	/// Input Starting Values = ISV
	/// </summary>
	[Serializable]
	public class ISV : DynamicObject
	{
		public ISV()
		{ }

		private Dictionary<string, object> _dictionary = new Dictionary<string, object>();

		public object this[string key] => _dictionary[key];

		public Dictionary<string, object>.KeyCollection Keys => _dictionary.Keys;

		public void Add(string name, object value)
		{
			_dictionary.Add(name, value);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			object data;
			if (!_dictionary.TryGetValue(binder.Name, out data))
			{
				throw new KeyNotFoundException("There's no key by that name");
			}

			result = data;
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			if (_dictionary.ContainsKey(binder.Name))
			{
				_dictionary[binder.Name] = value;
			}
			else
			{
				_dictionary.Add(binder.Name, value);
			}

			return true;
		}
	}
}


