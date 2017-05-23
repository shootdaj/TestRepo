using System.Collections.Generic;
using Anshul.Utilities;
using WebRemote.IoC;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace WebRemote.Models
{
	public class ProgramSetVM
	{
		public BetterList<ProgramSet> ProgramSets => Container.ZLM.ProgramSets;

		public BetterList<Zone> AvailableZones => Container.ZLM.AvailableZones;

		public IEnumerable<string> AvailablePrograms => Container.ZLM.AvailablePrograms;
	}
}