using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
	[DataContract]
	public class ProgramSet : IDisposable, IBetterListType, ITestProgramSet
	{
        [DataMember]
		public string Name { get; private set; }

		public SyncContext SyncContext { get; }

        [DataMember]
		public List<Zone> Zones { get; private set; }

        [DataMember]
		public string ProgramName { get; private set; }

		private List<ZoneProgram> ZonePrograms
		{
			get { return Zones.Select(zone => zone.ZoneProgram).ToList(); }
		}

        [DataMember]
	    public bool Sync { get; private set; }

		/// <summary>
		/// For testing only
		/// </summary>
		IEnumerable<ZoneProgram> ITestProgramSet.ZoneProgramsTest => ZonePrograms;

		public ProgramSet(string programName, IEnumerable<Zone> zones, bool sync, ISV isv, string setName)
		{
			if (!ZoneScaffolder.Instance.DoesProgramExist(programName))
				throw new Exception(string.Format("No program by the name '{0}' exists.", programName));

			Name = setName;
			Zones = zones.ToList();
			ProgramName = programName;
		    Sync = sync;

			if (Sync)
			{
				Zones.ForEach(zone =>
				{
					zone.Uninitialize(true);
					ZoneScaffolder.Instance.RunZone(zone, programName, isv, true, SyncContext, true);
				});

				SyncContext = new SyncContext();
				SyncContext.Sync(Zones);
			}
			else
			{
				Zones.ForEach(zone =>
				{
					zone.Uninitialize(true);
					ZoneScaffolder.Instance.RunZone(zone, programName, isv);
				});
			}
		}

		public void RemoveZone(Zone zone, bool force = true)
		{
			SyncContext?.Unsync(zone);
			Zones.Remove(zone);
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
