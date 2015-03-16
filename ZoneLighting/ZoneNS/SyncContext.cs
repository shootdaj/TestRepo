using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.ZoneNS
{
	/// <summary>
	/// A wrapper on top of Barrier to have easier manageability. This class is used to synchronize programs
	/// by providing a common barrier, and managing the programs around that Barrier. This class is sealed because
	/// overriding any functionality will most likely break many things in here because a lot of parts are in motion.
	/// </summary>
	public sealed class SyncContext : IDisposable
	{
		#region CORE

		/// <summary>
		/// A name for convenience.
		/// </summary>
		public string Name { get; private set; }

		private object _barrierLock = new object();
		private Barrier _barrier = new Barrier(0);
		/// <summary>
		/// Underlying barrier that synchronizes the programs that are attached to this SyncContext.
		/// </summary>
		private Barrier Barrier
		{
			get
			{
				lock (_barrierLock)
					return _barrier;
			}
		}

		private object _zoneProgramsLock = new object();
		private List<ZoneProgram> _zonePrograms = new List<ZoneProgram>();
		/// <summary>
		/// ZonePrograms that are synchronized using this SyncContext.
		/// </summary>
		private List<ZoneProgram> ZonePrograms
		{
			get
			{
				lock (_zoneProgramsLock)
					return _zonePrograms;
			}
		}

		public object SyncStateRequestLock { get; set; } = new object();
		public bool IsSyncStateRequested { get; set; }

		#endregion

		#region C+I

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="name">Name of synchronization context for handy referencing.</param>
		public SyncContext(string name = null)
		{
			if (name != null)
				Name = name + "SyncContext";
		}
		
		public void Dispose()
		{
			ZonePrograms.Clear();
			Barrier.Dispose();
			Name = null;
		}

		#endregion

		#region API

		public void Sync(params ZoneProgram[] zonePrograms)
		{
			Sync(zonePrograms.ToList());
		}

		/// <summary>
		/// Synchronizes the given programs with the programs that are already attached to this context.
		/// This can be called while the other programs are running, but will wait until they can get into
		/// their synchronizable states before executing the synchronization. If no programs are already attached,
		/// then this method attaches the given program(s) to this context and starts them.
		/// </summary>
		public void Sync(IEnumerable<ZoneProgram> zonePrograms)
		{
			var zoneProgramsEnumerated = zonePrograms as IList<ZoneProgram> ?? zonePrograms.ToList();

			//incoming program must be stopped
			if (zoneProgramsEnumerated.Any(zp => zp.State != ProgramState.Stopped))
				throw new Exception("Given program must be stopped before a live sync is executed.");

			if (zoneProgramsEnumerated.All(zp => zp is ReactiveZoneProgram))
			{
				zoneProgramsEnumerated.ToList().ForEach(zoneProgram =>
				{
					Barrier.AddParticipant(); //add participant for each program
					ZonePrograms.Add(zoneProgram);
				});
			}
			else if (zoneProgramsEnumerated.All(zp => zp is LoopingZoneProgram) && ZonePrograms.All(zp => zp is LoopingZoneProgram))
			{
				//send sync-state request
				
				IsSyncStateRequested = true;

				//request sync-state from existing programs and incoming programs
				ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
				{
					zp.RequestSyncState();
				});
				zoneProgramsEnumerated.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
				{
					zp.RequestSyncState();
				});

				//start all incoming programs
				zoneProgramsEnumerated.ToList().ForEach(zoneProgram => zoneProgram.Start(sync: false));

				//wait for sync-state from all programs (incoming and existing)
				ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
				{
					zp.IsSynchronizable.WaitForFire();
				});
				zoneProgramsEnumerated.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
				{
					zp.IsSynchronizable.WaitForFire();
				});

				//sync all incoming programs
				zoneProgramsEnumerated.ToList().ForEach(zoneProgram =>
				{
					Barrier.AddParticipant();
					zoneProgram.SetSyncContext(this);
					ZonePrograms.Add(zoneProgram);
				});

				//release all programs from sync-state
				ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
				{
					zp.WaitForSync.Fire(null, null);
				});

				//wait until all programs have left sync-state
				ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
				{
					zp.LeftSyncTrigger.WaitForFire();
				});
			}
			else
			{
				throw new Exception(
					"All programs must be of the same type and must be included in the if statement that precedes this exception.");
			}
		}

		/// <summary>
		/// Removes a given program from the synchronization.
		/// </summary>
		public void Unsync(ZoneProgram program)
		{
			if (!ZonePrograms.Contains(program)) return;
			Barrier.RemoveParticipant();
			ZonePrograms.Remove(program);
		}

		//public void AddParticipant(ZoneProgram program)
		//{
		//	if (ZonePrograms.Contains(program)) return;
		//	Barrier.AddParticipant();
		//	ZonePrograms.Add(program);
		//}

		/// <summary>
		/// Signals the barrier and waits for the other programs to catch up or if it's the last program,
		/// then propels all the programs. To be used by programs to signal the other programs that signalling
		/// program is now waiting on the rest.
		/// </summary>
		public void SignalAndWait()
		{
			if (ZonePrograms.ToList().Any())
				Barrier.SignalAndWait();
		}

		public void Reset()
		{
			for (int i = 0; i < Barrier.ParticipantsRemaining; i++)
			{
				Task.Run(() => Barrier.SignalAndWait());
			}
		}

		public int GetNumberOfRemainingParticipants() => Barrier.ParticipantsRemaining;

		public int GetNumberOfTotalParticipants() => Barrier.ParticipantCount;

		#endregion
	}
}
