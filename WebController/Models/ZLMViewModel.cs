using System.Collections.Generic;
using System.Web.Mvc;
using WebController.Container;
using WebController.Controllers;
using ZoneLighting;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace WebController.Models
{
	public class ZLMViewModel
	{
		//public ZoneLightingManager ZLM => ZoneLightingManager.Instance;

		public ZLMViewModel()
		{
			if (ZLMContainer.Instance?.Zones != null)
				ZLMController.ZLMAction(zlm => AvailableZones = new SelectList(zlm.Zones, "Name"));
		}

		public ZLM ZLM => ZLMContainer.Instance;

		public Zone Zone { get; set; }

		public BetterList<ProgramSet> ProgramSets => ZLMContainer.Instance.ProgramSets;

		public SelectList AvailableZones { get; set; }

		public IEnumerable<string> AvailablePrograms => ZLMContainer.Instance.AvailablePrograms;

	}
}