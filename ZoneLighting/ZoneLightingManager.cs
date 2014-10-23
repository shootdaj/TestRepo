using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

		public IList<Zone> Zones { get; set; }
		public IList<IZoneProgram> ZonePrograms { get; set; }

		#endregion

		#region C+I

		public ZoneLightingManager()
		{
			Zones = new List<Zone>();
		}

		public void Initialize()
		{
			if (!Initialized)
			{
				InitLightingControllers();
				LoadSampleZoneData();	//TODO: Replace
				InitializeAllZones();
				//var sw = File.CreateText(@"C:\Temp\Zones.txt");
				//sw.Write(JsonConvert.SerializeObject(Zones, Formatting.Indented));
				//sw.Close();
				Initialized = true;
			}
		}

		private void InitializeAllZones()
		{
			Zones.ToList().ForEach(z => z.Initialize());
			Zones.ToList()
				.ForEach(z =>
				{
					z.StartProgram(new Rainbow(), new RainbowParameter(3000));
					ProgramCommon.Delay(500);
				});
		}

		/// <summary>
		/// Add code here to initialize any other lighting controllers
		/// </summary>
		private void InitLightingControllers()
		{
			FadeCandyController.Instance.Initialize();
		}

		public bool Initialized { get; private set; }
		public void Uninitialize()
		{
			if (Initialized)
			{
				Zones.ToList().ForEach(z => z.Uninitialize());
				Zones.Clear();
				Initialized = false;
			}
		}

		public void Dispose()
		{
			Zones.Clear();
			Zones = null;
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
