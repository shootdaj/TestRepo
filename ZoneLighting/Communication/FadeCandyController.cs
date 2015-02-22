using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using WebSocketSharp;

namespace ZoneLighting.Communication
{
	/// <summary>
	/// This class is used to connect and send/receive messages to a FadeCandy
	/// board using WebSockets.
	/// </summary>
	public class FadeCandyController : LightingController, IInitializable
	{
		#region Singleton

		private static FadeCandyController _instance;
		public Process FadeCandyServerProcess { get; } = new Process();

		public static FadeCandyController Instance
			=> _instance ?? (_instance = new FadeCandyController(ConfigurationManager.AppSettings["FadeCandyServerURL"]));

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

		public override Type PixelType => typeof (IFadeCandyPixelContainer);

		#endregion

		#region C+I+D

		public FadeCandyController(string serverURL)
		{
			ServerURL = serverURL;
		}

		public bool Initialized { get; private set; }

		public bool FCServerRunning { get; private set; } = false;

		public void Initialize()
		{
			if (!Initialized)
			{
				StartFCServer();
				WebSocket = new WebSocket(ServerURL);
				Connect();
				Initialized = true;
			}
		}

		private void StartFCServer(bool createWindow = false)
		{
			var execExists = File.Exists(ConfigurationManager.AppSettings["FCServerExecutablePath"]);
			var configExists = File.Exists(ConfigurationManager.AppSettings["FCServerConfigFilePath"]);
			if (!execExists) throw new Exception("fcserver.exe not found at specified location. Please update the location in the config file.");
			if (!configExists) throw new Exception("FCServer configuration file not found at specified location. Please update the location in the config file.");

			if (FCServerRunning) return;
			var cmdInfo = new ProcessStartInfo
			{
				FileName = ConfigurationManager.AppSettings["FCServerExecutablePath"],
				Arguments = ConfigurationManager.AppSettings["FCServerConfigFilePath"],
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = !createWindow
			};
			FadeCandyServerProcess.StartInfo = cmdInfo;
			FadeCandyServerProcess.OutputDataReceived += (s, e1) =>
			{
				if (!string.IsNullOrEmpty(e1.Data))
				{
					//Console.WriteLine(e1.Data);
					//do something with returned data? --> procOut += e1.Data + Environment.NewLine;
				}
			};
			FadeCandyServerProcess.Start();
			FadeCandyServerProcess.BeginOutputReadLine();

			FCServerRunning = true;
		}

		/// <summary>
		/// Starts the WebSocket connections.
		/// </summary>
		public void Connect()
		{
			WebSocket.Connect();
		}

		public override void Dispose()
		{
			Uninitialize();
			ServerURL = null;
		}

		public void Uninitialize()
		{
			if (Initialized)
			{
				Disconnect();
				StopFCServer();
				WebSocket = null;
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
		
		private void StopFCServer()
		{
			if (!FCServerRunning) return;
			FadeCandyServerProcess.Kill();
			FCServerRunning = false;
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
		public override void SendPixelFrame(IPixelFrame opcPixelFrame)
		{
			var byteArray = ((OPCPixelFrame)opcPixelFrame).ToByteArray();
			string byteArrayString = DateTime.Now.ToLongTimeString() + ":" + "Sending {";
			byteArray.ToList().ForEach(x => byteArrayString += x + ",");
			byteArrayString += "}";
			Debug.Print(byteArrayString);
			AssertInit();
			if (WebSocket.ReadyState == WebSocketState.Closed)
				Connect();
			WebSocket.Send(byteArray); //TODO: Change this to async?
		}

		/// <summary>
		/// Sends a list of LEDs to the connected FadeCandy board.
		/// </summary>
		public override void SendLEDs(IList<ILightingControllerPixel> leds)
		{
			OPCPixelFrame.CreateChannelBurstFromLEDs(leds.Cast<IFadeCandyPixelContainer>().ToList()).ToList().ForEach(SendPixelFrame);
		}

		public WebSocketState ConnectionState => WebSocket.ReadyState;

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
