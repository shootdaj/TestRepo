using Refigure;

namespace OPCWebSocketController
{
    public class NodeMCUController : OPCWebSocketController
    {

        #region Singleton

        private static NodeMCUController _instance;

        public static NodeMCUController Instance
            => _instance ?? (_instance = new NodeMCUController(Config.Get("NodeMCUServerURL"),
                   new DefaultPixelMapper(), OPCPixelType.OPCRGBPixel, 1)); //TODO: Change channel - make the whole thing IoC'd

        #endregion

        public NodeMCUController(string serverURL, IPixelToOPCPixelMapper pixelMapper, OPCPixelType opcPixelType, byte channel) : base(serverURL, pixelMapper, opcPixelType, channel)
        {
        }

        public new void Initialize()
        {
            if (!Initialized)
            {
                base.Initialize();
                Initialized = true;
            }
        }

        public new void Uninitialize()
        {
            if (Initialized)
            {
                base.Uninitialize();
                Initialized = false;
            }
        }
    }
}
