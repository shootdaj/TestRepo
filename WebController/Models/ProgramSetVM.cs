using System.Collections.Generic;
using WebController.Container;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace WebController.Models
{
	public class ProgramSetVM
	{

		public BetterList<Zone> AvailableZones { get; set; } //=> ZLMContainer.Instance.AvailableZones;

		public IEnumerable<string> AvailablePrograms { get; set; } //=> ZLMContainer.Instance.AvailablePrograms;
	}
}