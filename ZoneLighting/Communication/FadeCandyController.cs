using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using WebSocketSharp;

namespace ZoneLighting.Communication
{
	/// <summary>
	/// This class is used to connect and send/receive messages to a FadeCandy
	/// board using WebSockets.
	/// </summary>
	public class FadeCandyController : ILightingController
	{
		#region Singleton

		private static FadeCandyController _instance;

		public static FadeCandyController Instance
		{
			get
			{
				return _instance ?? (_instance = new FadeCandyController(ConfigurationManager.AppSettings["FadeCandyServerURL"]));
			}
		}

		#endregion

		#region CORE

		/// <summary>
		/// URL for the server on which FadeCandy is running.
		/// </summary>
		public string ServerURL { get; private set; }

		/// <summary>
		/// The WebSocket that will be used to send/receive messages to/from the FadeCandy board.
		/// </summary>
		private WebSocket WebSocket { get; set; }

		#endregion

		#region C+I+D

		public FadeCandyController(string serverURL)
		{
			ServerURL = serverURL;
			WebSocket = new WebSocket(ServerURL);
		}

		public bool Initialized { get; private set; }
		
		public void Initialize()
		{
			if (!Initialized)
			{
				Connect();
				Initialized = true;
			}
		}

		/// <summary>
		/// Starts the WebSocket connections.
		/// </summary>
		public void Connect()
		{
			WebSocket.Connect();
		}

		public void Uninitialize()
		{
			if (Initialized)
			{
				Disconnect();
				Initialized = false;
			}
		}

		/// <summary>
		/// Stops the WebSocket connections.
		/// </summary>
		public void Disconnect()
		{
			AssertInit();
			WebSocket.Close();
		}
		
		public void Dispose()
		{
			WebSocket = null;
		}

		public void AssertInit()
		{
			if (!Initialized)
				throw new Exception("FadeCandyController instance is not initialized.");
		}

		#endregion
		
		#region API

		/// <summary>
		/// Sends a Pixel Frame to the connected FadeCandy board.
		/// </summary>
		/// <param name="opcPixelFrame">The OPCPixelFrame to send to the board.</param>
		public void SendPixelFrame(IPixelFrame opcPixelFrame)
		{
			AssertInit();
			WebSocket.Send(((OPCPixelFrame)opcPixelFrame).ToByteArray()); //TODO: Change this to async?
		}

		/// <summary>
		/// Sends a list of LEDs to the connected FadeCandy board.
		/// </summary>
		public void SendLEDs(IList<LED> leds)
		{
			OPCPixelFrame.CreateChannelBurstFromLEDs(leds).ToList().ForEach(SendPixelFrame);
		}

		/// <summary>
		/// Sends a list of Lights to the connected FadeCandy board.
		/// </summary>
		public void SendLights(IList<ILogicalRGBLight> lights)
		{
			SendLEDs(lights.Cast<LED>().ToList());
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
