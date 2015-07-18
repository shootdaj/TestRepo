using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

		public SyncContext SyncContext { get; private set; }

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

		public ProgramState State
		{
			get
			{
				var previousState = ZonePrograms.First().State;
				if (ZonePrograms.Any(zoneProgram => zoneProgram.State != previousState))
				{
					throw new Exception("Program Set in inconsistent state. Some programs are started while others are not.");
				}
				return previousState;
			}
		}

		public ProgramSet(string programName, IEnumerable<Zone> zones, bool sync, ISV isv, string name)
		{
			if (!ZoneScaffolder.Instance.DoesProgramExist(programName))
				throw new Exception($"No program by the name '{programName}' exists.");

			Name = name;
			Zones = zones.ToList();
			ProgramName = programName;
		    Sync = sync;

			if (Sync)
			{
				Zones.ForEach(zone =>
				{
					zone.Stop(true);
					ZoneScaffolder.Instance.RunZone(zone, programName, null, true, SyncContext, true);
				});

				SyncContext = new SyncContext();
				SyncContext.Sync(Zones, isv: isv);
			}
			else
			{
				Zones.ForEach(zone =>
				{
					zone.Stop(true);
					ZoneScaffolder.Instance.RunZone(zone, programName, isv);
				});
			}
		}

		[JsonConstructor]
		public ProgramSet(string programName, IEnumerable<Zone> zones, bool sync, string name)
		{
			if (!ZoneScaffolder.Instance.DoesProgramExist(programName))
				throw new Exception($"No program by the name '{programName}' exists.");

			Name = name;
			Zones = zones.ToList();
			ProgramName = programName;
			Sync = sync;
		}

		/// <summary>
		/// Creates a program set with a single program on a single zone.
		/// </summary>
		public ProgramSet(ZoneProgram program, Zone zone, ISV isv, string setName)
		{
			Name = setName;
			ProgramName = program.Name;
			Sync = false;

			zone.Stop(true);
			//ZoneScaffolder.Instance.RunZone(zone, "", isv);
			zone.Run(program, isv);
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
			if (Zones != null)
				Parallel.ForEach(Zones, zone => zone.Stop(force));
			//Zones.ForEach(zone => zone.Stop(force));
			Zones = null;
			ProgramName = null;
			SyncContext?.Dispose();
			SyncContext = null;
		}

		public void Start()
		{
			if (SyncContext == null)
				ZonePrograms.ForEach(zp => zp.Start());
			else
				SyncContext.Sync(ZonePrograms);
		}

		public void Stop(bool force = false)
		{
			Parallel.ForEach(ZonePrograms, zp => zp.Stop(force));
		}
	}
}
