using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Communication;

namespace ZoneLighting
{
	public class LED : ILight, IFadeCandyPixel
	{
		#region CORE

		private Color _color;

		#region Color Parts

		public byte Red { get { return _color.R; } }
		public byte Green { get { return _color.G; } }
		public byte Blue { get { return _color.B; } }
		public byte Alpha { get { return _color.A; } }

		/// <summary>
		/// The hue, in degrees, of the underlying System.Drawing.Color. 
		/// The hue is measured in degrees, ranging from 0.0 through 360.0, in HSB color space.
		/// </summary>
		public float Hue { get { return _color.GetHue(); } }
		
		/// <summary>
		/// The saturation of the underlying System.Drawing.Color.
		/// The saturation ranges from 0.0 through 1.0, where 0.0 is grayscale and 1.0 is the most saturated.
		/// </summary>
		public float Saturation { get { return _color.GetSaturation(); } }

		/// <summary>
		/// The brightness of the underlying System.Drawing.Color. 
		/// The brightness ranges from 0.0 through 1.0, where 0.0 represents black and 1.0 represents white.
		/// </summary>
		public float Brightness { get { return _color.GetBrightness(); } }

		#endregion

		#endregion

		#region C+I

		public LED(Color? color = null)
		{
			if (color != null)
				SetColor((Color) color);
		}

		#endregion

		#region API

		public bool SetColor(Color color)
		{
			_color = color;
			return true;
		}

		#endregion
	}
}
