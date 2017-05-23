using System.Drawing;

namespace ZoneLighting.MEF
{
	public interface IPixel
	{
		Color Color { get; set; }
	    int Index { get; set; }
    }
}