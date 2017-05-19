using ZoneLighting.Communication;

namespace OPCWebSocketController
{
    public interface IPixelToOPCPixelMapper
    {
        int GetOPCPixelIndex(int pixelIndex);
	    byte GetOPCPixelChannel(IPixel pixel);
    }
}