using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZoneLighting.Communication
{
	public class FadeCandyController : ILightingController
	{
		#region Singleton

		private static FadeCandyController _instance;

		public static FadeCandyController Instance
		{
			get {
				return _instance ?? (_instance = new FadeCandyController(ConfigurationManager.AppSettings["FadeCandyServerURL"]));
			}
		}

		#endregion

		#region CORE

		string ServerURL { get; set; }

		//WebSocketClient WebSocketClient { get; set; }
		ClientWebSocket WebSocket { get; set; }

		#endregion

		#region C+I+D

		public FadeCandyController(string serverURL)
		{
			WebSocket = new ClientWebSocket();
			ServerURL = serverURL;
			//WebSocketClient = new WebSocketClient(serverURL);
		}

		public void Dispose()
		{
			WebSocket.Abort();
			WebSocket.Dispose();
			//WebSocketClient = null;
		}

		#region Un/Initialization

		public bool Initialized { get; private set; }
		
		public async void Initialize()
		{
			if (!Initialized)
			{
				await Connect();
				Initialized = true;
			}
		}

		public void Uninitialize()
		{
			if (Initialized)
			{
				WebSocket.Abort();
				Initialized = false;
			}
		}

		public void AssertInit()
		{
			if (!Initialized)
				throw new Exception("FadeCandyController instance is not initialized.");
		}

		public async Task Connect()
		{
			//AssertInit();
			await WebSocket.ConnectAsync(new Uri(ServerURL), new CancellationToken());
		}

		public void Disconnect()
		{
			AssertInit();
			WebSocket.Abort();
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
			WebSocket.SendAsync(new ArraySegment<byte>(opcPixelFrame.ToByteArray()), WebSocketMessageType.Binary, true,
				new CancellationToken());
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
