namespace ZoneLighting.Communication
{
    public class OneToOnePixelIndexMapper : IPixelToOPCPixelIndexMapper
    {
        public int GetOPCPixelIndex(int pixelIndex)
        {
            return pixelIndex;
        }
    }
}