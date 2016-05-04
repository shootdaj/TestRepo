using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ZoneLighting.Communication;

namespace ZoneLighting.ZoneNS
{
	public class FadeCandyZone : Zone
	{
		[JsonConstructor]
		public FadeCandyZone(ILightingController lightingController = null, string name = "", double? brightness = null, byte? channel = null)
			: base(lightingController, name, brightness)
		{
			Channel = channel;
		}
		
		/// <summary>
		/// This constructor is for use by external zones.
		/// </summary>
		protected FadeCandyZone(string name = "", double? brightness = null)
			: base(FadeCandyController.Instance, name, brightness)
		{ }

		public void AddFadeCandyLights(PixelType pixelType, int numLights, byte fcChannel)
		{
			for (int i = 0; i < numLights; i++)
			{
				AddLight(new LED(logicalIndex: i, fadeCandyChannel: fcChannel, fadeCandyIndex: i, pixelType: pixelType));
			}
		}

		/// <summary>
		/// Used to add FadeCandyLights using a given mapping. The mapping is a map from the logical index to the physical index.
		/// So if your first light (the way Programs and Zones see it), is actually the fifth light connected to FadeCandy, the entry 
		/// for that light would be (1,5).
		/// </summary>
		/// <param name="pixelType">PixelType.</param>
		/// <param name="fcChannel">The FadeCandy channel on which these lights are connected.</param>
		/// <param name="logicalPhysicalMapping">The logical-physical mapping.</param>
		public void AddFadeCandyLights(PixelType pixelType, Dictionary<int, int> logicalPhysicalMapping, byte fcChannel)
		{
			logicalPhysicalMapping.Keys.ToList().ForEach(key =>
			{
				AddLight(new LED(logicalIndex: key, fadeCandyChannel: fcChannel, fadeCandyIndex: logicalPhysicalMapping[key],
					pixelType: pixelType));
			});
		}

		public byte? Channel { get; protected set; }
	}
}
