using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using ZoneLighting.Communication;
using ZoneLighting.ConfigNS;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLighting
{
	/// <summary>
	/// This class will be responsible for managing the higher level tasks for the zones.
	/// </summary>
	public class ZoneLightingManager : IInitializable, IDisposable
	{
		#region Singleton

		public static ZoneLightingManager _instance;

		public static ZoneLightingManager Instance
		{
			get { return _instance ?? (_instance = new ZoneLightingManager()); }
		}

		#endregion

		#region CORE

		/// <summary>
		/// All zones that can be managed by this class.
		/// </summary>
		[ImportMany(typeof(Zone), AllowRecomposition = true)] //TODO: Do composition at runtime rather than using attributes.
		public IList<Zone> Zones { get; set; }
		
		/// <summary>
		///	This factory member will provide the various implementations of zone programs that are to be loaded from external modules.
		/// </summary>
		[ImportMany(typeof(ZoneProgram), AllowRecomposition = true)]
		private IEnumerable<ExportFactory<ZoneProgram, IZoneProgramMetadata>> ZoneProgramFactories { get; set; }

		/// <summary>
		/// This factory member will provide the various implementations of zone program parameters that are to be loaded from external modules.
		/// </summary>
		[ImportMany(typeof(ZoneProgramParameter), AllowRecomposition = true)]
		private IEnumerable<ExportFactory<ZoneProgramParameter, IZoneProgramParameterMetadata>> ZoneProgramParameterFactories
		{ get; set; }

		/// <summary>
		/// Directory Catalog that stores catalog for the external programs
		/// </summary>
		private DirectoryCatalog ExternalProgramCatalog { get; set; }
		
		/// <summary>
		/// Directory Catalog that stores catalog for the external zones.
		/// </summary>
		private DirectoryCatalog ExternalZoneCatalog { get; set; }

		/// <summary>
		/// Container for the external modules.
		/// </summary>
		private CompositionContainer ExternalModuleContainer { get; set; }

		#endregion

		#region C+I

		public ZoneLightingManager()
		{
			Zones = new List<Zone>();
		}

		public bool Initialized { get; private set; }

		public void Initialize()
		{
			if (!Initialized)
			{
				InitLightingControllers();
				ComposeWithExternalModules();
				InitZoneScaffolder();
				InitializeAllZones();
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
			ZoneScaffolder.Initialize(ZoneProgramFactories, ZoneProgramParameterFactories);
		}

		/// <summary>
		/// Uninitializes the ZoneScaffolder singleton instance.
		/// </summary>
		private void UninitZoneScaffolder()
		{
			ZoneScaffolder.Uninitialize();
		}

		#region MEF
		
		/// <summary>
		/// Composes this class with external zones and programs. 
		/// This method populates the Zones and their respective ZoneProgram properties.
		/// </summary>
		private void ComposeWithExternalModules(bool programModules = true, bool zoneModules = true)
		{
			//AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles = "true";
			if (!programModules && !zoneModules)
				return;

			if (zoneModules)
				CatalogExternalZones();
			if (programModules)
				CatalogExternalPrograms();

			AggregateCatalog aggregateCatalog;
			
			if (programModules && zoneModules)
			{
				aggregateCatalog = new AggregateCatalog(ExternalZoneCatalog, ExternalProgramCatalog);
			}
			else if (programModules)
			{
				aggregateCatalog = new AggregateCatalog(ExternalProgramCatalog);
			}
			else
			{
				aggregateCatalog = new AggregateCatalog(ExternalZoneCatalog);	
			}

			ExternalModuleContainer = new CompositionContainer(aggregateCatalog);
			ExternalModuleContainer.ComposeParts(this);
		}

		/// <summary>
		/// Creates catalog for external zones.
		/// </summary>
		private void CatalogExternalZones()
		{
			ExternalZoneCatalog = new DirectoryCatalog(ConfigurationManager.AppSettings["ZoneDLLFolder"]);
		}

		/// <summary>
		/// Creates catalog for external programs.
		/// </summary>
		private void CatalogExternalPrograms()
		{
			ExternalProgramCatalog = new DirectoryCatalog(ConfigurationManager.AppSettings["ProgramDLLFolder"]);
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

		/// <summary>
		/// Loads programs in all zones and starts them. This should be converted to be read from a config file instead of hard-coded here.
		/// </summary>
		private void InitializeAllZones()
		{
			//var scrollDotDictionary = new Dictionary<string, object>();
			//scrollDotDictionary.Add("DelayTime", 30);
			//scrollDotDictionary.Add("Color", (Color?)Color.Chartreuse);

			//ZoneScaffolder.InitializeZone(Zones[0], "ScrollDot", scrollDotDictionary);

			//var rainbowDictionary = new Dictionary<string, object>();
			//rainbowDictionary.Add("DelayTime", 1);
			//rainbowDictionary.Add("Speed", 1);

			//ZoneScaffolder.InitializeZone(Zones[1], "Rainbow", rainbowDictionary);

			//Config.SaveZones(Zones, ConfigurationManager.AppSettings["ZoneConfigurationSaveFile"]);

			ZoneScaffolder.InitializeFromZoneConfiguration(Zones.ToList());
		}

		public void Uninitialize()
		{     
			if (Initialized)
			{
				UninitializeAllZones();
				UninitZoneScaffolder();
				UninitLightingControllers();
				Initialized = false;
				ExternalProgramCatalog = null;
				ExternalZoneCatalog = null;
				ExternalModuleContainer = null;
			}
		}

		/// <summary>
		/// Unintializes all zones.
		/// </summary>
		public void UninitializeAllZones()
		{
			Zones.ToList().ForEach(z => z.Uninitialize());
		}

		public void Dispose()
		{
			Uninitialize();
			Zones.Clear();
			Zones = null;
			ZoneProgramFactories = null;
			ZoneProgramParameterFactories = null;
		}

		#endregion

		#region API

		public string GetZoneSummary()
		{
			var newline = Environment.NewLine;
			string summary = string.Format("Currently {0} zones are loaded." + newline, Zones.Count);
			Zones.ToList().ForEach(zone =>
			{
				summary += "Zone: " + zone.Name + newline;
				summary += "Program: ";

				if (zone.ZoneProgram != null)
				{
					summary += zone.ZoneProgram.Name + newline;

					if (zone.ZoneProgram is ParameterizedZoneProgram)
					{
						var parameterDictionary = ((ParameterizedZoneProgram)zone.ZoneProgram).ProgramParameter.ToKeyValueDictionary();

						parameterDictionary.Keys.ToList().ForEach(key =>
						{
							summary += string.Format("    {0}: {1}", key, parameterDictionary[key].ToString()) + newline;
						});
					}
				}
				else
				{
					summary += "None" + newline;
				}

			});

			return summary;
		}

		#endregion

		#region Helpers

		///// <summary>
		///// Creates a zone program with the given name
		///// </summary>
		///// <returns></returns>
		//private IZoneProgram CreateZoneProgram(string name)
		//{
			
		//}

		#endregion
	}
}
