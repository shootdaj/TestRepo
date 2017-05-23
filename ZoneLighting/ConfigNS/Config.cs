using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Anshul.Utilities;
using Newtonsoft.Json;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.ConfigNS
{
	public class Config
	{
		#region Serialization Settings

		public static JsonSerializerSettings SaveZonesSerializerSettings => new JsonSerializerSettings()
		{
			//PreserveReferencesHandling = PreserveReferencesHandling.All,
			TypeNameHandling = TypeNameHandling.All,
			TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full,
			Formatting = Formatting.Indented
		};

		public static JsonSerializerSettings LoadZonesSerializerSettings => new JsonSerializerSettings()
		{
			//PreserveReferencesHandling = PreserveReferencesHandling.All,
			TypeNameHandling = TypeNameHandling.All,
			TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full,
			Formatting = Formatting.Indented,
			Converters = new JsonConverter[] { new UnderlyingTypeConverter() }
		};

		public static JsonSerializerSettings LoadProgramSetsSerializerSettings => new JsonSerializerSettings()
		{
			//PreserveReferencesHandling = PreserveReferencesHandling.All,
			TypeNameHandling = TypeNameHandling.All,
			TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full,
			Formatting = Formatting.Indented,
			Converters = new JsonConverter[] { new UnderlyingTypeConverter() }
		};

		public static JsonSerializerSettings SaveProgramSetsSerializerSettings => new JsonSerializerSettings()
		{
			//PreserveReferencesHandling = PreserveReferencesHandling.All,
			TypeNameHandling = TypeNameHandling.All,
			TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full,
			Formatting = Formatting.Indented
		};

		#endregion

		public static string SerializeProgramSets(IEnumerable<ProgramSet> programSets)
		{
			return Serialize(programSets, SaveProgramSetsSerializerSettings);
		}

		public static string SerializeZones(IEnumerable<Zone> zones)
		{
			zones.ToList().ForEach(zone => zone.SetZoneProgramInputs());
			return Serialize(zones, SaveZonesSerializerSettings);
		}

		public static string Serialize(object toSerialize, JsonSerializerSettings settings)
		{
			return JsonConvert.SerializeObject(toSerialize, settings);
		}

		public static BetterList<Zone> DeserializeZones(string config)
		{
			var zones = ((IEnumerable<Zone>)JsonConvert.DeserializeObject(config, LoadZonesSerializerSettings)).ToBetterList();
			zones.ForEach(AssignLightingController);
			return zones.ToBetterList();
		}

		public static BetterList<ProgramSet> DeserializeProgramSets(string config, IEnumerable<Zone> zones)
		{
			//deserialize the program sets
			var deserializedProgramSets = ((IEnumerable<ProgramSet>)JsonConvert.DeserializeObject(config, LoadProgramSetsSerializerSettings))
				.ToBetterList();

			//recreate the program sets from scratch using values from the deserialized ones
			var reinstantiatedProgramSets = new BetterList<ProgramSet>();
			deserializedProgramSets.ForEach(deserializedProgramSet =>
			{
				var zonesEnumerated = zones as IList<Zone> ?? zones.ToList();
				var zonesToPassIn = zonesEnumerated.Where(z => deserializedProgramSet.Zones.Select(dz => dz.Name).Contains(z.Name));
				List<ISV> isvs = null;

				//prepare isv from deserialized zones
				foreach (var zone in deserializedProgramSet.Zones.Where(zone => zone.ZoneProgramInputs.ContainsKey(deserializedProgramSet.ProgramName)))
				{
					if (isvs == null)
						isvs = new List<ISV>();

					isvs.Add(zone.ZoneProgramInputs[deserializedProgramSet.ProgramName].ToISV());
				}
				
				//create new program set with all values from the deserialized version
				reinstantiatedProgramSets.Add(new ProgramSet(deserializedProgramSet.ProgramName, zonesToPassIn,
					deserializedProgramSet.Sync, isvs, deserializedProgramSet.Name));
			});

			//dump the deserialized program sets
			deserializedProgramSets.ForEach(programSet =>
			{
				programSet.Zones.Clear();
				programSet.Dispose(true);
			});

			return reinstantiatedProgramSets;
		}
		public static void SaveZones(IEnumerable<Zone> zones, string filename)
		{
			var json = SerializeZones(zones);
			File.WriteAllText(filename, json);
		}

		public static void SaveProgramSets(IEnumerable<ProgramSet> programSets, string filename)
		{
			var json = SerializeProgramSets(programSets);
			File.WriteAllText(filename, json);
		}

        private static void AssignLightingController(Zone zone)
		{
            //TODO: This is where during the reloading of the zone, its previous lighting controller need to be attached back to it.
            
			//if (zone.GetType() == typeof(OPCZone))
			//{
			//	zone.SetLightingController(FadeCandyController.Instance);
			//}
		}

		public static IEnumerable<Zone> LoadZones(string filename = "", string zoneConfiguration = "")
		{
			var deserializedZones =
				JsonConvert.DeserializeObject(
					string.IsNullOrEmpty(zoneConfiguration) ? File.ReadAllText(filename) : zoneConfiguration,
					LoadZonesSerializerSettings);
			return (IEnumerable<Zone>)deserializedZones;
		}
    }
}
