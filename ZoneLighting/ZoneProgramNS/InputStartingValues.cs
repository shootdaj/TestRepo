using System;
using System.Collections.Generic;
using System.Dynamic;
using AustinHarris.JsonRpc;
using Newtonsoft.Json;

namespace ZoneLighting.ZoneProgramNS
{
	/// <summary>
	/// Input Starting Values = ISV
	/// </summary>
	[Serializable]
	[JsonConverter(typeof(ISVConverter))]
	public class ISV : DynamicObject
	{
		public ISV(Dictionary<string, object> dictionary)
		{
			Dictionary = dictionary;
		}

		public ISV()
		{
		}

		//[JsonConverter(typeof(Int32DictionaryConverter))]
		public Dictionary<string, object> Dictionary = new Dictionary<string, object>();

		public object this[string key] => Dictionary[key];

		public Dictionary<string, object>.KeyCollection Keys => Dictionary.Keys;

		public void Add(string name, object value)
		{
			Dictionary.Add(name, value);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			object data;
			if (!Dictionary.TryGetValue(binder.Name, out data))
			{
				throw new KeyNotFoundException("There's no key by that name");
			}

			result = data;
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			if (Dictionary.ContainsKey(binder.Name))
			{
				Dictionary[binder.Name] = value;
			}
			else
			{
				Dictionary.Add(binder.Name, value);
			}

			return true;
		}
	}
}