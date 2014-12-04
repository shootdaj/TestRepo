using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using ZoneLighting.ConfigNS;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ZoneProgramNS.Factories
{
	public static class ZoneScaffolder
	{
		//#region Singleton

		//public static ZoneScaffolder _instance;

		//public static ZoneScaffolder Instance
		//{
		//	get { return _instance; }
		//}

		//#endregion

		#region CORE

		public static IEnumerable<ExportFactory<ZoneProgram, IZoneProgramMetadata>> ZoneProgramFactories { get; set; }
		public static IEnumerable<ExportFactory<ZoneProgramParameter, IZoneProgramParameterMetadata>> ZoneProgramParameterFactories
		{
			get;
			set;
		}

		#endregion

		#region C+I

		public static bool Initialized { get; private set; }
		
		public static void Initialize(IEnumerable<ExportFactory<ZoneProgram, IZoneProgramMetadata>> zoneProgramFactories,
			IEnumerable<ExportFactory<ZoneProgramParameter, IZoneProgramParameterMetadata>> zoneProgramParameterFactories)

		{
			if (!Initialized)
			{
				ZoneProgramFactories = zoneProgramFactories;
				ZoneProgramParameterFactories = zoneProgramParameterFactories;
				Initialized = true;
			}
		}

		public static void Uninitialize()
		{
			if (Initialized)
			{
				ZoneProgramFactories = null;
				ZoneProgramParameterFactories = null;
				Initialized = false;
			}
		}

		#endregion

		#region API

		/// <summary>
		/// Creates a program parameter using Reflection, given the name of the program and a dictionary of property names to property values
		/// </summary>
		public static ZoneProgramParameter CreateProgramParameter(string programName, Dictionary<string, object> parameterDictionary)
		{
			var zoneProgramFactory = ZoneProgramFactories.ToDictionary(x => x.Metadata.Name)[programName];
			var programParameter =
				ZoneProgramParameterFactories.ToDictionary(x => x.Metadata.Name)[zoneProgramFactory.Metadata.ParameterName]
					.CreateExport();

			parameterDictionary.Keys.ToList().ForEach(propertyName =>
			{
				var property = programParameter.Value.GetType()
					.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
				if (property != null)
				{
					property.SetValue(programParameter.Value, parameterDictionary[propertyName]);
				}
			});

			return programParameter.Value;
		}

		/// <summary>
		/// Initializes a zone with the given program name and parameter name-value dictionary.
		/// </summary>
		public static void InitializeZone(Zone zone, string programName, Dictionary<string, object> parameterDictionary)
		{
			var zoneProgramFactoriesList = ZoneProgramFactories.ToList();
			var parameter = CreateProgramParameter(programName, parameterDictionary);
			zone.Initialize(zoneProgramFactoriesList.ToDictionary(x => x.Metadata.Name)[programName].CreateExport().Value, parameter);
		}

		/// <summary>
		/// Initializes a zone with the given program name and parameter.
		/// </summary>
		public static void InitializeZone(Zone zone, string programName, ZoneProgramParameter parameter = null)
		{
			zone.Initialize(ZoneProgramFactories.ToDictionary(x => x.Metadata.Name)[programName].CreateExport().Value, parameter);
		}

		/// <summary>
		/// Gets the names of all available programs.
		/// </summary>
		public static IEnumerable<string> AvailableProgramNames
		{
			get { return ZoneProgramFactories.Select(x => x.Metadata.Name); }
		}

		#endregion

		#region Macro API

		/// <summary>
		/// Initializes the given zones with information in the zone configuration saved in the zone configuration file.
		/// </summary>
		public static bool InitializeFromZoneConfiguration(List<Zone> zonesToLoadInto)
		{
			var configFile = ConfigurationManager.AppSettings["ZoneConfigurationSaveFile"];

			if (string.IsNullOrEmpty(configFile) || !File.Exists(configFile))
				return false;

			try
			{
				var zonesToLoadFrom = Config.LoadZones(configFile);

				zonesToLoadFrom.ToList().ForEach(zoneToLoadFrom =>
				{
					if (zonesToLoadInto.Select(zone => zone.Name).Contains(zoneToLoadFrom.Name) && zoneToLoadFrom.ZoneProgram != null)
					{
						var zoneToLoadInto =
							zonesToLoadInto.First(z => zoneToLoadFrom.Name == z.Name);
						
						var zoneProgramName = zoneToLoadFrom.ZoneProgram.Name;
						var zoneProgramParameter = zoneToLoadFrom.ZoneProgram is ParameterizedZoneProgram
							? ((ParameterizedZoneProgram) zoneToLoadFrom.ZoneProgram).ProgramParameter
							: null;
						
						InitializeZone(zoneToLoadInto, zoneProgramName, zoneProgramParameter);
					}
				});
			}
			catch (Exception ex)
			{
				return false;
			}

			return true;
		}

		#endregion
	}
}
