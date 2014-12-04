using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZoneLighting.ZoneProgramNS
{
	public abstract class ZoneProgramParameter : IEquatable<ZoneProgramParameter>
	{
		public bool Equals(ZoneProgramParameter other)
		{
			bool returnValue = true;
			this.GetType()
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.ToList()
				.ForEach(prop => returnValue &= prop.GetValue(this).Equals(prop.GetValue(other)));
			return returnValue;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Outputs a key-value dictionary of the names and values of all properties of this instance.
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, object> ToKeyValueDictionary()
		{
			var dictionary = new Dictionary<string, object>();

			GetType()
				.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList().ForEach(property =>
				{
					dictionary.Add(property.Name, property.GetValue(this));
				});

			return dictionary;
		}

		/// <summary>
		/// Reads in properties from a key value dictionary that contains the property name and its value.
		/// </summary>
		public void ReadProperties(Dictionary<string, object> properties)
		{
			properties.Keys.ToList().ForEach(propertyName =>
			{
				var property = this.GetType()
					.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
				if (property != null)
				{
					property.SetValue(this, properties[propertyName]);
				}
			});
		}
	}
}
