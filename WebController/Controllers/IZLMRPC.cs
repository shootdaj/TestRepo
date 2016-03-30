using System.Collections.Generic;
using WebController.Models;
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
		void SetZoneColor(string zoneName, string color, float brightness = 1);
		void StopZone(string zoneName, bool force);
		void CreateZLM();
		void DisposeProgramSets();
		List<ZoneJsonModel> GetZones();
		//void RestartProgramSet(string programSetName, string programName, List<string> zoneNames, ISV isv);
		void StopProgramSet(string programSetName);
		void SetInputs(string zoneName, ISV isv);
		string GetZoneSummary();
		void SetProgramSetInputs(string programSetName, ISV isv);
		void StartProgramSet(string programSetName);
		void RecreateProgramSet(string programSetName, string programName, List<string> zoneNames, ISV isv);

		ProgramSetJsonModel CreateProgramSet(string programSetName, string programName, IEnumerable<string> zoneNames, bool sync = true, ISV isv = null, dynamic startingParameters = null);

		//List<Zone> GetZones();
		//List<Zone> GetAvailableZones();
		void DisposeProgramSet(string programSetName);
		string GetStatus();
		void RecreateProgramSetWithoutZone(string programSetName, string zoneName, bool force = false);
		void SetColor(string zoneName, string color, int index, double? brightness = 1);
	}
}