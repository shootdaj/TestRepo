using System.ComponentModel.Composition;
using ZoneLighting.MEF;

namespace OPCWebSocketController
{
	[Export(typeof(ILightingController))]
	[ExportMetadata("Name", "NodeMCUController")]
	public class NodeMCUController : OPCWebSocketController
    {
        [ImportingConstructor]
        public NodeMCUController()
        {
        }

        public override void Initialize(dynamic parameters)
        {
            //base.Initialize((string) parameters.Name, (string) parameters.ServerURL,
            //    (IPixelToOPCPixelMapper) parameters.PixelMapper, (OPCPixelType) parameters.OPCPixelType,
            //    (byte) parameters.Channel);
            base.Initialize("NodeMCUController1", "ws://192.168.29.113:81/", new DefaultPixelMapper(), OPCPixelType.OPCRGBPixel, 1);
            Initialized = true;
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
