using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ZoneLighting.Communication;
using ZoneLighting.StockPrograms;
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

		public void MoveZone(Zone zone, ProgramSet targetProgramSet)
		{
			if (targetProgramSet.ContainsZone(zone))
				throw new Exception("Cannot move zone to the same program set as where it is currently.");

			if (ProgramSets.Any(ps => ps.ContainsZone(zone)))
			{
				var sourceProgramSet = ProgramSets.First(ps => ps.ContainsZone(zone));
				sourceProgramSet.RemoveZone(zone);
				//TODO: Remove this
				Thread.Sleep(3000);

				targetProgramSet.AddZone(zone);
			}
			else
			{
				targetProgramSet.AddZone(zone);
			}
		}

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

		[Alias()]
		public void Initialize(bool loadExternalZones = false, Action testAction = null)
		{
			if (!Initialized)
			{
				InitLightingControllers();
				if (loadExternalZones && ExternalZoneContainer != null)
                    ComposeWithExternalModules();
				InitZoneScaffolder();
				InitializeAllZones(testAction);
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

		/// <summary>
		/// Loads programs in all zones and starts them. This should be converted to be read from a config file instead of hard-coded here.
		/// </summary>
		private void InitializeAllZones(Action testAction)
		{
			//var configFilePath = ConfigurationManager.AppSettings["ZoneConfigurationSaveFile"];

			//if (!string.IsNullOrEmpty(configFilePath))
			//{
			//	ZoneScaffolder.Instance.InitializeFromZoneConfiguration(Zones, configFilePath);
			//}

			//AddBasementZonesAndPrograms();
			testAction?.Invoke();
			//AddBasementZonesAndProgramsWithSync();






			//Config.SaveZones(Zones, ConfigurationManager.AppSettings["ZoneConfigurationSaveFile"]);


			//InputStartingValues startingValues = //null;
			//	new InputStartingValues();
			//startingValues.Add("Color", Color.Red);

			//ZoneScaffolder.Instance.InitializeZone(Zones[0], "StaticColor", startingValues);



			//Config.SaveZones(Zones, ConfigurationManager.AppSettings["ZoneConfigurationSaveFile"]);

			//Zones.ToList().ForEach(z => z.Uninitialize(true));

			//ZoneScaffolder.Instance.InitializeFromZoneConfiguration(Zones);
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

		

		private void AddBasementZonesAndPrograms()
		{

			//var notificationSyncContext = new SyncContext();

			////add zones
			//var adsf = ZoneScaffolder.Instance.AddFadeCandyZone()

			//var leftWing =  AddFadeCandyZone("LeftWing", PixelType.FadeCandyWS2812Pixel, 6, 1);
			//var center = AddFadeCandyZone("Center", PixelType.FadeCandyWS2811Pixel, 21, 2);
			//var rightWing = AddFadeCandyZone("RightWing", PixelType.FadeCandyWS2812Pixel, 12, 3);
			//var baiClock = AddFadeCandyZone("BaiClock", PixelType.FadeCandyWS2812Pixel, 24, 4);

			//var rainbowSet = new ProgramSet("Rainbow",)

			////initialize zones
			//leftWing.Initialize(new Cylon(), null);//, true, syncContext, true);
			//center.Initialize(new Cylon(), null);//, true, syncContext, true);
			//rightWing.Initialize(new Cylon(), null);//, true, syncContext, true);
			//baiClock.Initialize(new Cylon(), null);//, true, syncContext, true);

			//////synchronize and start zone programs
			////syncContext.Sync(leftWing.ZoneProgram,
			////						center.ZoneProgram,
			////						rightWing.ZoneProgram,
			////						baiClock.ZoneProgram);

			////leftWing.ZoneProgram.Start();
			////rightWing.ZoneProgram.Start();
			////center.ZoneProgram.Start();
			////baiClock.ZoneProgram.Start();

			////setup interrupting inputs
			//leftWing.SetupInterruptingProgram(new BlinkColorReactive(), null);//, notificationSyncContext);
			//rightWing.SetupInterruptingProgram(new BlinkColorReactive(), null);//, notificationSyncContext);
			//center.SetupInterruptingProgram(new BlinkColorReactive(), null);//, notificationSyncContext);
			//baiClock.SetupInterruptingProgram(new BlinkColorReactive(), null);//, notificationSyncContext);

			////synchronize and start interrupting programs
			////notificationSyncContext.SyncAndStart(leftWing.InterruptingPrograms[0],
			////									rightWing.InterruptingPrograms[0],
			////									center.InterruptingPrograms[0],
			////									baiClock.InterruptingPrograms[0]);

			//leftWing.InterruptingPrograms[0].Start();
			//rightWing.InterruptingPrograms[0].Start();
			//center.InterruptingPrograms[0].Start();
			//baiClock.InterruptingPrograms[0].Start();
		}

		//private void AddBasementZonesAndProgramsWithSync()
		//{
		//	var notificationSyncContext = new SyncContext("NotificationContext");
			
		//	//add zones
		//	var leftWing = ZoneScaffolder.Instance.AddFadeCandyZone(Zones, "LeftWing", PixelType.FadeCandyWS2812Pixel, 6, 1);
		//	var center = ZoneScaffolder.Instance.AddFadeCandyZone(Zones, "Center", PixelType.FadeCandyWS2811Pixel, 21, 2);
		//	var rightWing = ZoneScaffolder.Instance.AddFadeCandyZone(Zones, "RightWing", PixelType.FadeCandyWS2812Pixel, 12, 3);
		//	var baiClock = ZoneScaffolder.Instance.AddFadeCandyZone(Zones, "BaiClock", PixelType.FadeCandyWS2812Pixel, 24, 4);

		//	CreateProgramSet("RainbowSet", "Rainbow", true, null, Zones);

		//	//setup interrupting inputs
		//	leftWing.SetupInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);
		//	rightWing.SetupInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);
		//	center.SetupInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);
		//	baiClock.SetupInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);

		//	//synchronize and start interrupting programs
		//	notificationSyncContext.Sync(leftWing.InterruptingPrograms[0],
		//										rightWing.InterruptingPrograms[0],
		//										center.InterruptingPrograms[0],
		//										baiClock.InterruptingPrograms[0]);

		//	leftWing.InterruptingPrograms[0].Start();
		//	rightWing.InterruptingPrograms[0].Start();
		//	center.InterruptingPrograms[0].Start();
		//	baiClock.InterruptingPrograms[0].Start();
		//}

		//#region Helpers

		
		//#endregion
	}
}
