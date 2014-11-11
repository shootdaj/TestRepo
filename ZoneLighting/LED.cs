using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Communication;

namespace ZoneLighting
{
	/// <summary>
	/// http://www.codeproject.com/Articles/10072/Simulated-Multiple-Inheritance-Pattern-for-C
	/// </summary>
	public class LED : ILogicalRGBLight
	{
		#region CORE

		private Color _color;
		public FadeCandyPixel FadeCandyPixel { get; set; }

		#region Color Parts

		#region ARGB

		public byte Red { get { return _color.R; } }
		public byte Green { get { return _color.G; } }
		public byte Blue { get { return _color.B; } }
		public byte Alpha { get { return _color.A; } }

		#endregion

		#region HSB
		
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

		#endregion

		#region C+I

		public LED(Color? color = null, int? logicalIndex = null, byte? fadeCandyChannel = null, int? fadeCandyIndex = null)
		{
			FadeCandyPixel = new FadeCandyPixel();
			if (color != null)
				SetColor((Color) color);
			if (logicalIndex != null)
				LogicalIndex = (int)logicalIndex;
			if (fadeCandyChannel != null)
				FadeCandyPixel.Channel = (byte)fadeCandyChannel;
			if (fadeCandyIndex != null)
				FadeCandyPixel.PhysicalIndex = (int)fadeCandyIndex;
		}

		#endregion

		#region ILogicalRGBLight

		public int LogicalIndex { get; set; }

		#endregion
		
		#region API

		public void MapToFadeCandyPixel(byte channel, int index)
		{
			FadeCandyPixel.Channel = channel;
			FadeCandyPixel.PhysicalIndex = index;
		}

		public bool SetColor(Color color)
		{
			_color = color;
			return true;
		}

		public Color GetColor()
		{
			return _color;
		}

		#endregion
	}
}
