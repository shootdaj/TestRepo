using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ZoneProgramNS
{
	//public class ZoneProgramJsonConverter : JsonConverter
	//{
	//	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	//	{
	//		var zone = (ZoneProgram)value;
	//		writer.WriteStartObject();
	//		writer.WritePropertyName();

	//		serializer.Serialize(writer, zone.ZoneProgram);
	//		writer.WriteEndObject();
	//	}

	//	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public override bool CanConvert(Type objectType)
	//	{
	//		return typeof(Zone).IsAssignableFrom(objectType);
	//	}
	//}
}
