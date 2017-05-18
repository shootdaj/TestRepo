using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using ZoneLighting.Communication;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ZoneProgramNS.Factories
{
    /// <summary>
    /// Responsible for managing heavy tasks related to zones, especially those that 
    /// need interaction with external modules such as creating programs in zones. Part of 
    /// that responsibility also inclues interacting with MEF to load the external programs.
    /// </summary>
	public class ZoneScaffolder
	{
		#region Singleton

		private static ZoneScaffolder _instance;
		public static ZoneScaffolder Instance => _instance ?? (_instance = new ZoneScaffolder());

		#endregion

		#region CORE + MEF

		[ImportMany(typeof (ZoneProgram), AllowRecomposition = true)]
		public IList<ExportFactory<ZoneProgram, IZoneProgramMetadata>> ZoneProgramFactories { get; set; }

		/// <summary>
		/// Container for the external programs.
		/// </summary>
		private CompositionContainer ExternalProgramContainer { get; set; }

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
				ExternalProgramContainer?.Dispose();
				ExternalProgramContainer = null;
				ZoneProgramFactories = null;
				Initialized = false;
			}
		}

		/// <summary>
		/// Composes this class with external programs. 
		/// </summary>
		private void ComposeWithExternalModules(string programModuleDirectory)
		{
			List<ComposablePartCatalog> fileCatalogs = new List<ComposablePartCatalog>();

			//need to set this because otherwise for WebController, the file is loaded from c:\windows\system32\inetsrv probably
			//because that's where w3wp.exe is or something.. who knows.
			Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

			foreach (var file in Directory.GetFiles(programModuleDirectory, "*.dll").ToList())
			{
				var assembly = Assembly.LoadFrom(file);

				if (assembly.GetCustomAttributesData()
					.Any(ass => ass.AttributeType == typeof (ZoneProgramAssemblyAttribute)))
				{
					fileCatalogs.Add(new AssemblyCatalog(assembly));
					File.Copy(file, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(file)), true);
				}
			}

			var aggregateCatalog = new AggregateCatalog(fileCatalogs);
			ExternalProgramContainer = new CompositionContainer(aggregateCatalog);
			ZoneProgramFactories = new List<ExportFactory<ZoneProgram, IZoneProgramMetadata>>();
			ExternalProgramContainer.ComposeParts(this);
		}

		#endregion

		#region API

		public bool DoesProgramExist(string programName)
		{
			return GetAvailablePrograms().Contains(programName);
		}

	    public IEnumerable<string> GetAvailablePrograms()
	    {
		    return ZoneProgramFactories.Select(x => x.Metadata.Name);
	    }

		internal ZoneProgram CreateZoneProgram(string programName)
		{
			if (DoesProgramExist(programName))
				return ZoneProgramFactories.ToDictionary(x => x.Metadata.Name)[programName].CreateExport().Value;
			else
				throw new Exception($"No program by the name '{programName}' exists.");
		}

		/// <summary>
		/// Runs a zone with the given program name and starting values of the inputs as a name-value dictionary.
		/// </summary>
		public void RunZone(Zone zone, string programName, ISV isv = null, bool isSyncRequested = false, SyncContext syncContext = null, bool dontStart = false, dynamic startingParameters = null)
		{
			zone.Run(CreateZoneProgram(programName), isv, isSyncRequested, syncContext, dontStart, startingParameters);
		}


		public void AddInterruptingProgram(Zone zone, string programName, ISV isv = null, SyncContext syncContext = null)
		{
			var zoneProgram = CreateZoneProgram(programName);

			if (zoneProgram is ReactiveZoneProgram)
				zone.AddInterruptingProgram((ReactiveZoneProgram)zoneProgram, isv, syncContext);
			else
				throw new Exception("Given program is not a reactive program.");
		}
        
		/// <summary>
		/// Gets the names of all available programs.
		/// </summary>
		public IEnumerable<string> AvailableProgramNames
		{
			get { return ZoneProgramFactories.Select(x => x.Metadata.Name); }
		}
        
		public OPCZone AddFadeCandyZone(BetterList<Zone> zones, string name, OPCPixelType pixelType, int numberOfLights, byte? channel, double? brightness = null)
		{
		    return AddOPCZone(zones, name, FadeCandyController.Instance, pixelType, numberOfLights, channel, brightness);
		}

        public OPCZone AddNodeMCUZone(BetterList<Zone> zones, string name, OPCPixelType pixelType, int numberOfLights, byte? channel, double? brightness = null)
        {
            return AddOPCZone(zones, name, NodeMCUController.Instance, pixelType, numberOfLights, channel, brightness);
        }

        public OPCZone AddOPCZone(BetterList<Zone> zones, string name, OPCWebSocketController lightingController, OPCPixelType pixelType, int numberOfLights,
	        byte? channel, double? brightness = null)
	    {
            //create new zone
            var zone = new OPCZone(lightingController, name, brightness, (byte)channel);

            //add lights and add zone to collection
            ((OPCZone)zones.Add(zone)).AddOPCLights(pixelType, numberOfLights, (byte)channel);
            return zone;
        }

		public OPCZone AddFadeCandyZone(BetterList<Zone> zones, string name, OPCPixelType pixelType, Dictionary<int,int> logicalPhysicalMapping, byte? channel, double? brightness = null)
		{
			//create new zone
			var zone = new OPCZone(FadeCandyController.Instance, name, brightness, (byte)channel);

			//add lights and add zone to collection
			((OPCZone)zones.Add(zone)).AddOPCLights(pixelType, logicalPhysicalMapping, (byte)channel);
			return zone;
		}

		#endregion

		#region Macro API

		public void CreateProgramSetsFromConfig()
        {
            
        }

        

        public void CreateProgramSetsFromConfig(string config)
        {
            
        }

  //      //TODO: Convert this method to CreateProgramSetsFromConfig or something like that. And another test called CreateZonesFromConfiguration.
		///// <summary>
		///// Initializes the given zones with information about the zone configuration saved in the zone configuration file.
		///// Note that this method does not create any zones. It simply loads up the configuration and matches up the loaded configuration
		///// with zones that already exist in zonesToLoadInto using the name. If there exist zones with the same name,
		///// it will "map" the loaded zones to its respective complement in zonesToLoadInto.
		///// </summary>
		///// <param name="zonesToLoadInto">Zones to initialize new zones into</param>
		///// <param name="configFile">If provided, use this configuration file to load zones from</param>
		//public bool InitializeFromZoneConfiguration(IList<Zone> zonesToLoadInto, string configFile)
		//{
		//	if (string.IsNullOrEmpty(configFile) || !File.Exists(configFile))
		//		return false;

		//	try
		//	{
		//		//strategy is to load a temporary list of zones from the configuration which will then be 
		//		//used to initialize zonesToLoadInto. Note that this temporary list of zones is scoped to this method
		//		//It is used only to get the important values like the program and input starting values and initialize
		//		//zonesToLoadInto from those values.
		//		var zonesToLoadFrom = ZoneConfig.DeserializeZones(File.ReadAllText(configFile));
				
		//		zonesToLoadFrom.ToList().ForEach(zoneToLoadFrom =>
		//		{
		//			if (zonesToLoadInto.Select(zone => zone.Name).Contains(zoneToLoadFrom.Name) && zoneToLoadFrom.ZoneProgram != null)
		//			{
		//				var zoneToLoadInto = zonesToLoadInto.First(z => zoneToLoadFrom.Name == z.Name);
		//				var zoneProgramName = zoneToLoadFrom.ZoneProgram.Name;
		//				ISV startingValues = zoneToLoadFrom.ZoneProgram.GetInputValues();

		//				//start the main program
		//				RunZone(zoneToLoadInto, zoneProgramName, startingValues);

		//				//TODO: start the interrupting programs
		//				//old shit
		//				//zoneToLoadFrom.InterruptingPrograms.ToList().ForEach(program =>
		//				//{
		//				//	zoneToLoadInto.AddInterruptingProgram(program, true, program.GetInputValues());
		//				//});
		//			}
		//		});
		//	}
		//	catch (Exception ex)
		//	{
		//		return false;
		//	}

		//	return true;
		//}

		#endregion
	}
}