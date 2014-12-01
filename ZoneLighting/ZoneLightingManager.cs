using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Reflection;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgram;

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
		[ImportMany(typeof(IZoneProgram), AllowRecomposition = true)]
		private IEnumerable<ExportFactory<IZoneProgram, IZoneProgramMetadata>> ZoneProgramFactories { get; set; }

		/// <summary>
		/// This factory member will provide the various implementations of zone program parameters that are to be loaded from external modules.
		/// </summary>
		[ImportMany(typeof (IZoneProgramParameter), AllowRecomposition = true)]
		private IEnumerable<ExportFactory<IZoneProgramParameter, IZoneProgramParameterMetadata>> ZoneProgramParameterFactories
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
				//LoadSampleZoneData();	//TODO: Replace
				ComposeWithExternalModules();
				InitializeAllZones();
				Initialized = true;
			}
		}
		
		/// <summary>
		/// Add code here to initialize any other lighting controllers
		/// </summary>
		private void InitLightingControllers()
		{
			FadeCandyController.Instance.Initialize();
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

		/// <summary>
		/// Composes this class with external zones and programs.
		/// </summary>
		private void ComposeWithExternalModules()
		{
			//AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles = "true";
			CatalogExternalZones();
			CatalogExternalPrograms();
			AggregateCatalog aggregateCatalog = new AggregateCatalog(ExternalZoneCatalog, ExternalProgramCatalog);
			ExternalModuleContainer = new CompositionContainer(aggregateCatalog);
			ExternalModuleContainer.ComposeParts(this);
		}

		//TODO: This is not working because MEF has to be "hacked" to have the 
		//TODO: assemblies load in a different appdomain with ShadowCopyFiles set to true as in the following example:
		//TODO: http://www.codeproject.com/Articles/633140/MEF-and-AppDomain-Remove-Assemblies-On-The-Fly
		//public void RefreshExternalPrograms()
		//{
		//	ExternalProgramCatalog.Refresh();
		//	ExternalProgramContainer.ComposeParts(this);
		//}

		/// <summary>
		/// Loads programs in all zones and starts them. This should be converted to be read from a config file instead of hard-coded here.
		/// </summary>
		private void InitializeAllZones()
		{
			//Zones[0].Initialize(ZoneProgramFactories.First(x => x.Metadata.Name == "Rainbow").CreateExport().Value,
			//	ZoneProgramParameterFactories.First(x => x.Metadata.Name == "RainbowParameter").CreateExport().Value);

			//var rainbowParameterDictionary = new Dictionary<string, object>();
			//rainbowParameterDictionary.Add("Speed", 100);
			//rainbowParameterDictionary.Add("DelayTime", 0);

			//InitializeZone(Zones[0], "Rainbow", rainbowParameterDictionary);

			var scrollDotDictionary = new Dictionary<string, object>();
			scrollDotDictionary.Add("DelayTime", 30);
			//scrollDotDictionary.Add("Color", (Color?)Color.Chartreuse);

			InitializeZone(Zones[0], "ScrollDot", scrollDotDictionary);


			//Zones[0].Initialize(new Rainbow(), new RainbowParameter(1, 1));
			//Zones[1].Initialize(new ScrollDot(), new ScrollDotParameter(50, Color.BlueViolet));
		}

		public void Uninitialize()
		{     
			if (Initialized)
			{
				UninitializeAllZones();
				Zones.Clear();
				Initialized = false;
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
			Zones = null;
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

		#region API

		/// <summary>
		/// Initializes a zone with the given program name and parameter.
		/// </summary>
		public void InitializeZone(Zone zone, string programName, IZoneProgramParameter parameter)
		{
			zone.Initialize(ZoneProgramFactories.ToDictionary(x => x.Metadata.Name)[programName].CreateExport().Value, parameter);
		}

		/// <summary>
		/// Initializes a zone with the given program name and parameter name-value dictionary.
		/// </summary>
		public void InitializeZone(Zone zone, string programName, Dictionary<string, object> parameterDictionary)
		{
			var parameter = CreateProgramParameter(programName, parameterDictionary);
			zone.Initialize(ZoneProgramFactories.ToDictionary(x => x.Metadata.Name)[programName].CreateExport().Value, parameter);
		}

		/// <summary>
		/// Gets the names of all available programs.
		/// </summary>
		public IEnumerable<string> AvailableProgramNames
		{
			get { return ZoneProgramFactories.Select(x => x.Metadata.Name); }
		}

		/// <summary>
		/// Creates a program parameter using Reflection, given the name of the program and a dictionary of property names to property values
		/// </summary>
		public IZoneProgramParameter CreateProgramParameter(string programName, Dictionary<string, object> parameterDictionary)
		{
			var zoneProgramFactory = ZoneProgramFactories.ToDictionary(x => x.Metadata.Name)[programName];//.First(x => x.Metadata.Name == programName);
			var programParameter =
				ZoneProgramParameterFactories.ToDictionary(x => x.Metadata.Name)[zoneProgramFactory.Metadata.ParameterName]
					.CreateExport();
			
			parameterDictionary.Keys.ToList().ForEach(propertyName =>
			{
				var property = programParameter.Value.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
				if (property != null)
				{
					property.SetValue(programParameter.Value, parameterDictionary[propertyName]);
				}
			});

			return programParameter.Value;
		}

		#endregion

		#region Sample Data

		//public void LoadSampleZoneData()
		//{
		//	var leftWingZone = AddFadeCandyLEDStripZone("LeftWing", 6, 1);
		//	var rightWingZone = AddFadeCandyLEDStripZone("RightWing", 12, 2);
		//}

		//private Zone AddFadeCandyLEDStripZone(string name, int numLights, byte fcChannel)
		//{
		//	var zone = new Zone(FadeCandyController.Instance, name);
		//	Zones.Add(zone);
		//	for (int i = 0; i < numLights; i++)
		//	{
		//		zone.AddLight(new LED(logicalIndex: i, fadeCandyChannel: fcChannel, fadeCandyIndex: i));
		//	}

		//	return zone;
		//}

		#endregion

	}

}
