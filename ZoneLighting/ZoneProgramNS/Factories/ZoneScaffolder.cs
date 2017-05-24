using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using Anshul.Utilities;
using ZoneLighting.MEF;
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

        #region CORE

        public BetterList<ILightingController> LightingControllers { get; } =
            new BetterList<ILightingController>();

        #endregion

        #region MEF

		//test

			//test

        [ImportMany(typeof(ZoneProgram), AllowRecomposition = true)]
        public IList<ExportFactory<ZoneProgram, IZoneProgramMetadata>> ZoneProgramFactories { get; set; }

		[ImportMany(typeof(ILightingController), AllowRecomposition = true)]
	    public IList<ExportFactory<ILightingController, ILightingControllerMetadata>> LightingControllerFactories { get; set; }

		/// <summary>
		/// Container for the external modules.
		/// </summary>
		private CompositionContainer ModuleContainer { get; set; }

		#endregion

		#region C+I

		public bool Initialized { get; private set; }

        public void Initialize(string programModuleDirectory, string lightingControllerModuleDirectory)
        {
            if (!Initialized)
            {
                LightingControllerFactories = new List<ExportFactory<ILightingController, ILightingControllerMetadata>>();
	            ZoneProgramFactories = new List<ExportFactory<ZoneProgram, IZoneProgramMetadata>>();
				LoadModules(programModuleDirectory, lightingControllerModuleDirectory);
                Initialized = true;
            }
        }

        public void InitLightingControllers(ILightingControllerConfig config)
        {
			//for now - initialize a lighting controller for each of the lighting controller types imported
			//later - this needs to be driven by the user somehow - maybe during setup of ZL
			//or manually when they wanna add new controllers
			LightingControllerFactories.ToList().ForEach(factory =>
			{
				var lightingController = factory.CreateExport().Value;

				//var parameters = config.ToExpandoObject();
				var parameters = "yooy";

				lightingController.Initialize(parameters);
				LightingControllers.Add(lightingController);
			});
		}



        public void UninitLightingControllers()
        {
            LightingControllers.ForEach(x => x.Uninitialize());
        }

        public void Uninitialize()
        {
            if (Initialized)
            {
                UninitLightingControllers();
                ModuleContainer?.Dispose();
                ModuleContainer = null;
                ZoneProgramFactories = null;
                Initialized = false;
            }
        }

        /// <summary>
        /// Loads external ZoneProgram modules.
        /// </summary>
        private void LoadModules(string programModuleDirectory, string lightingControllerModuleDirectory)
        {
            var fileCatalogs = new List<ComposablePartCatalog>();

            //need to set this because otherwise for WebController, the file is loaded from c:\windows\system32\inetsrv probably
            //because that's where w3wp.exe is or something.. who knows.
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            
            LoadProgramModules(programModuleDirectory, fileCatalogs);
            LoadLightingControllerModules(lightingControllerModuleDirectory, fileCatalogs);

			var aggregateCatalog = new AggregateCatalog(fileCatalogs);
            ModuleContainer = new CompositionContainer(aggregateCatalog);
            ModuleContainer.ComposeParts(this);
        }

        private static void LoadLightingControllerModules(string lightingControllerModuleDirectory, List<ComposablePartCatalog> fileCatalogs)
        {
            LoadModulesCore(lightingControllerModuleDirectory, fileCatalogs, typeof(LightingControllerAssemblyAttribute));
        }

        private static void LoadProgramModules(string programModuleDirectory, List<ComposablePartCatalog> fileCatalogs)
        {
            LoadModulesCore(programModuleDirectory, fileCatalogs, typeof(ZoneProgramAssemblyAttribute));
        }

        private static void LoadModulesCore(string moduleDirectory, List<ComposablePartCatalog> fileCatalogs, Type assemblyAttribute)
        {
            foreach (var file in Directory.GetFiles(moduleDirectory, "*.dll").ToList())
            {
                var assembly = Assembly.LoadFrom(file);

                if (assembly.GetCustomAttributesData()
                    .Any(ass => ass.AttributeType == assemblyAttribute))
                {
                    fileCatalogs.Add(new AssemblyCatalog(assembly));

                    foreach (var referencedFile in Directory.GetFiles(Path.GetDirectoryName(file), "*.dll").ToList())
                    {
                        try
                        {
                            File.Copy(referencedFile,
                                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(referencedFile)),
                                false);
                        }
                        catch (Exception ex)
                        {
                            // only copy if file is not in use
                        }
                    }
                }
            }
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

        //public Zone AddFadeCandyZone(BetterList<Zone> zones, string name, int numberOfLights, double? brightness = null)
        //{
        //    return AddZone(zones, name, FadeCandyController.Instance, numberOfLights, brightness);
        //}

        public Zone AddNodeMCUZone(BetterList<Zone> zones, string name, int numberOfLights, double? brightness = null)
        {
            return AddZone(zones, name, LightingControllers["NodeMCUController1"], numberOfLights, brightness);
        }

        public Zone AddZone(BetterList<Zone> zones, string name, ILightingController lightingController, int numberOfLights,
            double? brightness = null)
        {
            //create new zone
            var zone = new Zone(lightingController, name, brightness);

            //add lights and add zone to collection
            zones.Add(zone).AddLights(numberOfLights);
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