using System.Collections.Generic;
using Anshul.Utilities;
using ZoneLighting.Usables;

namespace WebRemote.Models
{
	public class ZLMJsonModel
	{
		public BetterList<ZoneJsonModel> Zones { get; set; }

		public BetterList<ZoneJsonModel> AvailableZones { get; set; }

		public IEnumerable<string> AvailablePrograms { get; set; }

		public BetterList<ProgramSetJsonModel> ProgramSets { get; set; }
	}
}