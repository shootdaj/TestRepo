using System.Drawing;

namespace ZoneLighting
{
	public interface ILogicalRGBLight
	{
		bool SetColor(Color color);
		int LogicalIndex { get; set; }
		Color GetColor();
	}
}
