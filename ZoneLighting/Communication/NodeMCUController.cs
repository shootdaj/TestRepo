using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoneLighting.Communication
{
    public class NodeMCUController : OPCWebSocketController
    {

        #region Singleton

        private static NodeMCUController _instance;

        public static NodeMCUController Instance
            => _instance ?? (_instance = new NodeMCUController(ConfigurationManager.AppSettings["NodeMCUServerURL"],
                   new OneToOnePixelIndexMapper(), OPCPixelType.OPCRGBPixel));

        #endregion

        public NodeMCUController(string serverURL, IPixelToOPCPixelIndexMapper pixelIndexMapper, OPCPixelType opcPixelType) : base(serverURL, pixelIndexMapper, opcPixelType)
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
