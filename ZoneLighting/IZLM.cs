using System.Collections.Generic;
using ZoneLighting.Communication;
using ZoneLighting.Usables;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting
{
	public interface IZLM
	{
		IEnumerable<string> AvailablePrograms { get; }
		BetterList<Zone> AvailableZones { get; }
		BetterList<ProgramSet> ProgramSets { get; }
		BetterList<Zone> Zones { get; }

		Zone AddFadeCandyZone(string name, PixelType pixelType, int numberOfLights, int channel);
		ProgramSet CreateProgramSet(string programSetName, string programName, bool sync, ISV isv, IEnumerable<Zone> zones, dynamic startingParameters = null);
		ProgramSet CreateSingularProgramSet(string programSetName, ZoneProgram program, ISV isv, Zone zone);
		void Dispose();
		void DisposeProgramSets(params string[] programSetNames);
		string GetZoneSummary();
		void SaveProgramSets(string filename = null);
		void SaveZones(string filename = null);
	}
}