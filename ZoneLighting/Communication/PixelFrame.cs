using System.Collections.Generic;

namespace ZoneLighting.Communication
{
	/// <summary>
	/// Represents a single frame of pixels that will be reflected on the channel on which it's sent.
	/// </summary>
	public class OPCPixelFrame : WebSocketOPCPacket
	{
		public OPCPixelFrame(byte channel, IList<byte> data)
			: base(channel, data, OPCCommand.SetPixelColors)
		{
		}

		#region API

		/// <summary>
		/// Sends this Pixel Frame instance using the given Lighting Controller.
		/// </summary>
		/// <param name="controller"></param>
		public void Send(ILightingController controller)
		{
			controller.SendPixelFrame(this);
		}

		#endregion
	}
}