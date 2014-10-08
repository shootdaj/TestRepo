using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoneLighting.Communication;

namespace ZoneLighting
{
	/// <summary>
	/// Represents a zone (room or whatever) that contains the lights to be controlled.
	/// </summary>
    public class Zone : IInitializable, IDisposable
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
		private SortedList<int, ILight> Lights { get; set; }

		/// <summary>
		/// The Lighting Controller used to control this Zone.
		/// </summary>
		public ILightingController LightingController { get; private set; }

		/// <summary>
		/// Scrolls a dot across the entire length of Lights
		/// </summary>
		private void ScrollDot()
		{
			while (!ScrollDotCTS.IsCancellationRequested)
			{
				for (int i = 0; i < 16; i++)
				{
					Lights.Values.ToList().ForEach(x => x.SetColor(Color.FromArgb(0, 0, 0))); //set all lights to black
					Lights[i].SetColor(Color.White); //set one to white

					//TODO: This is where the mapping provider would map the Lights collection to the byte order of the data in the OPCPixelFrame
					//send frame 
					LightingController.SendPixelFrame(OPCPixelFrame.CreateFromLightsCollection(1, Lights.Values.Cast<LED>().ToList()));
				}
			}
		}

		public void CancelScrollDot()
		{
			ScrollDotCTS.Cancel();
		}

		private CancellationTokenSource ScrollDotCTS { get; set; }

		#endregion

		#region C+I

		public Zone(ILightingController lightingController, string name = "")
		{
			Zones = new List<Zone>();
			Lights = new SortedList<int, ILight>();
			LightingController = lightingController;
			ScrollDotCTS = new CancellationTokenSource();
			Name = name;
		}

		public void Initialize()
		{
			if (!Initialized)
			{
				foreach (var zone in Zones)
				{
					zone.Initialize();
				}
				Task.Factory.StartNew(ScrollDot);
				Initialized = true;
			}
		}

		public bool Initialized { get; private set; }

		public void Uninitialize()
		{
			if (Initialized)
			{
				CancelScrollDot();
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

		//public void SetColor(Color color)
		//{
		//	foreach (var light in Lights)
		//	{
		//		light.SetColor(color);
		//	}
		//}

		public void AddLight(ILight light)
		{
			Lights.Add(Lights.Count, light);
		}

		public void AddZone(Zone zone)
		{
			Zones.Add(zone);
		}

		#endregion
	}
}
