using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Communication;

namespace ZoneLighting
{
	/// <summary>
	/// Represents a zone (room or whatever) that contains the lights to be controlled.
	/// </summary>
    public class Zone : IInitializable
	{
		#region CORE

		/// <summary>
		/// Zones can contain other zones in a recursive fashion.
		/// </summary>
		public IList<Zone> Zones { get; private set; }

		/// <summary>
		/// All lights in the area.
		/// </summary>
		public IList<ILight> Lights { get; private set; }

		/// <summary>
		/// The Lighting Controller used to control this Zone.
		/// </summary>
		public ILightingController LightingController { get; private set; }

		#endregion

		#region C+I

		public Zone(ILightingController lightingController)
		{
			LightingController = lightingController;
		}

		public void Initialize()
		{
			if (!Initialized)
			{
				foreach (var zone in Zones)
				{
					zone.Initialize();
				}
				Initialized = true;
			}
		}

		public bool Initialized { get; private set; }

		public void Uninitialize()
		{
			if (Initialized)
			{
				Initialized = false;
			}
		}

		#endregion

		#region API

		public void SetColor(Color color)
		{
			foreach (var light in Lights)
			{
				light.SetColor(color);
			}
		}

		public void AddLight(ILight light)
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
