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

		public void Uninitialize()
		{
			if (Initialized)
			{
				UninitializeProgramSets();
				UninitializeZones();
				UninitLightingControllers();
				Initialized = false;
			}
		}
		
		private void UninitializeZones()
		{
			Parallel.ForEach(Zones, zone =>
			{
				zone.Stop();
			});
		}

		private void UninitializeProgramSets()
		{
			ProgramSets.ForEach(programSet => programSet.Dispose());
			ProgramSets.Clear();
		}

		public void Dispose()
		{
			Uninitialize();
			DisposeZones();
			UninitZoneScaffolder();
			ExternalZoneContainer?.Dispose();
			ExternalZoneContainer = null;
			Zones.Clear();
			Zones = null;
		}

		private void DisposeZones()
		{
			Zones.ToList().ForEach(zone => zone.Dispose());
			Zones.Clear();
		}

		#endregion

		#region Save/Load

		private void LoadZonesFromConfig(string filename = null)
		{
			Zones.AddRange(Config.DeserializeZones(File.ReadAllText(filename ?? Config.Get("ZoneConfigurationSaveFile",
					"Zone configuration save file not found."))));
		}

		private void LoadProgramSetsFromConfig(string filename = null)
		{
			ProgramSets.AddRange(Config.DeserializeProgramSets(File.ReadAllText(filename ?? Config.Get("ProgramSetConfigurationSaveFile",
					"Program Set configuration save file not found.")), Zones));
		}

		public void SaveProgramSets(string filename = null)
		{
			Config.SaveProgramSets(ProgramSets,
				filename ?? Config.Get("ProgramSetConfigurationSaveFile", "Program Set configuration save file not found."));
		}

		public void SaveZones(string filename = null)
		{
			Config.SaveZones(Zones, filename ?? Config.Get("ZoneConfigurationSaveFile", "Zone configuration save file not found."));
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
