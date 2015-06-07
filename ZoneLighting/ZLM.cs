using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ZoneLighting.Communication;
using ZoneLighting.ConfigNS;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLighting
{
    /// <summary>
    /// This class will be responsible for managing the higher level tasks for the zones.
    /// </summary>
    public sealed class ZLM : IDisposable
    {
        #region API

        public string GetZoneSummary()
        {
            var newline = Environment.NewLine;
            string summary = string.Format("Currently {0} zones are loaded." + newline, Zones.Count);
            summary += "===================" + newline;
            Zones.ToList().ForEach(zone =>
            {
                summary += "Zone: " + zone.Name + newline;
                summary += "Program: ";

                if (zone.ZoneProgram != null)
                {
                    summary += zone.ZoneProgram.Name + newline;

                    //TODO: Add current input values output
                    //if (zone.ZoneProgram is ParameterizedZoneProgram)
                    //{
                    //	var parameterDictionary = ((ParameterizedZoneProgram)zone.ZoneProgram).ProgramParameter.ToKeyValueDictionary();

                    //	parameterDictionary.Keys.ToList().ForEach(key =>
                    //	{
                    //		summary += string.Format("    {0}: {1}", key, parameterDictionary[key].ToString()) + newline;
                    //	});
                    //}
                }
                else
                {
                    summary += "None" + newline;
                }
                summary += "-------------------" + newline;

            });

            return summary;
        }

        /// <summary>
        /// Creates a ProgramSet
        /// </summary>
        /// <param name="programSetName">Name of program set</param>
        /// <param name="programName">Name of program</param>
        /// <param name="sync"></param>
        /// <param name="isv"></param>
        /// <param name="zones"></param>
        public ProgramSet CreateProgramSet(string programSetName, string programName, bool sync, ISV isv, IEnumerable<Zone> zones)
        {
            var zonesList = zones as IList<Zone> ?? zones.ToList();
            if (zonesList.Any(z => !AvailableZones.Contains(z))) throw new Exception("Some of the provided zones are not available.");

            var programSet = new ProgramSet(programName, zonesList, sync, null, programSetName);
            ProgramSets.Add(programSet);
            return programSet;
        }

        /// <summary>
        /// Adds a fade candy zone to the manager.
        /// </summary>
        /// <param name="name">Name of the zone</param>
        /// <param name="pixelType">Type of pixel for the zone</param>
        /// <param name="numberOfLights">Number of lights in the zone</param>
        /// <param name="channel">FadeCandy channel on which this zone is connected</param>
        /// <returns></returns>
        public Zone AddFadeCandyZone(string name, PixelType pixelType, int numberOfLights, int channel)
        {
            return ZoneScaffolder.Instance.AddFadeCandyZone(Zones, name, pixelType, numberOfLights, channel);
        }





        //TODO: Leaving this to finish at a later point in time. 
        //TODO: Possible move this and its test to a different branch, and it can be merged back later when 
        //TODO: it's ready to be worked on.
        //      /// <summary>
        //      /// Moves a zone from its current program set to another given program set.
        //      /// </summary>
        //public void MoveZone(Zone zone, ProgramSet targetProgramSet)
        //{
        //	if (targetProgramSet.ContainsZone(zone))
        //		throw new Exception("Cannot move zone to the same program set as where it is currently.");

        //	if (ProgramSets.Any(ps => ps.ContainsZone(zone)))
        //	{
        //		var sourceProgramSet = ProgramSets.First(ps => ps.ContainsZone(zone));
        //		sourceProgramSet.RemoveZone(zone);
        //		//TODO: Remove this
        //		Thread.Sleep(3000);

        //		targetProgramSet.AddZone(zone);
        //	}
        //	else
        //	{
        //		targetProgramSet.AddZone(zone);
        //	}
        //}

        #endregion


        #region Singleton

        private static ZLM _instance;

        /// <summary>
        /// I is short for Instance.
        /// </summary>
        public static ZLM I => _instance ?? (_instance = new ZLM());

        #endregion

        #region CORE

        /// <summary>
        /// All zones that can be managed by this class.
        /// </summary>
        [ImportMany(typeof(Zone), AllowRecomposition = true)]
        public BetterList<Zone> Zones { get; private set; } = new BetterList<Zone>();

        /// <summary>
        /// Returns the zones that are not being used by any program sets.
        /// </summary>
        public BetterList<Zone> AvailableZones
            => Zones.Where(z => !ProgramSets.Any(p => p.Zones.Contains(z))).ToBetterList();

        /// <summary>
        /// Container for the external modules.
        /// </summary>
        private CompositionContainer ExternalZoneContainer { get; set; }

        /// <summary>
        /// List of ProgramSets that are being managed by this class.
        /// </summary>
        public BetterList<ProgramSet> ProgramSets { get; } = new BetterList<ProgramSet>();

        #endregion

        #region C+I

        public ZLM()
        {
        }

        public bool Initialized { get; private set; }

        public void Initialize(bool loadZoneModules = false, bool loadZonesFromConfig = true, bool loadProgramSetsFromConfig = true, Action initAction = null)
        {
            if (!Initialized)
            {
                InitLightingControllers();
                InitZoneScaffolder();
                if (loadZoneModules && ExternalZoneContainer != null)
                    ComposeWithExternalModules();
                if (loadZonesFromConfig)
                    LoadZonesFromConfig();
                if (loadProgramSetsFromConfig)
                    LoadProgramSetsFromConfig();
                initAction?.Invoke();
                Initialized = true;
            }
        }


        /// <summary>
        /// Add code here to initialize any other lighting controllers.
        /// </summary>
        private void InitLightingControllers()
        {
            FadeCandyController.Instance.Initialize();
        }

        /// <summary>
        /// Add code here to uninitialize any other lighting controllers.
        /// </summary>
        private void UninitLightingControllers()
        {
            FadeCandyController.Instance.Uninitialize();
        }

        /// <summary>
        /// Initializes the ZoneScaffolder singleton instance by feeding the factories into it.
        /// </summary>
        private void InitZoneScaffolder()
        {
            ZoneScaffolder.Instance.Initialize(ConfigurationManager.AppSettings["ProgramDLLFolder"]);
        }

        /// <summary>
        /// Uninitializes the ZoneScaffolder singleton instance.
        /// </summary>
        private void UninitZoneScaffolder()
        {
            ZoneScaffolder.Instance.Uninitialize();
        }

        #region MEF

        /// <summary>
        /// Composes this class with external zones and programs. 
        /// This method populates the Zones and their respective ZoneProgram properties.
        /// </summary>
        private void ComposeWithExternalModules()
        {
            List<ComposablePartCatalog> fileCatalogs = new List<ComposablePartCatalog>();
            foreach (var file in Directory.GetFiles(ConfigurationManager.AppSettings["ZoneDLLFolder"], "*.dll").ToList())
            {
                var assembly = Assembly.LoadFrom(file);

                if (assembly.GetCustomAttributesData()
                    .Any(ass => ass.AttributeType == typeof(ZoneAssemblyAttribute)))
                {
                    fileCatalogs.Add(new AssemblyCatalog(assembly));
                    //this may be required to be uncommented in the future
                    File.Copy(file, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(file)), true);
                }
            }

            var aggregateCatalog = new AggregateCatalog(fileCatalogs);
            ExternalZoneContainer = new CompositionContainer(aggregateCatalog);
            ExternalZoneContainer.ComposeParts(this);
        }

        //TODO: This is not working because MEF has to be "hacked" to have the 
        //TODO: assemblies load in a different appdomain with ShadowCopyFiles set to true as in the following example:
        //TODO: http://www.codeproject.com/Articles/633140/MEF-and-AppDomain-Remove-Assemblies-On-The-Fly
        //public void RefreshExternalPrograms()
        //{
        //	ExternalProgramCatalog.Refresh();
        //	ExternalProgramContainer.ComposeParts(this);
        //}

        #endregion

        private void LoadZonesFromConfig()
        {
            Zones.AddRange(Config.DeserializeZones(File.ReadAllText(GetConfig("ZoneConfigurationSaveFile",
                    "Zone configuration save file not found."))));
        }

        private void LoadProgramSetsFromConfig()
        {
            ProgramSets.AddRange(Config.DeserializeProgramSets(File.ReadAllText(GetConfig("ProgramSetConfigurationSaveFile",
                    "Program Set configuration save file not found.")), Zones));
        }

        public void SaveProgramSets()
        {
			Config.SaveProgramSets(ProgramSets, GetConfig("ProgramSetConfigurationSaveFile"));
        }

        public void SaveZones()
        {
	        Config.SaveZones(Zones, GetConfig("ZoneConfigurationSaveFile"));
        }

        private string GetConfig(string settingName, string exceptionMessage = null)
        {
            var configValue = ConfigurationManager.AppSettings[settingName];
			if (exceptionMessage != null && string.IsNullOrEmpty(configValue))
             throw new Exception(exceptionMessage);
	        return configValue;
        }

        public void Uninitialize()
        {
            if (Initialized)
            {
                UninitializeAllZones();

                UninitLightingControllers();
                Initialized = false;
            }
        }

        /// <summary>
        /// Unintializes all zones.
        /// </summary>
        public void UninitializeAllZones()
        {
            Parallel.ForEach(Zones, zone =>
            {
                zone.Uninitialize();
            });

            //todo: remove this later
            Zones.ToList().ForEach(zone => zone.Dispose());
            Zones.Clear();
        }

        public void Dispose()
        {
            Uninitialize();
            UninitZoneScaffolder();
            ExternalZoneContainer?.Dispose();
            ExternalZoneContainer = null;
            Zones.Clear();
            Zones = null;
        }

        #endregion
    }
}
