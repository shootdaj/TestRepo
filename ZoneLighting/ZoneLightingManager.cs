using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json.Serialization;
using ZoneLighting.Communication;
using ZoneLighting.ConfigNS;
using ZoneLighting.TEMP;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLighting
{
	/// <summary>
	/// This class will be responsible for managing the higher level tasks for the zones.
	/// </summary>
	public class ZoneLightingManager : IDisposable
	{
		#region Singleton

		private static ZoneLightingManager _instance;

		public static ZoneLightingManager Instance => _instance ?? (_instance = new ZoneLightingManager());

		#endregion

		#region CORE

		/// <summary>
		/// All zones that can be managed by this class.
		/// </summary>
		[ImportMany(typeof(Zone), AllowRecomposition = true)]
		public IList<Zone> Zones { get; set; } = new List<Zone>();

		/// <summary>
		/// Container for the external modules.
		/// </summary>
		private CompositionContainer ExternalZoneContainer { get; set; }

		/// <summary>
		/// Synchronization contexts (clocks) for synchronizing the programs running in different zones.
		/// </summary>
		private IList<SyncContext> SyncContexts { get; set; } = new List<SyncContext>();

		#endregion

		#region C+I

		public ZoneLightingManager()
		{
		}

		public bool Initialized { get; private set; }

		public void Initialize(bool loadExternalZones = true)
		{
			if (!Initialized)
			{
				InitLightingControllers();
				if (loadExternalZones)
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
		private void InitializeAllZones()
		{
			//var configFilePath = ConfigurationManager.AppSettings["ZoneConfigurationSaveFile"];

			//if (!string.IsNullOrEmpty(configFilePath))
			//{
			//	ZoneScaffolder.Instance.InitializeFromZoneConfiguration(Zones, configFilePath);
			//}

			AddBasementZonesAndPrograms();
			
			





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
				UninitZoneScaffolder();
				UninitLightingControllers();
				Initialized = false;
				ExternalZoneContainer = null;
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
		}

		#endregion

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
		/// This method synchronizes the program running on one zone to the program running in another zone
		/// </summary>
		/// <param name="syncSource"></param>
		/// <param name="syncTarget"></param>
		public void Synchronize(Zone syncSource, Zone syncTarget)
		{
			if (!(syncTarget.ZoneProgram is LoopingZoneProgram) || !(syncSource.ZoneProgram is LoopingZoneProgram))
				throw new Exception("Both zones passed in must have a looping zone program running on them.");

			var sourceProgram = (LoopingZoneProgram) syncSource.ZoneProgram;
			var targetProgram = (LoopingZoneProgram)syncTarget.ZoneProgram;

			//request and wait for synchronizable state of source and target programs
			sourceProgram.RequestSyncState();
			sourceProgram.IsSynchronizable.WaitForFire();
			targetProgram.RequestSyncState();
			targetProgram.IsSynchronizable.WaitForFire();

			//start synchronization

			//remove target from all sync contexts
			if (IsInAnySyncContext(syncTarget))
			{
				RemoveFromAllSyncContexts(syncTarget);
			}

			//if there are any contexts in which the source is, use it.
			if (SyncContexts.Any(c => c.Zones.Contains(syncSource)))
			{
				SyncContexts.First(c => c.Zones.Contains(syncSource))
					.AddZone(syncTarget);
			}
			//else create a new context and use that.
			else
			{
				var syncContext = new SyncContext(syncSource, syncSource.Name + "SyncContext");
				syncContext.AddZone(syncTarget);
				SyncContexts.Add(syncContext);
			}

			//end synchronization

			//resume the programs that were waiting after they are synced
			sourceProgram.WaitForSync.Fire(this, null);
			targetProgram.WaitForSync.Fire(this, null);
		}

		public void Unsynchronize(Zone unSyncTarget)
		{
			//remove target from all sync contexts
			if (IsInAnySyncContext(unSyncTarget))
			{
				RemoveFromAllSyncContexts(unSyncTarget);
			}
		}

		#endregion



		private void AddBasementZonesAndPrograms()
		{

			//var syncContext = new SyncContext();
			var notificationSyncContext = new SyncContext();

			var leftWing = AddFadeCandyZone("LeftWing", new Rainbow(), PixelType.FadeCandyWS2812Pixel, 6, 1);//, syncContext);
			//AddInterruptingProgramToZone(leftWing, "BlinkColor");//, notificationSyncContext.Barrier);
			var center = AddFadeCandyZone("Center", new Rainbow(), PixelType.FadeCandyWS2811Pixel, 21, 2);//, syncContext);
																									  //AddInterruptingProgramToZone(center, "BlinkColor");//, notificationSyncContext.Barrier);
			var rightWing = AddFadeCandyZone("RightWing", new Rainbow(), PixelType.FadeCandyWS2812Pixel, 12, 3);//, syncContext);
																											//AddInterruptingProgramToZone(rightWing, "BlinkColor");//, notificationSyncContext.Barrier);
			//var baiClock = AddFadeCandyZone("BaiClock", "Rainbow", PixelType.FadeCandyWS2812Pixel, 24, 4);//, syncContext);
																										  //AddInterruptingProgramToZone(baiClock, "BlinkColor");//, notificationSyncContext.Barrier);


			leftWing.Synchronize(center).Synchronize(rightWing);//.Synchronize(baiClock);

			//var rainbowContext = new SyncContext((LoopingZoneProgram)leftWing.ZoneProgram, "RainbowContext");
			//rainbowContext.AddZoneProgram((LoopingZoneProgram)rightWing.ZoneProgram);
			//rainbowContext.AddZoneProgram((LoopingZoneProgram)center.ZoneProgram);
			//rainbowContext.AddZoneProgram((LoopingZoneProgram)baiClock.ZoneProgram);





			//1. Create Zones and Barriers


			//var baiClock = new FadeCandyZone("BaiClock", brightness: 0.5);
			//baiClock.AddFadeCandyLights(PixelType.FadeCandyWS2812Pixel, 24, 4);
			//Zones.Add(baiClock);

			////2. 

			//var rainbowBarrier = new Barrier(0);

			////2. Start Programs

			//var centerStartingValues = new InputStartingValues { { "DelayTime", 1 }, { "Speed", 1 } };
			//ZoneScaffolder.Instance.InitializeZone(center, "Rainbow", centerStartingValues, rainbowBarrier);
			//ZoneScaffolder.Instance.InitializeZone(rightWing, "Rainbow", barrier: rainbowBarrier);
			//ZoneScaffolder.Instance.InitializeZone(baiClock, "Rainbow", barrier: rainbowBarrier);


			////3. Add Interrupting Programs
			//ZoneScaffolder.Instance.StartInterruptingProgram(leftWing, "BlinkColor");
			//ZoneScaffolder.Instance.StartInterruptingProgram(center, "BlinkColor");
			//ZoneScaffolder.Instance.StartInterruptingProgram(rightWing, "BlinkColor");
			//ZoneScaffolder.Instance.StartInterruptingProgram(baiClock, "BlinkColor");

			//TODO: Add an interrupting program that will notify for something.
		}



		public FadeCandyZone AddFadeCandyZone(string name, string programName, PixelType pixelType, int numLights, byte channel, SyncContext syncContext = null)
		{
			var zone = new FadeCandyZone(name);
			zone.AddFadeCandyLights(pixelType, numLights, channel);
			Zones.Add(zone);
			ZoneScaffolder.Instance.InitializeZone(zone, programName, barrier: syncContext?.Barrier);

			return zone;
		}

		public FadeCandyZone AddFadeCandyZone(string name, ZoneProgram program ,PixelType pixelType, int numLights, byte channel, SyncContext syncContext = null)
		{
			var zone = new FadeCandyZone(name);
			zone.AddFadeCandyLights(pixelType, numLights, channel);
			Zones.Add(zone);
			ZoneScaffolder.Instance.InitializeZone(zone, program, barrier: syncContext?.Barrier);

			return zone;
		}

		public void AddInterruptingProgramToZone(Zone zone, string interruptingProgramName, Barrier barrier = null)
		{
			ZoneScaffolder.Instance.StartInterruptingProgram(zone, interruptingProgramName, barrier: barrier);
		}


		#region Helpers

		

		private void RemoveFromAllSyncContexts(Zone zone)
		{
			SyncContexts.ToList().ForEach(c =>
			{
				if (c.Zones.Contains(zone))
					c.RemoveZone(zone);
			});
		}

		private bool IsInAnySyncContext(Zone zone)
		{
			return SyncContexts.Any(context => context.Zones.Contains(zone));
		}

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
