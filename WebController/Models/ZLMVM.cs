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
	public class ZLMVM
	{
		//public ZoneLightingManager ZLM => ZoneLightingManager.Instance;

		public ZLMVM()
		{
			//if (ZLMContainer.Instance?.Zones != null)
			//	ZLMController.ZLMAction(zlm => AvailableZones = new SelectList(zlm.Zones, "Name"));
		}

		//public ZLM ZLM => ZLMContainer.Instance;

		public BetterList<ProgramSet> ProgramSets { get; set; } // => ZLMContainer.Instance.ProgramSets;

		public SelectList AvailableZones { get; set; }

		public IEnumerable<string> AvailablePrograms { get; set; } //=> ZLMContainer.Instance.AvailablePrograms;

	}
}