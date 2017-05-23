using System.Collections.Generic;
using Anshul.Utilities;
using ZoneLighting.Usables;
using ZoneLighting.ZoneProgramNS;

namespace WebRemote.Models
{
	public class ZoneJsonModel : IBetterListType
	{
		public string Name { get; set; }
		
		public string ZoneProgramName { get; set; }
		
		public double Brightness { get; set; }

		public bool Running { get; set; }

		public int LightCount { get; set; }

		public ISV Inputs { get; set; }
	}
}