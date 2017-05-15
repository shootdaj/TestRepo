using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Anshul.Utilities;
using WebSocketSharp;
using Config = Refigure.Config;

namespace ZoneLighting.Communication
{
    /// <summary>
	/// This class is used to connect and send/receive messages to a FadeCandy
	/// board using WebSockets.
	/// </summary>
	public class FadeCandyController : OPCWebSocketController
    {
        #region Singleton

        private static FadeCandyController _instance;

        public static FadeCandyController Instance
            => _instance ?? (_instance = new FadeCandyController(ConfigurationManager.AppSettings["FadeCandyServerURL"]));

        #endregion

        #region CORE

        private Process FadeCandyServerProcess { get; set; } = new Process();

        public bool FCServerRunning { get; private set; } = false;

        protected override int NodeMCUWifiThreadSleepTime { get; set; } = 0;

        #endregion

        #region C+I+D

        public FadeCandyController(string serverURL) : base(serverURL)
        {
        }

        public void KillFCServer()
        {
            TryClass.Try(() =>
            {
                foreach (
                    var process in
                        Process.GetProcessesByName(Config.Get("FCServerExecutablePath").Split('\\').Last().Split('.').First()))
                {
                }
            }, 5, false, () => { Console.WriteLine("Unable to kill FCServer."); });
        }

        public void Initialize(string configFilePath = null)
        {
            if (!Initialized)
            {
                KillFCServer();
                StartFCServer(configFilePath ?? ConfigurationManager.AppSettings["FCServerConfigFilePath"]);
                base.Initialize();
                Initialized = true;
            }
        }

        private void StartFCServer(string configFilePath, bool createWindow = false)
        {
            //need to set this because otherwise for WebController, the file is loaded from runner's dir 
            //for example if its resharper, it tries to load from resharper's base dir
            //if its iis, it tries to load from w3wp or iisexpress's base dir
            var oldEnvDir = Environment.CurrentDirectory;
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var execExists = File.Exists(ConfigurationManager.AppSettings["FCServerExecutablePath"]);
            var configExists = File.Exists(configFilePath);
            if (!execExists)
                throw new Exception(
                    "fcserver.exe not found at specified location. Please update the location in the config file. BaseDirectory: " +
                    AppDomain.CurrentDomain.BaseDirectory + ". Looking for path: " +
                    Path.GetFullPath(Config.Get("FCServerExecutablePath")) + ". Environment Directory: " + Environment.CurrentDirectory);
            if (!configExists)
                throw new Exception(
                    "FCServer configuration file not found at specified location. Please update the location in the config file. BaseDirectory: " +
                    AppDomain.CurrentDomain.BaseDirectory + ". Looking for path: " +
                    Path.GetFullPath(configFilePath) + ". Environment Directory: " + Environment.CurrentDirectory);

            if (FCServerRunning) return;
            var cmdInfo = new ProcessStartInfo
            {
                FileName = Config.Get("FCServerExecutablePath"),
                Arguments = configFilePath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = !createWindow
            };
            FadeCandyServerProcess = new Process { StartInfo = cmdInfo };
            //FadeCandyServerProcess.OutputDataReceived += (s, e1) =>
            //{
            //	if (!string.IsNullOrEmpty(e1.Data))
            //	{
            //		//Console.WriteLine(e1.Data);
            //		//do something with returned data from the fcserver process? log it? --> procOut += e1.Data + Environment.NewLine;
            //	}
            //};
            FCServerRunning = FadeCandyServerProcess.Start();
            //FadeCandyServerProcess.BeginOutputReadLine();

            //FCServerRunning = true;

            Environment.CurrentDirectory = oldEnvDir;
        }

        public override void Dispose()
        {
            base.Dispose();
            Uninitialize();
            _instance = null;
        }

        public override void Uninitialize()
        {
            if (Initialized)
            {
                if (WebSocket.ReadyState == WebSocketState.Connecting || WebSocket.ReadyState == WebSocketState.Open)
                    base.Uninitialize();
                StopFCServer();
                Initialized = false;
            }
        }

        private void StopFCServer()
        {
            if (!FCServerRunning) return;
            if (FadeCandyServerProcess.HasExited) return;
            FadeCandyServerProcess.Kill();
            FadeCandyServerProcess.Dispose();
            FCServerRunning = false;
        }

        #endregion

        #region API

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
