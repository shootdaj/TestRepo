using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ZoneLighting.Communication;
using ZoneLighting.ZoneProgram;

namespace ZoneLighting.ZoneNS
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
		public LightingController LightingController { get; private set; }

		/// <summary>
		/// The program that is active on this zone.
		/// </summary>
		public IZoneProgram ZoneProgram { get; private set; }

		#endregion

		#region C+I

		public Zone(LightingController lightingController, string name = "", IZoneProgram program = null, IZoneProgramParameter programParameter = null)
		{
			Zones = new List<Zone>();
			Lights = new List<ILogicalRGBLight>();
			LightingController = lightingController;
			Name = name;
			if (program == null) return;
			if (programParameter == null)
			{
				SetProgram(program);
			}
			else
			{
				Initialize(program, programParameter);
			}
		}

		private void Initialize(IZoneProgramParameter parameter)
		{
			if (!Initialized)
			{
				if (ZoneProgram != null)
					StartProgram(parameter);

				foreach (var zone in Zones)
				{
					zone.Initialize(parameter);
				}
				Initialized = true;
			}
		}

		public void Initialize(IZoneProgram zoneProgram, IZoneProgramParameter parameter)
		{
			if (!Initialized)
			{
				SetProgram(zoneProgram);
				Initialize(parameter);
			}
		}

		public bool Initialized { get; private set; }

		public void Uninitialize()
		{
			if (Initialized)
			{
				StopProgram();

				foreach (var zone in Zones)
				{
					zone.Uninitialize();
				}

				Initialized = false;
			}
		}

		public void Dispose()
		{
			Uninitialize();
			Zones.Clear();
			Zones = null;
			Lights.Clear();
			Lights = null;	
		}

		#endregion

		#region MISC

		public override string ToString()
		{
			return Name;
		}

		#endregion

		#region API

		/// <summary>
		/// Sets this zone's program to the given program.
		/// </summary>
		/// <param name="program"></param>
		public void SetProgram(IZoneProgram program)
		{
			ZoneProgram = program;
			ZoneProgram.Zone = this;
		}
	
		/// <summary>
		/// Starts this zone's program with the given parameter
		/// </summary>
		/// <param name="parameter"></param>
		public void StartProgram(IZoneProgramParameter parameter)
		{
			ZoneProgram.StartBase(parameter);
		}

		/// <summary>
		/// Stops this zone's program.
		/// </summary>
		public void StopProgram()
		{
			ZoneProgram.Stop();
		}

		/// <summary>
		/// Sets all lights in zone to a given color.
		/// </summary>
		/// <param name="color"></param>
		public void SetAllLightsColor(Color color)
		{
			Lights.ToList().ForEach(x => x.SetColor(color));
		}

		/// <summary>
		/// Adds a new light to this zone.
		/// </summary>
		/// <param name="light"></param>
		public void AddLight(ILogicalRGBLight light)
		{
			Lights.Add(light);
		}

		/// <summary>
		/// Adds a new zone to this zone recursively.
		/// </summary>
		/// <param name="zone"></param>
		public void AddZone(Zone zone)
		{
			Zones.Add(zone);
		}

		#endregion
	}
}
