namespace ZoneLighting.Communication
{
    public interface IPixelToOPCPixelMapper
    {
        int GetOPCPixelIndex(int pixelIndex);
	    byte GetOPCPixelChannel(IPixel pixel);
    }
}