using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.ConfigNS
{
	public class ZoneConfig
	{
		public static JsonSerializerSettings SaveZonesSerializerSettings => new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.All,
			Formatting = Formatting.Indented
		};

		public static JsonSerializerSettings LoadZonesSerializerSettings => new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.All,
			Formatting = Formatting.Indented,
			Converters = new JsonConverter[]{new UnderlyingTypeConverter()}
		};


		public static void SaveZones(IEnumerable<Zone> zones, string filename)
		{
			var json = JsonConvert.SerializeObject(zones, SaveZonesSerializerSettings);
			File.WriteAllText(filename, json);
		}

		public static IEnumerable<Zone> LoadZones(string filename = "", string zoneConfiguration = "")
		{
			var deserializedZones =
				JsonConvert.DeserializeObject(
					string.IsNullOrEmpty(zoneConfiguration) ? File.ReadAllText(filename) : zoneConfiguration,
					LoadZonesSerializerSettings);
			return (IEnumerable<Zone>) deserializedZones;
		}
	}
}
