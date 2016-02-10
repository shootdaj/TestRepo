using System;
using Newtonsoft.Json;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting
{
	public class ISVConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(ISV);
		}

		public override bool CanWrite => false;

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var result = new ISV();
			reader.Read();

			while (reader.TokenType == JsonToken.PropertyName)
			{
				var propertyName = (string)reader.Value;
				reader.Read();

				var value = reader.TokenType == JsonToken.Integer ? Convert.ToInt32(reader.Value) : serializer.Deserialize(reader);
				result.Add(propertyName, value);
				reader.Read();
			}

			return result;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}