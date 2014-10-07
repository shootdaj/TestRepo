using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoneLighting
{
	/// <summary>
	/// Represents a zone (room or whatever) that contains the lights to be controlled.
	/// </summary>
    public class Zone
    {
		/// <summary>
		/// Zones can contain other zones in a recursive fashion.
		/// </summary>
		public List<Zone> Zones { get; set; }

		/// <summary>
		/// All lights in the area.
		/// </summary>
		public List<ILight> Lights { get; set; }
    }
}
