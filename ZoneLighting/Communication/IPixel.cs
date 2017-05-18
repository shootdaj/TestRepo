using System.Drawing;

namespace ZoneLighting.Communication
{
	public interface IPixel
	{
		Color Color { get; set; }
	    int Index { get; set; }
    }
}