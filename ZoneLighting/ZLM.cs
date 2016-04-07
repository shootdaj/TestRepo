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
using ZoneLighting.Communication;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;
using Config = ZoneLighting.ConfigNS.Config;

namespace ZoneLighting
{
	/// <summary>
	/// This class is responsible for managing the higher level tasks for zones and program sets.
	/// </summary>
	public sealed class ZLM : IDisposable, IZLM
	{
		#region API

		#region Admin

		public Zone AddFadeCandyZone(string name, PixelType pixelType, int numberOfLights, byte? channel)
		{
			return ZoneScaffolder.Instance.AddFadeCandyZone(Zones, name, pixelType, numberOfLights, channel);
		}

		public void Save()
		{
			SaveZones();
			SaveProgramSets();
		}

		#endregion

		#region Program Set

		public ProgramSet CreateProgramSet(string programSetName, string programName, IEnumerable<string> zoneNames, bool sync = true,
			ISV isv = null, dynamic startingParameters = null)
		{
			var zones = GetZonesByNames(zoneNames.ToList());
			return CreateProgramSet(programSetName, programName, sync, isv, zones, startingParameters);
		}

		/// <summary>
		/// Creates a ProgramSet
		/// </summary>
		/// <param name="programSetName">Name of program set</param>
		/// <param name="programName">Name of program</param>
		/// <param name="sync">Whether or not to start the programs in sync</param>
		/// <param name="isv">Input starting values - starting values for the inputs</param>
		/// <param name="zones">Zones to run the program set on</param>
		/// <param name="startingParameters">Starting parameters for creating this program set. These will be fed to the constructor(s) of the ZoneProgram(s).</param>
		public ProgramSet CreateProgramSet(string programSetName, string programName, bool sync, ISV isv,
			IEnumerable<Zone> zones, dynamic startingParameters = null)
		{
			var zonesList = zones as IList<Zone> ?? zones.ToList();
			if (zonesList.Any(z => !AvailableZones.Contains(z)))
				throw new Exception("Some of the provided zones are not available.");

			var programSet = new ProgramSet(programName, zonesList, sync, isv.Listify(), programSetName, startingParameters);
			ProgramSets.Add(programSet);
			return programSet;
		}

		/// <summary>
		/// Creates a ProgramSet with one program instance
		/// </summary>
		public ProgramSet CreateSingularProgramSet(string programSetName, ZoneProgram program, ISV isv, Zone zone)
		{
			if (!AvailableZones.Contains(zone)) throw new Exception("The provided zone is not available.");

			var programSet = new ProgramSet(program, zone, isv, programSetName);
			ProgramSets.Add(programSet);
			return programSet;
		}

		/// <summary>
		/// Disposes the given program set and creates another with the same name and the given parameters (name, zones and ISV).
		/// </summary>
		public void RecreateProgramSet(string programSetName, string programName, List<string> zoneNames, ISV isv)
		{
			var zones = GetZonesByNames(zoneNames);

			DisposeProgramSets(programSetName.Listify());
			CreateProgramSet(programSetName, programName, false, isv, zones);
		}

		/// <summary>
		/// Disposes the given program set(s). If no specific program sets are provided, disposes all program sets.
		/// </summary>
		public void DisposeProgramSets(List<string>programSetNames = null, bool force = false)
		{
			if (programSetNames == null || !programSetNames.Any())
			{
				ProgramSets?.ForEach(programSet => programSet?.Dispose(force));
				ProgramSets?.Clear();
			}
			else
			{
				var programSetsToDispose = ProgramSets?.Where(programSet => programSetNames.Contains(programSet.Name)).ToList();
				programSetsToDispose?.ForEach(programSet =>
				{
					ProgramSets?.Remove(programSet);
					programSet?.Dispose(force);
				});
			}
		}

		/// <summary>
		/// Sets inputs for a given program set. This will set inputs for all programs in the program set.
		/// </summary>
		public void SetProgramSetInputs(string programSetName, ISV isv)
		{
			ProgramSets[programSetName].SetInputs(isv);
		}

		/// <summary>
		/// Removes the given zone from the given program set and stops the zone.
		/// TODO: This is a very unelegant and stupid way of removing zones (basically disposing and recreating the program set without the zone)
		/// TODO: What needs to happen is Unsync, which is currently broken for 3 or more programs. This needs to be resolved eventually. 
		/// </summary>
		public void RecreateProgramSetWithoutZone(string programSetName, string zoneName, bool force = false)
		{
			var isv = ProgramSets[programSetName].Zones.First().ZoneProgram.GetInputValues();
			var programName = ProgramSets[programSetName].ProgramName;
			var zones = ProgramSets[programSetName].Zones.Select(zone => zone.Name).ToList();
			var sync = ProgramSets[programSetName].Sync;
			DisposeProgramSets(programSetName.Listify(), true);
			zones.Remove(zoneName);
			Zones[zoneName].ClearColors();
			//todo: get starting parameters to work - currently null is being passed in
			CreateProgramSet(programSetName, programName, zones, sync, isv, null);
		}

		#endregion

		#region Zone

		public void StopZone(string zoneName, bool force)
		{
			Zones.First(z => z.Name == zoneName).Stop(force);
		}

