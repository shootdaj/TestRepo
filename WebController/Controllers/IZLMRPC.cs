using ZoneLighting.ZoneProgramNS;

namespace WebController.Controllers
{
	public interface IZLMRPC
	{
		void DisposeZLM();
		void Notify(string colorString, int? time, int? cycles);
		void ProcessZLMCommand(string command, string programSetName, string programName, ISV isv);
		void Save();
		void SetZoneColor(string zoneName, string color, float brightness);
		void StopZone(string zoneName);
		void CreateZLMInstance();
	}
}