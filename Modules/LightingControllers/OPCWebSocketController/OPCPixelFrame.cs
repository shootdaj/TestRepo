using System.Collections.Generic;
using System.Linq;

namespace OPCWebSocketController
{
	/// <summary>
	/// Represents a single frame of pixels that will be reflected on the channel on which it's sent.
	/// This can be seen as a unit of data that can be sent to the OPC device (FadeCandy or whatever).
	/// </summary>
	public class OPCPixelFrame : WebSocketOPCPacket
	{
		public OPCPixelFrame(byte channel, IList<byte> data)
			: base(channel, data, OPCCommand.SetPixelColors)
		{
		}

		#region API

		///// <summary>
		///// Sends this Pixel Frame instance using the given Lighting Controller.
		///// </summary>
		///// <param name="controller">The lighting controller to use to </param>
		//public void Send(ILightingController controller)
		//{
		//	controller.SendPixelFrame(this);
		//}
		
		/// <summary>
		/// Creates an OPC Pixel Frame from a list OPC Pixels (or leds)
		/// </summary>
		/// <param name="channel">Channel this frame will be sent to.</param>
		/// <param name="opcPixels">List of LEDs to map.</param>
		public static OPCPixelFrame CreateFromOPCPixels(byte channel, IList<OPCPixel> opcPixels)
		{
			var data = new byte[opcPixels.Count * 3];

			foreach (OPCPixel led in opcPixels)
			{
				data[led.RedIndex] = led.Color.R;
				data[led.GreenIndex] = led.Color.G;
				data[led.BlueIndex] = led.Color.B;
			}

			var returnValue = new OPCPixelFrame(channel, data);
			return returnValue;
		}

		/// <summary>
		/// Since OPC messages contain a single channel, a separate message needs to be 
		/// sent for each channel within the LEDs collection.
		/// </summary>
		/// <param name="opcPixels"></param>
		/// <returns></returns>
		public static IList<OPCPixelFrame> CreateChannelBurstFromOPCPixels(IList<OPCPixel> opcPixels)
		{
			var returnValue = new List<OPCPixelFrame>();

			foreach (var channel in opcPixels.Select(x => x.Channel).Distinct())
			{
				returnValue.Add(CreateFromOPCPixels(channel,
					opcPixels.Where(x => x.Channel == channel).ToList()));
			}

			return returnValue;
		}

		#endregion
	}
}