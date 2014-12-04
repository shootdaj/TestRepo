using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.ZoneNS
{
	public class ZonesJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var zones = (IEnumerable<Zone>)value;
			
			writer.WriteStartObject();
			writer.WritePropertyName("Zones");
			writer.WriteStartArray();

			zones.ToList().ForEach(zone =>
			{
				writer.WriteStartObject();
				writer.WritePropertyName("Name");
				serializer.Serialize(writer, zone.Name);
				writer.WritePropertyName("ZoneProgram");
				writer.WriteStartObject();
				writer.WritePropertyName("Name");
				serializer.Serialize(writer, zone.ZoneProgram.Name);
				if (zone.ZoneProgram is ParameterizedZoneProgram)
				{
					writer.WritePropertyName("ProgramParameter");
					serializer.Serialize(writer, ((ParameterizedZoneProgram)zone.ZoneProgram).ProgramParameter);
				}
				writer.WriteEndObject();
				writer.WriteEndObject();

			});

			writer.WriteEndArray();
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jZones = JArray.Load(reader);
			IEnumerable<Zone> zones = new List<Zone>();

			
			jZones.ToList().ForEach(jZone =>
			{
				//jZone
			});


			return zones;
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(IEnumerable<Zone>).IsAssignableFrom(objectType);
		}
	}
}
