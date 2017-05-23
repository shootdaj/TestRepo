using ZoneLighting.MEF;

namespace OPCWebSocketController
{
    public interface IPixelToOPCPixelMapper
    {
        int GetOPCPixelIndex(int pixelIndex);
	    byte GetOPCPixelChannel(IPixel pixel);
    }
}