		public void SetZoneInputs(string zoneName, ISV isv)
		{
			Zones[zoneName].ZoneProgram.SetInputs(isv);
		}

		public void SetZoneColor(string zoneName, string color, float brightness)
		{
			Zones[zoneName].SetColor(Color.FromName(color).Darken(brightness));
			Zones[zoneName].SendLights(Zones[zoneName].LightingController);
		}

		public void SetLightColor(string zoneName, string color, int index, float brightness = 1)
		{
			Zones[zoneName].SetColor(Color.FromName(color), index, brightness);
			Zones[zoneName].SendLights(Zones[zoneName].LightingController);
		}

		public void SetAllZonesColor(string color, float brightness = 1)
		{
			Zones.ForEach(zone =>
			{
				zone.SetColor(Color.FromName(color).Darken(brightness));
				zone.SendLights(zone.LightingController);
			});
		}

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

		#endregion

		#region Helpers

		private IEnumerable<Zone> GetZonesByNames(List<string> zoneNames)
		{
			IEnumerable<Zone> zones;
			if (zoneNames.Count == 1 && zoneNames.First().ToUpperInvariant() == "ALL")
			{
				zones = AvailableZones;
			}
			else
			{
				zones = Zones.Where(zone => zoneNames.Contains(zone.Name));
			}
			return zones;
		}

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
			=> Zones.Where(z => !ProgramSets.Any(p => p.Zones != null && p.Zones.Contains(z))).ToBetterList();

		public IEnumerable<string> AvailablePrograms => ZoneScaffolder.Instance.GetAvailablePrograms();

		/// <summary>
		/// Container for the external modules.
		/// </summary>
		private CompositionContainer ExternalZoneContainer { get; set; }

		/// <summary>
		/// List of ProgramSets that are being managed by this class.
		/// </summary>
		public BetterList<ProgramSet> ProgramSets { get; private set; } = new BetterList<ProgramSet>();

		#endregion

		#region C+I

		/// <summary>
		/// Initializes a new instance of the <see cref="ZLM"/> (ZoneLightingManager) class.
		/// </summary>
		/// <param name="loadZoneModules">if set to <c>true</c> loads external zone modules from assemblies in the folder provided in the configuration appSetting with key "ZoneDLLFolder".</param>
		/// <param name="loadZonesFromConfig">if set to <c>true</c>, loads external zones from the saved configuration file provided in the application configuration (app/web.config) appSetting with key "ZoneConfigurationSaveFile".</param>
		/// <param name="loadProgramSetsFromConfig">if set to <c>true</c>, loads program sets from the saved configuration file provided in the application configuration appSetting (app/web.config) with key "ProgramSetConfigurationSaveFile".</param>
		/// <param name="initAction">If provided, this action will be executed after this instance is initialized.</param>
		/// <param name="fadeCandyConfigFilePath">The path at which the FadeCandy configuration file is located.</param>
		public ZLM(bool loadZoneModules = false, bool loadZonesFromConfig = true, bool loadProgramSetsFromConfig = true,
			Action<ZLM> initAction = null, string fadeCandyConfigFilePath = null)
		{
			InitLightingControllers(fadeCandyConfigFilePath);
			InitZoneScaffolder();
			if (loadZoneModules && ExternalZoneContainer != null)
				ComposeWithExternalModules();
			if (loadZonesFromConfig)
				LoadZonesFromConfig();
			if (loadProgramSetsFromConfig)
				LoadProgramSetsFromConfig();
			initAction?.Invoke(this);
		}

		/// <summary>
		/// Add code here to initialize any other lighting controllers.
		/// </summary>
		private void InitLightingControllers(string configFilePath)
		{
			FadeCandyController.Instance.Initialize(configFilePath);
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

		public void StopZones()
		{
			Zones.Parallelize(zone => zone.Stop());
		}

		public void Dispose()
		{
			DisposeProgramSets();
			ProgramSets = null;
			DisposeZones();
			Zones = null;
			UninitLightingControllers();
			UninitZoneScaffolder();
			ExternalZoneContainer?.Dispose();
			ExternalZoneContainer = null;
		}

		public void DisposeZones()
		{
			Zones?.Parallelize(zone => zone?.Dispose());
			Zones?.Clear();
		}

		#endregion

		#region Save/Load

		private void LoadZonesFromConfig(string filename = null)
		{
			Zones.AddRange(Config.DeserializeZones(File.ReadAllText(filename ?? Refigure.Config.Get("ZoneConfigurationSaveFile",
				"Zone configuration save file not found."))));
		}

		private void LoadProgramSetsFromConfig(string filename = null)
		{
			ProgramSets.AddRange(
				Config.DeserializeProgramSets(File.ReadAllText(filename ?? Refigure.Config.Get("ProgramSetConfigurationSaveFile",
					"Program Set configuration save file not found.")), Zones));
		}

		public void SaveProgramSets(string filename = null)
		{
			Config.SaveProgramSets(ProgramSets,
				filename ?? Refigure.Config.Get("ProgramSetConfigurationSaveFile", "Program Set configuration save file not found."));
		}

		public void SaveZones(string filename = null)
		{
			Config.SaveZones(Zones,
				filename ?? Refigure.Config.Get("ZoneConfigurationSaveFile", "Zone configuration save file not found."));
		}

		#endregion

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
	}
}
