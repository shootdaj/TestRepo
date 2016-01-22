using System.Collections.Generic;
using System.Web.Mvc;
using WebController.ContainerNS;
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
			if (ZLMRPC.ZLM?.Zones != null)
				ZLMRPC.ZLMAction(zlm => AvailableZones = new SelectList(zlm.Zones, "Name"));
		}

		public IZLM ZLM => Container.ZLM;

		public Zone Zone { get; set; }

		public BetterList<ProgramSet> ProgramSets => Container.ZLM.ProgramSets;

		public SelectList AvailableZones { get; set; }

		public IEnumerable<string> AvailablePrograms => Container.ZLM.AvailablePrograms;

	}
}