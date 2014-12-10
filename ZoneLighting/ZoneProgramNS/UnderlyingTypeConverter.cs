using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZoneLighting.ZoneProgramNS
{
	public class UnderlyingTypeConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			//var name = value as WierdName;
			//writer.WriteStartObject();
			//writer.WritePropertyName("$" + name.Name);
			//writer.WriteStartObject();
			//writer.WritePropertyName("nested");
			//serializer.Serialize(writer, name.Value);
			//writer.WriteEndObject();
			//writer.WriteEndObject();

			//var zoneProgramInput = (ZoneProgramInput) value;

			//writer.WriteStartObject();
			//writer.WritePropertyName("$type");
			//serializer.Serialize(writer, zoneProgramInput.GetType());
			//writer.WritePropertyName("Name");
			//serializer.Serialize(writer, zoneProgramInput.Name);
			//writer.WritePropertyName("Type");
			//serializer.Serialize(writer, zoneProgramInput.Type);
			//writer.WritePropertyName("Value");
			//serializer.Serialize(writer, zoneProgramInput.Value, zoneProgramInput.Type);
			//writer.WriteEndObject();


			//serializer.Serialize(writer, value);

			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{

			var result = new ZoneProgramInput();

			// Deserialize into a temporary JObject
			JObject obj = serializer.Deserialize<JObject>(reader);

			// Populate the ZoneProgramInput object with the contents
			serializer.Populate(obj.CreateReader(), result);

			// Overwrite the "Value" property with the correct value based on the 
			// "Type" property.
			result.Set(obj.GetValue("value", StringComparison.OrdinalIgnoreCase)
					.ToObject(result.Type, serializer));

			return result;













			//JObject jsonObject = JObject.Load(reader);

			//Color color = JsonConvert.DeserializeObject<Color>(jsonObject["Value"].ToString());

			//string name = "";
			//Type type = typeof (object);
			//reader.Read();
			//reader.Read();
			////var 

			////type = (Type)serializer.Deserialize(reader, type);

			//	//(Type) reader.Value;


			////IZoneProgramInput zoneProgramInput = new ZoneProgramInput<object>(name, type);
			////zoneProgramInput.Value = serializer.Deserialize()

			////var tempObject = (IZoneProgramInput)serializer.Deserialize(reader);
			////var realObject = serializer.Deserialize(reader, tempObject.Type);
			//return null; //zoneProgramInput;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(ZoneProgramInput);
		}
	}
}
