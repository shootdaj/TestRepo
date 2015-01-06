using System.Collections.Generic;
using System.Linq;

namespace ZoneLighting.Communication
{
	/// <summary>
	/// Represents a single frame of pixels that will be reflected on the channel on which it's sent.
	/// This can be seen as a unit of data that can be sent to the OPC device (FadeCandy or whatever).
	/// </summary>
	public class OPCPixelFrame : WebSocketOPCPacket, IPixelFrame
	{
		public OPCPixelFrame(byte channel, IList<byte> data)
			: base(channel, data, OPCCommand.SetPixelColors)
		{
		}

		#region API

		/// <summary>
		/// Sends this Pixel Frame instance using the given Lighting Controller.
		/// </summary>
		/// <param name="controller">The lighting controller to use to </param>
		public void Send(ILightingController controller)
		{
			controller.SendPixelFrame(this);
		}
		
		/// <summary>
		/// Creates an OPC Pixel Frame from a list of LEDs.
		/// </summary>
		/// <param name="channel">Channel this frame will be sent to.</param>
		/// <param name="leds">List of LEDs to map.</param>
		public static OPCPixelFrame CreateFromLEDs(byte channel, IList<IFadeCandyPixelContainer> leds)
		{
			var data = new byte[leds.Count * 3];

			foreach (IFadeCandyPixelContainer led in leds)
			{
				data[led.FadeCandyPixel.RedIndex] = led.Color.R;
				data[led.FadeCandyPixel.GreenIndex] = led.Color.G;
				data[led.FadeCandyPixel.BlueIndex] = led.Color.B;
			}

			var returnValue = new OPCPixelFrame(channel, data);
			return returnValue;
		}

		/// <summary>
		/// Since OPC messages contain a single channel, a separate message needs to be 
		/// sent for each channel within the LEDs collection.
		/// </summary>
		/// <param name="leds"></param>
		/// <returns></returns>
		public static IList<OPCPixelFrame> CreateChannelBurstFromLEDs(IList<IFadeCandyPixelContainer> leds)
		{
			var returnValue = new List<OPCPixelFrame>();

			foreach (var channel in leds.Select(x => x.FadeCandyPixel.Channel).Distinct())
			{
				returnValue.Add(CreateFromLEDs(channel,
					leds.Where(x => x.FadeCandyPixel.Channel == channel).ToList()));
			}

			return returnValue;
		}


		///// <summary>
		///// Creates an OPC Pixel Frame from a list of LEDs.
		///// </summary>
		///// <param name="channel">Channel this frame will be sent to.</param>
		///// <param name="leds">List of LEDs to map.</param>
		//public static OPCPixelFrame CreateFromLEDCollectionOld(byte channel, IList<LED> leds)
		//{
		//	var data = new List<byte>();

		//	foreach (LED led in leds)
		//	{
		//		data.AddRange(new[]{led.Red, led.Green, led.Blue});
		//	}

		//	var returnValue = new OPCPixelFrame(channel, data);
		//	return returnValue;
		//}

		#endregion
	}

	public interface IPixelFrame
	{
	}
}