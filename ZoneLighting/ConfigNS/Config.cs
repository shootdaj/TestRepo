using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ZoneLighting.Communication;
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
			PreserveReferencesHandling = PreserveReferencesHandling.All,
			TypeNameHandling = TypeNameHandling.All,
			TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full,
            Formatting = Formatting.Indented
		};

		public static JsonSerializerSettings LoadZonesSerializerSettings => new JsonSerializerSettings()
		{
			PreserveReferencesHandling = PreserveReferencesHandling.All,
			TypeNameHandling = TypeNameHandling.All,
			TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full,
            Formatting = Formatting.Indented,
			Converters = new JsonConverter[] { new UnderlyingTypeConverter() }
		};

		public static JsonSerializerSettings LoadProgramSetsSerializerSettings => new JsonSerializerSettings()
		{
			PreserveReferencesHandling = PreserveReferencesHandling.All,
			TypeNameHandling = TypeNameHandling.All,
			TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full,
			Formatting = Formatting.Indented,
			Converters = new JsonConverter[] { new UnderlyingTypeConverter() }
		};

		public static JsonSerializerSettings SaveProgramSetsSerializerSettings => new JsonSerializerSettings()
		{
			PreserveReferencesHandling = PreserveReferencesHandling.All,
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
			var deserializedProgramSets = new BetterList<ProgramSet>();

			deserializedProgramSets =
					((IEnumerable<ProgramSet>)JsonConvert.DeserializeObject(config, LoadProgramSetsSerializerSettings))
						.ToBetterList();


			var reinstantiatedProgramSets = new BetterList<ProgramSet>();
			deserializedProgramSets.ForEach(deserializedProgramSet =>
			{
				var zonesEnumerated = zones as IList<Zone> ?? zones.ToList();
				var zonesToPassIn = zonesEnumerated.Where(z => deserializedProgramSet.Zones.Select(dz => dz.Name).Contains(z.Name));
				reinstantiatedProgramSets.Add(new ProgramSet(deserializedProgramSet.ProgramName, zonesToPassIn,
					deserializedProgramSet.Sync, deserializedProgramSet.Zones.First().ZoneProgramInputs.First().Item2 //TODO: This is not correct, if the programs had different input values, this scenario doesn't handle that
																										//TODO: We need to save the inputs with each program and modify the ProgramSet constructor to be able to take in
																										//TODO: multiple ISVs and populate them according to the zone.
					, deserializedProgramSet.Name));
			});

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
			if (zone.GetType() == typeof(FadeCandyZone))
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
			return (IEnumerable<Zone>)deserializedZones;
		}


		public static string Get(string settingName, string exceptionMessage = null)
		{
			var configValue = ConfigurationManager.AppSettings[settingName];
			if (exceptionMessage != null && string.IsNullOrEmpty(configValue))
				throw new Exception(exceptionMessage);
			return configValue;
		}
	}
}
