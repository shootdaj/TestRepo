namespace ZoneLighting.Communication
{
    public interface IPixelToOPCPixelIndexMapper
    {
        int GetOPCPixelIndex(int pixelIndex);
    }
}