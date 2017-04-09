namespace ZoneLighting.Communication
{
	public interface IOPCPixelContainer : ILightingControllerPixel
	{
		OPCPixel OPCPixel { get; set; }
	}
}