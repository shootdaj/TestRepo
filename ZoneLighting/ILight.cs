using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoneLighting
{
	public interface ILight
	{
		bool SetColor(Color color);
		int Index { get; }
		Color GetColor();
	}
}
