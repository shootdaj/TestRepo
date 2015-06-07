using System.Runtime.Serialization;

namespace ZoneLighting.Communication
{
    [DataContract]
	public abstract class PhysicalRGBLight
	{
        [DataMember]
		public int PhysicalIndex { get; set; }
	}
}
