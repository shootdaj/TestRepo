using System.Drawing;

namespace ZoneLighting.Communication
{
	public interface IFadeCandyPixelContainer : ILightingControllerPixel
	{
		FadeCandyPixel FadeCandyPixel { get; set; }
	}
}