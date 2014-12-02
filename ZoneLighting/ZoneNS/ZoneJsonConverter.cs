using System;
using Newtonsoft.Json;

namespace ZoneLighting.ZoneNS
{
	public class ZoneJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var zone = (Zone)value;
			writer.WriteStartObject();
			writer.WritePropertyName("$" + zone.Name);
			serializer.Serialize(writer, zone.ZoneProgram); //TODO: fill in how to serialize ZoneProgram (as in Notepad++ zoneconfigurationexample.txt)
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof (Zone).IsAssignableFrom(objectType);
		}
	}
}
