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

		private void AddBasementZonesAndPrograms()
		{
			var leftWing = new FadeCandyZone("LeftWing");
			leftWing.AddFadeCandyLights(12, 1);
			Zones.Add(leftWing);

			var rightWing = new FadeCandyZone("RightWing");
			rightWing.AddFadeCandyLights(12, 2);
			Zones.Add(rightWing);

			var center = new FadeCandyZone("Center");
			center.AddFadeCandyLights(21, 3);
			Zones.Add(center);

			var leftWingStartingValues = new InputStartingValues();
			leftWingStartingValues.Add("DelayTime", 30);
			leftWingStartingValues.Add("DotColor", (Color?)Color.Red);
			ZoneScaffolder.Instance.InitializeZone(leftWing, "ScrollDot", leftWingStartingValues);

			var rightWingStartingValues = new InputStartingValues();
			rightWingStartingValues.Add("DelayTime", 1);
			rightWingStartingValues.Add("Speed", 1);
			ZoneScaffolder.Instance.InitializeZone(rightWing, "Rainbow", rightWingStartingValues);

			var centerStartingValues = new InputStartingValues();
			centerStartingValues.Add("DelayTime", 1);
			centerStartingValues.Add("Speed", 1);
			ZoneScaffolder.Instance.InitializeZone(center, "Rainbow", centerStartingValues);

			ZoneScaffolder.Instance.StartInterruptingProgram(center, "BlinkColor");

			//TODO: Add an interrupting program that will notify for something.
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
