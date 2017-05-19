using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Anshul.Utilities;
using Refigure;
using WebSocketSharp;
using ZoneLighting.Communication;

namespace OPCWebSocketController
{
    public class OPCWebSocketController : ILightingController, IDisposable
    {
        protected virtual int NodeMCUWifiThreadSleepTime { get; set; } = Config.GetAsInt("NodeMCUWIFIThreadSleepTime");

        #region CORE

        /// <summary>
        /// The WebSocket that will be used to send/receive messages to/from the FadeCandy board.
        /// </summary>
        protected WebSocket WebSocket { get; set; }

        /// <summary>
        /// URL for the server on which FadeCandy is running.
        /// </summary>
        public string ServerURL { get; protected set; }

        public IPixelToOPCPixelMapper PixelMapper { get; }

        public bool Initialized { get; protected set; }

        public virtual OPCPixelType OPCPixelType { get; }
	    public byte Channel { get; set; }

	    #endregion

        #region C+I+D


        //Timer Timer = new Timer();
        //private int Ticks;

        public OPCWebSocketController(string serverURL, IPixelToOPCPixelMapper pixelMapper, OPCPixelType opcPixelType, byte channel)
        {
            ServerURL = serverURL;
            PixelMapper = pixelMapper;
            OPCPixelType = opcPixelType;
	        Channel = channel;

	        //Timer.Interval = 1000;

            //Timer.Elapsed += (sender, args) =>
            //{
            //    Console.WriteLine(Ticks);
            //    Ticks = 0;
            //};

            //Timer.Start();
        }
        
        public void Dispose()
        {
            Uninitialize();
            ServerURL = null;
        }

        protected void Initialize()
        {
            WebSocket = new WebSocket(ServerURL);
            Connect();
        }

        public virtual void Uninitialize()
        {
            Disconnect();
            WebSocket = null;
        }

        /// <summary>
        /// Starts the WebSocket connections.
        /// </summary>
        public void Connect()
        {
            TryClass.Try(() =>
            {
                WebSocket.Connect();
            });
        }

        /// <summary>
        /// Stops the WebSocket connections.
        /// </summary>
        public void Disconnect()
        {
            AssertInit();
            WebSocket.Close();
        }

        public void AssertInit()
        {
            if (!Initialized)
                throw new Exception("OPCWebSocketController instance is not initialized.");
        }
        
        #endregion

        #region API

        /// <summary>
        /// Sends a Pixel Frame to the connected FadeCandy board.
        /// </summary>
        /// <param name="opcPixelFrame">The OPCPixelFrame to send to the board.</param>
        public void SendPixelFrame(OPCPixelFrame opcPixelFrame)
        {
            
            var byteArray = ((OPCPixelFrame)opcPixelFrame).Data;
			//var byteArrayString = DateTime.Now.ToLongTimeString() + ":" + "Sending {";
			//byteArray.ToList().ForEach(x => byteArrayString += x + ",");
			//byteArrayString += "}";
			//Debug.Print(byteArrayString);
			//AssertInit();

			if (WebSocket.ReadyState == WebSocketState.Closed)
                Connect();
            
            WebSocket.Send(byteArray.ToArray());
            if (NodeMCUWifiThreadSleepTime > 0)
                Thread.Sleep(NodeMCUWifiThreadSleepTime);
            
			//Ticks++;
        }


        /// <summary>
        /// Sends a list of LEDs to the connected FadeCandy board.
        /// </summary>
        public void SendLights(IList<IPixel> lights)
        {
            var opcLights = ConvertToOPCPixels(lights);
            OPCPixelFrame.CreateChannelBurstFromOPCPixels(opcLights).ToList().ForEach(SendPixelFrame);
        }

        private IList<OPCPixel> ConvertToOPCPixels(IList<IPixel> lights)
        {
            var opcPixels = lights.ToList()
                .Select(light =>
	            {
		            var opcPixel = OPCPixel.GetOPCPixelInstance(OPCPixelType, Channel,
			            PixelMapper.GetOPCPixelIndex(light.Index));
                    opcPixel.Color = light.Color;
                    return opcPixel;
                });

            return opcPixels.ToList();
        }

        #endregion


    }
}