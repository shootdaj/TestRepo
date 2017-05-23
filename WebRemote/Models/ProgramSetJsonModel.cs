using System.Collections.Generic;
using Anshul.Utilities;
using ZoneLighting.Usables;
using ZoneLighting.ZoneProgramNS;

namespace WebRemote.Models
{
	public class ProgramSetJsonModel : IBetterListType
	{
		public string Name { get; set; }
		
		public List<ZoneJsonModel> Zones { get; set; }

		public string ProgramName { get; set; }
		
		public bool Sync { get; set; }

		public ProgramState State { get; set; }
	}
}