using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Alchemy;

namespace ZoneLighting.Communication
{
	public class FadeCandyController : ILightingController 
	{
		#region CORE

		WebSocketClient WebSocketClient { get; set; }

		#endregion

		#region C+I+D

		public FadeCandyController(string serverURL)
		{
			WebSocketClient = new WebSocketClient(serverURL);
		}

		public void Dispose()
		{
			WebSocketClient = null;
		}

		#region Un/Initialization

		public bool Initialized { get; private set; }
		
		public void Initialize()
		{
			if (!Initialized)
			{
				WebSocketClient.Connect();
				Initialized = true;
			}
		}

		public void Uninitialize()
		{
			if (Initialized)
			{
				WebSocketClient.Disconnect();
				Initialized = false;
			}
		}

		public void AssertInit()
		{
			if (!Initialized)
				throw new Exception("FadeCandyController instance is not initialized.");
		}

		public void Connect()
		{
			WebSocketClient.Connect();
		}

		public void Disconnect()
		{
			WebSocketClient.Disconnect();
		}

		#endregion

		#endregion

		#region API

		/// <summary>
		/// Sends a Pixel Frame to the connected FadeCandy board
		/// </summary>
		/// <param name="opcPixelFrame"></param>
		public void SendPixelFrame(OPCPixelFrame opcPixelFrame)
		{
			AssertInit();
			WebSocketClient.Send(opcPixelFrame.ToByteArray());
		}

		#endregion
	}

	

	//TODO: Test out control messages on the board and then finish this.
	///// <summary>
	///// Control Frames are used for administrative tasks on the FadeCandy server.
	///// </summary>
	//public class ControlFrame : WebSocketOPCPacket
	//{
	//	public ControlFrame(byte channel, IList<byte> data) : base(channel, data, OPCCommand.SystemExclusive)
	//	{
	//	}
	//}
}
