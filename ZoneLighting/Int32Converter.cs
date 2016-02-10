using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZoneLighting
{
	public class Int32Converter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(long);
		}

		public override bool CanWrite => false;

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jsonValue = serializer.Deserialize<JValue>(reader);

			if (jsonValue.Type == JTokenType.Integer)
			{
				return jsonValue.Value<int>();
			}

			throw new FormatException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}