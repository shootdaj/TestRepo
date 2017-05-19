using System;
using System.Collections.Generic;
using System.Linq;

namespace OPCWebSocketController
{
	/// <summary>
	/// Represents an Open Pixel Control packet. One packet maps to one channel and one command.
	/// http://openpixelcontrol.org/
	/// </summary>
	public class OPCPacket
	{
		#region CORE

		/// <summary>
		/// Channel for this OPC Packet.
		/// </summary>
		public byte Channel { get; set; }

		/// <summary>
		/// Command for this OPC Packet.
		/// </summary>
		public OPCCommand Command { get; set; }

		/// <summary>
		/// This is ushort because 2 bytes are needed to represent it and ushort is 2 bytes long.
		/// </summary>
		public ushort Length { get; set; }

		/// <summary>
		/// Data contained in the packet.
		/// </summary>
		public IList<byte> Data { get; set; }

		#endregion

		#region API

		/// <summary>
		/// Converts this OPC Packet instance to a byte array as specified by the OPC Protocol.
		/// https://github.com/scanlime/fadecandy/blob/master/doc/fc_protocol_opc.md
		/// </summary>
		/// <returns>Byte Array representation of the OPCCommand.</returns>
		public byte[] ToByteArray()
		{
			List<byte> returnValue = new byte[]
			{
				Channel,
				(byte) Command
			}.ToList();

			returnValue.AddRange(BitConverter.GetBytes(Length));
			returnValue.AddRange(Data);

			return returnValue.ToArray();
		}

		#endregion

		#region AUX

		public enum OPCCommand : byte
		{
			SetPixelColors = 0,
			SystemExclusive = 255
		}

		#endregion
	}
}