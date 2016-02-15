using System.Collections.Generic;
using System.Web.Mvc;
using WebController.Controllers;
using WebController.IoC;
using ZoneLighting;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace WebController.Models
{
	public class ZLMViewModel
	{
		private IZLMRPC ZLMRPC { get; set; }

		public ZLMViewModel(IZLMRPC zlmrpc)
		{
			ZLMRPC = zlmrpc;

			var zones = ZLMRPC.GetZoneNames();

			if (zones != null)
				AvailableZones = new SelectList(zones);
		}

		public IZLM ZLM => Container.ZLM;

		public Zone Zone { get; set; }

		public BetterList<ProgramSet> ProgramSets => Container.ZLM.ProgramSets;

		public SelectList AvailableZones { get; set; }

		public IEnumerable<string> AvailablePrograms => Container.ZLM.AvailablePrograms;

	}
}