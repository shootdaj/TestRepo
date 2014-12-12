using System.Drawing;

namespace ZoneLighting.Communication
{
	public interface IFadeCandyPixel : ILightingControllerPixel
	{
		FadeCandyPixel FadeCandyPixel { get; set; }
	}
}