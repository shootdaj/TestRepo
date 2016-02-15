using System.Collections.Generic;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace WebController.Controllers
{
	public interface IZLMRPC
	{
		void DisposeZLM();
		void Notify(string colorString, int? time, int? cycles);
		void Save();
		void SetZoneColor(string zoneName, string color, float brightness);
		void StopZone(string zoneName);
		void CreateZLM();
		void DisposeProgramSets();
		List<string> GetZoneNames();
		void StartProgramSet(string programSetName, string programName, List<string> zoneNames, ISV isv);
		void StopProgramSet(string programSetName);
	}
}