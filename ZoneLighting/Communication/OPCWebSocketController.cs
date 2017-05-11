using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anshul.Utilities;
using Refigure;
using WebSocketSharp;
using Timer = System.Timers.Timer;

namespace ZoneLighting.Communication
{
    public class OPCWebSocketController : OPCController
    {
        #region CORE

        /// <summary>
        /// The WebSocket that will be used to send/receive messages to/from the FadeCandy board.
        /// </summary>
        protected WebSocket WebSocket { get; set; }

        /// <summary>
        /// URL for the server on which FadeCandy is running.
        /// </summary>
        public string ServerURL { get; protected set; }

        public bool Initialized { get; protected set; }

        #endregion

        #region C+I+D


        //Timer Timer = new Timer();
        //private int Ticks;

        public OPCWebSocketController(string serverURL)
        {
            ServerURL = serverURL;

            //Timer.Interval = 1000;

            //Timer.Elapsed += (sender, args) =>
            //{
            //    Console.WriteLine(Ticks);
            //    Ticks = 0;
            //};

            //Timer.Start();
        }

        public override void Dispose()
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
        public override void SendPixelFrame(IPixelFrame opcPixelFrame)
        {
            //var byteArray = ((OPCPixelFrame)opcPixelFrame).ToByteArray();
            ////var byteArrayString = DateTime.Now.ToLongTimeString() + ":" + "Sending {";
            ////byteArray.ToList().ForEach(x => byteArrayString += x + ",");
            ////byteArrayString += "}";
            ////Debug.Print(byteArrayString);
            //AssertInit();
            //if (WebSocket.ReadyState == WebSocketState.Closed)
            //    Connect();
            //WebSocket.Send(byteArray); //TODO: Change this to async?






            var byteArray = ((OPCPixelFrame)opcPixelFrame).Data;
            //var byteArrayString = DateTime.Now.ToLongTimeString() + ":" + "Sending {";
            //byteArray.ToList().ForEach(x => byteArrayString += x + ",");
            //byteArrayString += "}";
            //Debug.Print(byteArrayString);
            //AssertInit();
            if (WebSocket.ReadyState == WebSocketState.Closed)
                Connect();
            
            WebSocket.Send(byteArray.ToArray()); //TODO: Change this to async?
            Thread.Sleep(Config.GetAsInt("NodeMCUWIFIThreadSleepTime"));
            //Ticks++;
        }


        /// <summary>
        /// Sends a list of LEDs to the connected FadeCandy board.
        /// </summary>
        public override void SendLEDs(IList<ILightingControllerPixel> leds)
        {
            OPCPixelFrame.CreateChannelBurstFromOPCPixels(leds.Cast<IOPCPixelContainer>().ToList()).ToList().ForEach(SendPixelFrame);
        }

        #endregion
    }
}