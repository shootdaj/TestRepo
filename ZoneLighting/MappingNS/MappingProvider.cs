//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ZoneLighting.Communication;

//namespace ZoneLighting.MappingNS
//{
//	/// <summary>
//	/// This class provides mappings between the lights in a zone and 
//	/// the lights on a lighting controller.
//	/// </summary>
//	public class MappingProvider : IInitializable
//	{
//		#region Singleton

//		private static MappingProvider _instance;

//		public static MappingProvider Instance
//		{
//			get { return _instance ?? (_instance = new MappingProvider()); }
//		}

//		#endregion

//		#region CORE

//		private IList<Mapping<PhysicalRGBLight, ILogicalRGBLight>> Mappings;

//		#endregion

//		#region C+I

//		public void Initialize()
//		{
//			if (!Initialized)
//			{
//				LoadSampleMappings();	//TODO: Replace
//				Initialized = true;
//			}
//		}

//		public bool Initialized { get; private set; }
//		public void Uninitialize()
//		{
//			if (Initialized)
//			{
//				Mappings.Clear();
//				Initialized = false;
//			}
//		}

//		#endregion

//		#region API

//		public T GetMapInfo<T>(ILightingController lightingController, int zoneIndex) where T : PhysicalRGBLight
//		{
//			return (T)Mappings.First(x => x.LightingController == lightingController && x.ZoneIndex == zoneIndex).PhysicalRGBLight;
//		}
		
//		public int GetZoneIndex(ILightingController lightingController, PhysicalRGBLight pixel)
//		{
//			return Mappings.First(x => x.LightingController == lightingController && x.PhysicalRGBLight == pixel).ZoneIndex;
//		}

//		#endregion

//		#region Sample Mappings

//		private void LoadSampleMappings()
//		{
//			for (int i = 0; i < 63; i++)
//			{
//				Mappings.Add(new Mapping<PhysicalRGBLight, ILogicalRGBLight>(new LED(), new FadeCandyPixel(0, i), FadeCandyController.Instance));
//			}
//		}

//		public void AddFadeCandyMap(LED led, int channel, int physicalIndex, )
//		{
//			Mappings.Add(new Mapping<PhysicalRGBLight, ILogicalRGBLight>(new LED(logicalIndex: ), ));
//		}

//		#endregion

		
//	}
//}
