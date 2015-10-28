using System.Collections.Generic;
using WebController.Container;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace WebController.Models
{
	public class ProgramSetVM
	{
		public BetterList<ProgramSet> ProgramSets => ZLMContainer.Instance.ProgramSets;

		public BetterList<Zone> AvailableZones => ZLMContainer.Instance.AvailableZones;

		public IEnumerable<string> AvailablePrograms => ZLMContainer.Instance.AvailablePrograms;
	}
}