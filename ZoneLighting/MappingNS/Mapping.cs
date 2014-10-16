//using ZoneLighting.Communication;

//namespace ZoneLighting.MappingNS
//{
//	public class Mapping<T, U> where T : PhysicalRGBLight where U : ILogicalRGBLight
//	{
//		public U LogicalRGBLight { get; set; }
//		public T PhysicalRGBLight { get; set; }
//		public ILightingController LightingController { get; set; }

//		public Mapping(U logicalRGBLight, T physicalIndex, ILightingController lc)
//		{
//			LogicalRGBLight = logicalRGBLight;
//			PhysicalRGBLight = physicalIndex;
//			LightingController = lc;
//		}

//		//public static Mapping<T> CreateMapping(int logicalIndex, T physicalIndex, ILightingController lc)
//		//{
//		//	var returnValue = new Mapping<T>
//		//	{
//		//		LogicalRGBLight = {LogicalIndex = logicalIndex},
//		//		PhysicalRGBLight = physicalIndex,
//		//		LightingController = lc
//		//	};

//		//	return returnValue;
//		//}
//	}
//}
