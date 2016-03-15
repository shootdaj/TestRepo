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

		public byte? Channel { get; protected set; }
	}
}
