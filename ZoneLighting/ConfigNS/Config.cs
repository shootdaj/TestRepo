﻿using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ConfigNS
{
	public class Config
	{
		public static void SaveZones(IEnumerable<Zone> zones, string filename)
		{
			var settings = new JsonSerializerSettings
			{
				//PreserveReferencesHandling = PreserveReferencesHandling.All,
				TypeNameHandling = TypeNameHandling.All,
				Formatting = Formatting.Indented,
				//Converters = new List<JsonConverter>() { new ZonesJsonConverter() }
			};

			var json = JsonConvert.SerializeObject(zones, settings);
			File.WriteAllText(filename, json);
		}

		public static IEnumerable<Zone> LoadZones(string filename)
		{
			var deserializedZones = JsonConvert.DeserializeObject<IEnumerable<Zone>>(File.ReadAllText(filename),
				new JsonSerializerSettings
				{
					//PreserveReferencesHandling = PreserveReferencesHandling.All,
					TypeNameHandling = TypeNameHandling.All,
					Formatting = Formatting.Indented,
					//Converters = new List<JsonConverter>() { new ZonesJsonConverter() }
				});

			return deserializedZones;
		}
	}
}