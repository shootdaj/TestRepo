using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoneLighting.Communication;
using ZoneLighting.ZoneProgram;

namespace ZoneLighting
{
	/// <summary>
	/// Represents a zone (room or whatever) that contains the lights to be controlled.
	/// </summary>
    public class Zone : IDisposable
	{
		#region CORE

		/// <summary>
		/// Name of the zone.
		/// </summary>
		public string Name;

		/// <summary>
		/// Zones can contain other zones in a recursive fashion.
		/// </summary>
		public IList<Zone> Zones { get; private set; }

		/// <summary>
		/// All lights in the zone.
		/// </summary>
		public IList<ILogicalRGBLight> Lights { get; set; }

		/// <summary>
		/// The Lights list as a dictionary with the logical index as the key and the light as the value.
		/// </summary>
		public Dictionary<int, ILogicalRGBLight> SortedLights
		{
			get { return Lights.ToDictionary(x => x.LogicalIndex); }
		}

		/// <summary>
		/// The Lighting Controller used to control this Zone.
		/// </summary>
		public ILightingController LightingController { get; private set; }

		/// <summary>
		/// The program that is active on this zone.
		/// </summary>
		public IZoneProgram ActiveZoneProgram { get; private set; }

		///// <summary>
		///// Static Red
		///// </summary>
		//private void StaticRed()
		//{
		//	while (!TaskCTS.IsCancellationRequested)
		//	{
		//		var color = Color.Red;

		//		Lights.ToList().ForEach(x => x.SetColor(color)); //set all lights to black
		//		LightingController.SendPixelFrame(OPCPixelFrame.CreateFromLEDs(0, Lights.Cast<LED>().ToList()));	//set all lights to Red
		//	}
		//}

		///// <summary>
		///// Rainbow
		///// </summary>
		//private void Rainbow()
		//{
		//	SetAllLightsColor(Color.FromArgb(0, 0, 0)); //turn all lights off

		//	while (!TaskCTS.IsCancellationRequested)
		//	{
		//		var colors = new List<Color>();
		//		colors.Add(Color.Violet);
		//		colors.Add(Color.Indigo);
		//		colors.Add(Color.Blue);
		//		colors.Add(Color.Green);
		//		colors.Add(Color.Yellow);
		//		colors.Add(Color.Orange);
		//		colors.Add(Color.Red);

		//		for (int i = 0; i < 7; i++)
		//		{
		//			SetAllLightsColor(colors[i]);

		//			//send frame 
		//			LightingController.SendPixelFrame(OPCPixelFrame.CreateFromLEDs(0, Lights.Cast<LED>().ToList()));
		//			Thread.Sleep(5000);
		//		}
		//	}
		//}
		
		#endregion

		#region C+I

		public Zone(ILightingController lightingController, string name = "", IZoneProgram program = null, IZoneProgramParameter programParameter = null)
		{
			Zones = new List<Zone>();
			Lights = new List<ILogicalRGBLight>();
			LightingController = lightingController;
			Name = name;
			if (program != null && programParameter != null)
			{
				StartProgram(program, programParameter);
			}
		}

		public void Initialize(IZoneProgramParameter parameter)
		{
			if (!Initialized)
			{
				if (ActiveZoneProgram != null)
					ActiveZoneProgram.Start(parameter);

				foreach (var zone in Zones)
				{
					zone.Initialize(parameter);
				}
				//Task.Factory.StartNew(ScrollDot);
				//Task.Factory.StartNew(Rainbow);
				Initialized = true;
			}
		}

		public bool Initialized { get; private set; }

		public void Uninitialize()
		{
			if (Initialized)
			{
				ActiveZoneProgram.Stop();

				foreach (var zone in Zones)
				{
					zone.Uninitialize();
				}

				Initialized = false;
			}
		}

		public void Dispose()
		{
			Zones.Clear();
			Zones = null;
			Lights.Clear();
			Lights = null;
			LightingController.Dispose();
			LightingController = null;
		}

		#endregion

		#region API

		public void StartProgram(IZoneProgram program, IZoneProgramParameter parameter)
		{
			SetProgram(program);
			ActiveZoneProgram.Start(parameter);
		}

		public void SetProgram(IZoneProgram program)
		{
			ActiveZoneProgram = program;
			ActiveZoneProgram.Zone = this;
		}

		public void StopProgram()
		{
			ActiveZoneProgram.Stop();
		}

		private void SetAllLightsColor(Color color)
		{
			Lights.ToList().ForEach(x => x.SetColor(color)); //set all lights to black
		}

		public void AddLight(ILogicalRGBLight light)
		{
			Lights.Add(light);
		}

		public void AddZone(Zone zone)
		{
			Zones.Add(zone);
		}

		#endregion
	}
}
