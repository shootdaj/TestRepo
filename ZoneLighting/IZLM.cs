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

		void Dispose();

		/// <summary>
		/// Adds a fade candy zone to the manager.
		/// </summary>
		/// <param name="name">Name of the zone</param>
		/// <param name="pixelType">Type of pixel for the zone</param>
		/// <param name="numberOfLights">Number of lights in the zone</param>
		/// <param name="channel">FadeCandy channel on which this zone is connected</param>
		/// <returns></returns>
		Zone AddFadeCandyZone(string name, int numberOfLights);

		/// <summary>
		/// Saves active program sets and zones
		/// </summary>
		void Save();

		/// <summary>
		/// Creates a ProgramSet
		/// </summary>
		/// <param name="programSetName">Name of program set</param>
		/// <param name="programName">Name of program</param>
		/// <param name="sync">Whether or not to start the programs in sync</param>
		/// <param name="isv">Input starting values - starting values for the inputs</param>
		/// <param name="zones">Zones to run the program set on</param>
		/// <param name="startingParameters">Starting parameters for creating this program set. These will be fed to the constructor(s) of the ZoneProgram(s).</param>
		ProgramSet CreateProgramSet(string programSetName, string programName, bool sync, ISV isv,
			IEnumerable<Zone> zones, dynamic startingParameters = null);

		/// <summary>
		/// Creates a ProgramSet with one program instance
		/// </summary>
		ProgramSet CreateSingularProgramSet(string programSetName, ZoneProgram program, ISV isv, Zone zone, dynamic startingParameters = null);

		void RecreateProgramSet(string programSetName, string programName, List<string> zoneNames, ISV isv);
		void DisposeProgramSets(List<string> programSetNames = null, bool force = false);
		void StopZone(string zoneName, bool force);
		void SetZoneInputs(string zoneName, ISV isv);
		void SetZoneColor(string zoneName, string color, float brightness);
		string GetZoneSummary();
		void StopZones();
		void DisposeZones();
		void SaveProgramSets(string filename = null);
		void SaveZones(string filename = null);
		void SetProgramSetInputs(string programSetName, ISV isv);

		ProgramSet CreateProgramSet(string programSetName, string programName, IEnumerable<string> zoneNames, bool sync = true,
			ISV isv = null, dynamic startingParameters = null);

		
		/// <summary>
		/// Removes the given zone from the given program set and stops the zone.
		///TODO: This is a very unelegant and stupid way of removing zones (basically disposing and recreating the program set without the zone)
		/// TODO: What needs to happen is Unsync, which is currently broken for 3 or more programs. This needs to be resolved eventually. 
		/// </summary>
		void RecreateProgramSetWithoutZone(string programSetName, string zoneName, bool force = false);

		void SetLightColor(string zoneName, string color, int index, float brightness = 1);

		void SetAllZonesColor(string color, float brightness = 1);
	}
}