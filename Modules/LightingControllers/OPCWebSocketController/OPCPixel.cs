using System.Drawing;
using System.Runtime.Serialization;

namespace OPCWebSocketController
{
    /// <summary>
    /// Represents a pixel that can be controlled by the Open Pixel Control protocol.
    /// http://openpixelcontrol.org/
    /// </summary>
    [DataContract]
	public abstract class OPCPixel/* : PhysicalRGBLight*/
	{
		protected OPCPixel(byte channel, int physicalIndex)
		{
			Channel = channel;
			PhysicalIndex = physicalIndex;
		}

        [DataMember]
		public byte Channel { get; set; }
        
        public abstract int RedIndex { get; }
        
        public abstract int GreenIndex { get; }
        
        public abstract int BlueIndex { get; }

	    [DataMember]
	    public int PhysicalIndex { get; set; }

        public Color Color { get; set; }

	    public static OPCPixel GetOPCPixelInstance(OPCPixelType pixelType, byte channel, int index)
	    {
	        switch (pixelType)
	        {
	            case OPCPixelType.OPCRBGPixel:
	                return new OPCRBGPixel(channel, index);
	            case OPCPixelType.OPCRGBPixel:
	                return new OPCRGBPixel(channel, index);
	        }

	        return null;
	    }
    }

	public class OPCRGBPixel : OPCPixel
	{
		public override int RedIndex => PhysicalIndex * 3;
		public override int GreenIndex => PhysicalIndex * 3 + 1;
		public override int BlueIndex => PhysicalIndex * 3 + 2;

	    public OPCRGBPixel(byte channel, int physicalIndex) : base(channel, physicalIndex)
	    {
	    }
	}

	public class OPCRBGPixel : OPCPixel
	{
		public override int RedIndex => PhysicalIndex * 3;
		public override int GreenIndex => PhysicalIndex * 3 + 2;
		public override int BlueIndex => PhysicalIndex * 3 + 1;

	    public OPCRBGPixel(byte channel, int index) : base(channel, index)
	    {
	    }
	}
    
    public enum OPCPixelType
	{
		None,
		OPCRGBPixel,
		OPCRBGPixel,
	}
}