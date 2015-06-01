using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ZoneLighting.Communication;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.ConfigNS
{
	public class ZoneConfig
	{
		public static JsonSerializerSettings SaveZonesSerializerSettings => new JsonSerializerSettings()
		{
            PreserveReferencesHandling = PreserveReferencesHandling.All,
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
			var json = SerializeZones(zones);
			File.WriteAllText(filename, json);
		}

	    public static string SerializeZones(IEnumerable<Zone> zones)
	    {
	        return JsonConvert.SerializeObject(zones, SaveZonesSerializerSettings);
	    }

		public static BetterList<Zone> DeserializeZones(string config)
		{
			var zones = ((IEnumerable<Zone>) JsonConvert.DeserializeObject(config, LoadZonesSerializerSettings)).ToBetterList();
			zones.ForEach(AssignLightingController);
			return zones.ToBetterList();
		}

		private static void AssignLightingController(Zone zone)
		{
			if (zone.LightingController is FadeCandyController)
			{
				zone.SetLightingController(FadeCandyController.Instance);
			}
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
