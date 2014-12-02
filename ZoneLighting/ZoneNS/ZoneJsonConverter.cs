using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			//serializer.Serialize(writer, );
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
