using System.Drawing;
using System.Runtime.Serialization;
using ZoneLighting.Communication;

namespace ZoneLighting
{
    /// <summary>
    /// Represents an LED. This class must implement the ILightingControllerPixel for each type of lighting controller
    /// that it needs to be output on (for example is currently implements IOPCPixelContainer which inherits from ILightingControllerPixel).
    /// </summary>
    [DataContract]
    public class LED : ILogicalRGBLight, IOPCPixelContainer
	{
		#region CORE

        public Color Color { get; set; }

        [DataMember]
		public OPCPixel OPCPixel { get; set; }

		#region Color Parts

		#region HSB
		
		/// <summary>
		/// The hue, in degrees, of the underlying System.Drawing.Color. 
		/// The hue is measured in degrees, ranging from 0.0 through 360.0, in HSB color space.
		/// </summary>
		public float Hue => Color.GetHue();

		/// <summary>
		/// The saturation of the underlying System.Drawing.Color.
		/// The saturation ranges from 0.0 through 1.0, where 0.0 is grayscale and 1.0 is the most saturated.
		/// </summary>
		public float Saturation => Color.GetSaturation();

		/// <summary>
		/// The brightness of the underlying System.Drawing.Color. 
		/// The brightness ranges from 0.0 through 1.0, where 0.0 represents black and 1.0 represents white.
		/// </summary>
		public float Brightness => Color.GetBrightness();

		#endregion

		#endregion

		#endregion

		#region C+I

		public LED(Color? color = null, int? logicalIndex = null, byte? fadeCandyChannel = null, int? fadeCandyIndex = null, OPCPixelType pixelType = OPCPixelType.None)
		{
			OPCPixel = GetOPCPixelInstance(pixelType);

			if (color != null)
				SetColor((Color) color);
			if (logicalIndex != null)
				LogicalIndex = (int)logicalIndex;
			if (fadeCandyChannel != null)
				OPCPixel.Channel = (byte)fadeCandyChannel;
			if (fadeCandyIndex != null)
				OPCPixel.PhysicalIndex = (int)fadeCandyIndex;
		}

		#endregion

		#region ILogicalRGBLight

        [DataMember]
		public int LogicalIndex { get; set; }

		#endregion

		#region API
        
		public static OPCPixel GetOPCPixelInstance(OPCPixelType pixelType)
		{
			switch (pixelType)
			{
				case OPCPixelType.OPCRBGPixel:
					return new OPCRBGPixel();
				case OPCPixelType.OPCRGBPixel:
					return new OPCRGBPixel();
			}

			return null;
		}

		public void SetOPCPixel(byte channel, int index)
		{
			OPCPixel.Channel = channel;
			OPCPixel.PhysicalIndex = index;
		}

		public bool SetColor(Color color)
		{
			Color = color;
			return true;
		}

		public Color GetColor()
		{
			return Color;
		}

		#endregion
	}
}
