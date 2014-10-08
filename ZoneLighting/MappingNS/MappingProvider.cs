using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Communication;

namespace ZoneLighting.MappingNS
{
	public class MappingProvider : IInitializable
	{
		#region Singleton

		private static MappingProvider _instance;

		public static MappingProvider Instance
		{
			get { return _instance ?? (_instance = new MappingProvider()); }
		}

		#endregion

		#region CORE

		private IList<Mapping> Mappings;

		#endregion

		#region C+I

		public void Initialize()
		{
			if (!Initialized)
			{
				LoadSampleMappings();	//TODO: Replace
				Initialized = true;
			}
		}

		public bool Initialized { get; private set; }
		public void Uninitialize()
		{
			if (Initialized)
			{
				Mappings.Clear();
				Initialized = false;
			}
		}

		#endregion

		#region API

		public int GetLightingControllerIndex(ILightingController lightingController, int zoneIndex)
		{
			return Mappings.First(x => x.LightingController == lightingController && x.ZoneIndex == zoneIndex).LightingControllerIndex;
		}

		public int GetZoneIndex(ILightingController lightingController, int lightingControllerIndex)
		{
			return Mappings.First(x => x.LightingController == lightingController && x.LightingControllerIndex == lightingControllerIndex).ZoneIndex;
		}

		#endregion

		#region Sample Mappings

		private void LoadSampleMappings()
		{
			for (int i = 0; i < 63; i++)
			{
				Mappings.Add(new Mapping(i, i, FadeCandyController.Instance));	
			}
		}

		#endregion

		
	}
}
