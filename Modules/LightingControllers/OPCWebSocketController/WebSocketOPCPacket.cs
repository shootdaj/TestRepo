using System.Collections.Generic;

namespace OPCWebSocketController
{
	/// <summary>
	/// Represents an OPC Packet that can be sent as a WebSocket message. Currently,
	/// this can be either a Pixel Frame or a Control Frame.
	/// </summary>
	public class WebSocketOPCPacket : OPCPacket
	{
		#region CORE

		/// <summary>
		/// The Length field of a WebSocket OPC Packet is set at 0 because 
		/// the WebSocket protocol includes its own length field. It was chosen to hide
		/// this property rather than make it virtual in the base class because the Length 
		/// is only 0 for this type of OPC Packet, and this is the special case.
		/// </summary>
		public new ushort Length { get { return base.Length; } }

		#endregion

		#region C+I

		public WebSocketOPCPacket()
		{
			base.Length = 0;
		}

		public WebSocketOPCPacket(byte channel, IList<byte> data, OPCCommand command)
		{
			Channel = channel;
			Data = data;
			Command = command;
		}

		#endregion
	}
}
