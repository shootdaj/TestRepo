using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Linq;
using ZoneLighting.ConfigNS;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ZoneProgramNS.Factories
{
	public static class ZoneScaffolder
	{
		#region CORE

		public static IEnumerable<ExportFactory<ZoneProgram, IZoneProgramMetadata>> ZoneProgramFactories { get; set; }
		
		#endregion

		#region C+I

		public static bool Initialized { get; private set; }
		
		public static void Initialize(IEnumerable<ExportFactory<ZoneProgram, IZoneProgramMetadata>> zoneProgramFactories)

		{
			if (!Initialized)
			{
				ZoneProgramFactories = zoneProgramFactories;
				Initialized = true;
			}
		}

		public static void Uninitialize()
		{
			if (Initialized)
			{
				ZoneProgramFactories = null;
				Initialized = false;
			}
		}

		#endregion

		#region API
		
		/// <summary>
		/// Initializes a zone with the given program name and parameter name-value dictionary.
		/// </summary>
		public static void InitializeZone(Zone zone, string programName, InputStartingValues inputStartingValues = null)
		{
			var zoneProgramFactoriesList = ZoneProgramFactories.ToList();
			zone.Initialize(zoneProgramFactoriesList.ToDictionary(x => x.Metadata.Name)[programName].CreateExport().Value, inputStartingValues);
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

						InputStartingValues startingValues = zoneToLoadFrom.ZoneProgram.GetInputValues();

						//TODO: Replace with starting value for input
						//var zoneProgramParameter = zoneToLoadFrom.ZoneProgram is ParameterizedZoneProgram
						//	? ((ParameterizedZoneProgram) zoneToLoadFrom.ZoneProgram).ProgramParameter
						//	: null;
						
						InitializeZone(zoneToLoadInto, zoneProgramName, startingValues);
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
