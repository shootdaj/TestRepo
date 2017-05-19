//using System.Collections.Generic;
//using System.Linq;
//using Newtonsoft.Json;
//using ZoneLighting.Communication;

//namespace ZoneLighting.ZoneNS
//{
//	public class OPCZone : Zone
//	{
//		[JsonConstructor]
//		public OPCZone(OPCWebSocketController lightingController = null, string name = "", double? brightness = null, byte? channel = null)
//			: base(lightingController, name, brightness)
//		{
//			Channel = channel;
//		}
		
//		///// <summary>
//		///// This constructor is for use by external zones.
//		///// </summary>
//		//protected OPCZone(OPCController lightingController, string name = "", double? brightness = null, byte? channel = null)
//		//	: base(lightingController, name, brightness)
//		//{ }

//		public void AddOPCLights(int numLights)
//		{
//			for (int i = 0; i < numLights; i++)
//			{
//				AddLight(new LED(index: i));
//			}
//		}

//		///// <summary>
//		///// Used to add OPCLights using a given mapping. The mapping is a map from the logical index to the physical index.
//		///// So if your first light (the way Programs and Zones see it), is actually the fifth light connected to OPC board, the entry 
//		///// for that light would be (1,5).
//		///// </summary>
//		///// <param name="pixelType">OPCPixelType.</param>
//		///// <param name="fcChannel">The OPC channel on which these lights are connected.</param>
//		///// <param name="logicalPhysicalMapping">The logical-physical mapping.</param>
//		//public void AddOPCLights(OPCPixelType pixelType, Dictionary<int, int> logicalPhysicalMapping)
//		//{
//		//	logicalPhysicalMapping.Keys.ToList().ForEach(key =>
//		//	{
//		//		AddLight(new LED(index: key));
//		//	});
//		//}

//		public byte? Channel { get; protected set; }
//	}
//}
