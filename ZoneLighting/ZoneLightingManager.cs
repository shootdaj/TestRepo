using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Communication;

namespace ZoneLighting
{
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
				FadeCandyController.Instance.Initialize();
				LoadSampleData();	//TODO: Replace
				Zones.ToList().ForEach(z => z.Initialize());
				Initialized = true;
			}
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

		#region Sample Values

		public void LoadSampleData()
		{
			var topLeftZone = new Zone(FadeCandyController.Instance, "TopLeft");
			Zones.Add(topLeftZone);
			for (int i = 0; i < 16; i++)
			{
				topLeftZone.AddLight(new LED());
			}
		}

		#endregion

	}
}
