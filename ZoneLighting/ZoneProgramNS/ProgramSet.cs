using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Usables;
using ZoneLighting.Usables.TestInterfaces;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLighting.ZoneProgramNS
{
	/// <summary>
	/// A logical grouping of programs that will be run on the zones passed into the constructor. All programs can either
	/// be started with or without sync. 
	/// </summary>
	public class ProgramSet : IDisposable, IBetterListType, ITestProgramSet
	{
		public string Name { get; private set; }
		public SyncContext SyncContext { get; } = new SyncContext();
		public List<Zone> Zones { get; private set; }
		public string ProgramName { get; private set; }

		private List<ZoneProgram> ZonePrograms
		{
			get { return Zones.Select(zone => zone.ZoneProgram).ToList(); }
		}

		IEnumerable<ZoneProgram> ITestProgramSet.ZoneProgramsTest => ZonePrograms;

		public ProgramSet(string programName, IEnumerable<Zone> zones, bool sync, ISV isv, string setName)
		{
			if (!ZoneScaffolder.Instance.DoesProgramExist(programName))
				throw new Exception(string.Format("No program by the name '{0}' exists.", programName));

			Name = setName;
			Zones = zones.ToList();
			ProgramName = programName;

			if (sync)
			{
				Zones.ForEach(zone =>
				{
					zone.Uninitialize();
					ZoneScaffolder.Instance.InitializeZone(zone, programName, isv, true, SyncContext, true);
				});
				
				SyncContext.Sync(Zones);
			}
			else
			{
				Zones.ForEach(zone =>
				{
					zone.Uninitialize();
					ZoneScaffolder.Instance.InitializeZone(zone, programName, isv);
				});
			}
		}

		public void Dispose()
		{
			Dispose(false);
		}

		public void Dispose(bool force)
		{
			Name = null;
			Zones.ForEach(zone => zone.Uninitialize(force));
			Zones = null;
			ProgramName = null;
		}

		public void StartAllPrograms()
		{
			if (SyncContext == null)
				ZonePrograms.ForEach(zp => zp.Start());
			else
				SyncContext.Sync(ZonePrograms);
		}

		public void StopAllPrograms(bool force = false)
		{
			ZonePrograms.ForEach(zp => zp.Stop(force));
		}
	}
}
