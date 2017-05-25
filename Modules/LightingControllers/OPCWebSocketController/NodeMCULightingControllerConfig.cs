using System.ComponentModel.Composition;
using System.Dynamic;
using ZoneLighting.MEF;

namespace OPCWebSocketController
{
	[Export(typeof(ILightingControllerConfig))]
	[ExportMetadata("Name", "NodeMCUController")]
	public class NodeMCULightingControllerConfig : ILightingControllerConfig
    {
        public string ServerURL { get; set; }
        public string Name { get; set; } = "NodeMCUController1";


        public dynamic ToExpandoObject()
        {
            dynamic returnValue = new ExpandoObject();

            returnValue.Name = Name;
            returnValue.ServerURL = ServerURL;
            returnValue.PixelMapper = PixelMapper;
            returnValue.OPCPixelType = OPCPixelType.OPCRGBPixel;
            returnValue.Channel = (byte)1;

            return returnValue;
        }

        public IPixelToOPCPixelMapper PixelMapper { get; set; }

        public OPCPixelType PixelType { get; set; }
    }
}