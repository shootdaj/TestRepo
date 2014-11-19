using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZoneLighting.Communication;
using ZoneLighting.ZoneProgram;
using ZoneLighting.ZoneProgram.Programs;

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
		/// Container for the external programs.
		/// </summary>
		private CompositionContainer ExternalProgramContainer { get; set; }

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
				LoadSampleZoneData();	//TODO: Replace
				LoadExternalPrograms();
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
		/// Loads external programs using MEF.
		/// </summary>
		private void LoadExternalPrograms()
		{
			ExternalProgramCatalog = new DirectoryCatalog(ConfigurationManager.AppSettings["ProgramDLLFolder"]);
			ExternalProgramContainer = new CompositionContainer(ExternalProgramCatalog);
			ExternalProgramContainer.ComposeParts(this);
		}

		/// <summary>
		/// Loads programs in all zones and starts them. This should be converted to be read from a config file instead of hard-coded here.
		/// </summary>
		private void InitializeAllZones()
		{
			//Zones[0].Initialize(ZoneProgramFactories.First(x => x.Metadata.Name == "Rainbow").CreateExport().Value,
			//	ZoneProgramParameterFactories.First(x => x.Metadata.Name == "RainbowParameter").CreateExport().Value);

			var parameterDictionary = new Dictionary<string, object>();
			parameterDictionary.Add("Speed", 1);
			parameterDictionary.Add("DelayTime", 1);

			InitializeZone(Zones[0], "Rainbow", parameterDictionary);


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
			zone.Initialize(ZoneProgramFactories.First(x => x.Metadata.Name == programName).CreateExport().Value, parameter);
		}

		/// <summary>
		/// Initializes a zone with the given program name and parameter name-value dictionary.
		/// </summary>
		public void InitializeZone(Zone zone, string programName, Dictionary<string, object> parameterDictionary)
		{
			var parameter = CreateProgramParameter(programName, parameterDictionary);
			zone.Initialize(ZoneProgramFactories.First(x => x.Metadata.Name == programName).CreateExport().Value, parameter);
		}

		/// <summary>
		/// Creates a program parameter using Reflection, given the name of the program and a dictionary of property names to property values
		/// </summary>
		public IZoneProgramParameter CreateProgramParameter(string programName, Dictionary<string, object> parameterDictionary)
		{
			var zoneProgramFactory = ZoneProgramFactories.First(x => x.Metadata.Name == programName);
			var programParameter =
				ZoneProgramParameterFactories.First(x => x.Metadata.Name == zoneProgramFactory.Metadata.ParameterName)
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

		public void LoadSampleZoneData()
		{
			var leftWingZone = AddFadeCandyLEDStripZone("LeftWing", 6, 1);
			var rightWingZone = AddFadeCandyLEDStripZone("RightWing", 12, 2);
		}

		private Zone AddFadeCandyLEDStripZone(string name, int numLights, byte fcChannel)
		{
			var zone = new Zone(FadeCandyController.Instance, name);
			Zones.Add(zone);
			for (int i = 0; i < numLights; i++)
			{
				zone.AddLight(new LED(logicalIndex: i, fadeCandyChannel: fcChannel, fadeCandyIndex: i));
			}

			return zone;
		}

		#endregion

	}

}
