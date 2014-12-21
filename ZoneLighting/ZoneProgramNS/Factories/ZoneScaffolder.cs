using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using ZoneLighting.ConfigNS;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ZoneProgramNS.Factories
{
	public class ZoneScaffolder
	{
		#region Singleton

		private static ZoneScaffolder _instance;
		public static ZoneScaffolder Instance => _instance ?? (_instance = new ZoneScaffolder());

		#endregion

		#region CORE + MEF

		[ImportMany(typeof (ZoneProgram), AllowRecomposition = true)]
		public IList<ExportFactory<ZoneProgram, IZoneProgramMetadata>> ZoneProgramFactories { get; set; } = new List<ExportFactory<ZoneProgram, IZoneProgramMetadata>>();

		/// <summary>
		/// Container for the external programs.
		/// </summary>
		private CompositionContainer ExternalProgramContainer { get; set; }

		/// <summary>
		/// Directory Catalog that stores catalog for the external programs
		/// </summary>
		private DirectoryCatalog ExternalProgramCatalog { get; set; }

		#endregion

		#region C+I

		public bool Initialized { get; private set; }

		public void Initialize(string programModuleDirectory)
		{
			if (!Initialized)
			{
				ComposeWithExternalModules(programModuleDirectory);
				Initialized = true;
			}
		}

		public void Uninitialize()
		{
			if (Initialized)
			{
				ZoneProgramFactories = null;
				Initialized = false;
			}
		}

		/// <summary>
		/// Creates catalog for external programs.
		/// </summary>
		private void CatalogExternalPrograms(string programModuleDirectory)
		{
			ExternalProgramCatalog = new DirectoryCatalog(programModuleDirectory);
		}

		/// <summary>
		/// Composes this class with external programs. 
		/// </summary>
		private void ComposeWithExternalModules(string programModuleDirectory)
		{
			CatalogExternalPrograms(programModuleDirectory);
			var aggregateCatalog = new AggregateCatalog(ExternalProgramCatalog);
			ExternalProgramContainer = new CompositionContainer(aggregateCatalog);
			ExternalProgramContainer.ComposeParts(this);
		}

		#endregion

		#region API

		internal ZoneProgram CreateZoneProgram(string programName, IList<ExportFactory<ZoneProgram, IZoneProgramMetadata>> zoneProgramFactoriesList)
		{
			return zoneProgramFactoriesList.ToDictionary(x => x.Metadata.Name)[programName].CreateExport().Value;
		}

		/// <summary>
		/// Initializes a zone with the given program name and starting values of the inputs as a name-value dictionary.
		/// </summary>
		public void InitializeZone(Zone zone, string programName, InputStartingValues inputStartingValues = null)
		{
			var zoneProgramFactoriesList = ZoneProgramFactories.ToList();
			zone.Initialize(CreateZoneProgram(programName, zoneProgramFactoriesList), inputStartingValues);
		}

		/// <summary>
		/// Initializes a zone with the given program instance and starting values of the inputs as a name-value dictionary.
		/// </summary>
		public void InitializeZone(Zone zone, ZoneProgram zoneProgram, InputStartingValues inputStartingValues = null)
		{
			zone.Initialize(zoneProgram, inputStartingValues);
		}

		/// <summary>
		/// Gets the names of all available programs.
		/// </summary>
		public IEnumerable<string> AvailableProgramNames
		{
			get { return ZoneProgramFactories.Select(x => x.Metadata.Name); }
		}

		#endregion

		#region Macro API

		/// <summary>
		/// Initializes the given zones with information about the zone configuration saved in the zone configuration file.
		/// Note that this method does not create any zones. It simply loads up the configuration and matches up the loaded configuration
		/// with zones that already exist in zonesToLoadInto using the name. If there exist zones with the same name,
		/// it will "map" the loaded zones to its respective complement in zonesToLoadInto.
		/// </summary>
		/// <param name="zonesToLoadInto">Zones to initialize new zones into</param>
		/// <param name="configFile">If provided, use this configuration file to load zones from</param>
		public bool InitializeFromZoneConfiguration(IList<Zone> zonesToLoadInto, string configFile = "")
		{
			if (string.IsNullOrEmpty(configFile))
				configFile = ConfigurationManager.AppSettings["ZoneConfigurationSaveFile"];

			if (string.IsNullOrEmpty(configFile) || !File.Exists(configFile))
				return false;

			try
			{
				//strategy is to load a temporary list of zones from the configuration which will then be 
				//used to initialize zonesToLoadInto. Note that this temporary list of zones is scoped to this method
				//It is used only to get the important values like the program and input starting values and initialize
				//zonesToLoadInto from those values.
				var zonesToLoadFrom = Config.LoadZones(configFile);
				
				zonesToLoadFrom.ToList().ForEach(zoneToLoadFrom =>
				{
					if (zonesToLoadInto.Select(zone => zone.Name).Contains(zoneToLoadFrom.Name) && zoneToLoadFrom.ZoneProgram != null)
					{
						var zoneToLoadInto = zonesToLoadInto.First(z => zoneToLoadFrom.Name == z.Name);
						var zoneProgramName = zoneToLoadFrom.ZoneProgram.Name;
						InputStartingValues startingValues = zoneToLoadFrom.ZoneProgram.GetInputValues();

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