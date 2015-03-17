using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZoneLighting.TriggerDependencyNS;
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
		
		/// <summary>
		/// Underlying barrier that synchronizes the programs that are attached to this SyncContext.
		/// </summary>
		private Barrier Barrier { get; } = new Barrier(0);

		/// <summary>
		/// ZonePrograms that are synchronized using this SyncContext.
		/// </summary>
		private List<ZoneProgram> ZonePrograms { get; } = new List<ZoneProgram>();

		public object SyncStateLock { get; } = new object();

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
					lock (Barrier)
					{
						Barrier.AddParticipant(); //add participant for each program
					}

					lock (ZoneProgramsLock)
					{
						ZonePrograms.Add(zoneProgram);
					}
				});
			}
			else if (zoneProgramsEnumerated.All(zp => zp is LoopingZoneProgram) && ZonePrograms.All(zp => zp is LoopingZoneProgram))
			{
				lock (SyncStateLock)
				{
					//IsSyncStateRequested = true;

					ProgramsToSync.AddRange(zoneProgramsEnumerated);
					ProgramsToSync.ForEach(zoneProgram =>
					{
						zoneProgram.SetSyncContext(this);
						lock (ZoneProgramsLock)
						{
							ZonePrograms.Add(zoneProgram);
						}
					});

					zoneProgramsEnumerated.ToList().ForEach(zoneProgram => zoneProgram.Start(sync: false));
				}

				////wait for sync-state from all programs (incoming and existing)
				//ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
				//{
				//	zp.IsSynchronizable.WaitForFire();
				//});
				//zoneProgramsEnumerated.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
				//{
				//	zp.IsSynchronizable.WaitForFire();
				//});

				////sync all incoming programs
				//zoneProgramsEnumerated.ToList().ForEach(zoneProgram =>
				//{
				//	Barrier.AddParticipant();
				//	zoneProgram.SetSyncContext(this);
				//	ZonePrograms.Add(zoneProgram);
				//});

				////release all programs from sync-state
				//ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
				//{
				//	zp.WaitForSync.Fire(null, null);
				//});

				////wait until all programs have left sync-state
				//ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
				//{
				//	zp.LeftSyncTrigger.WaitForFire();
				//});
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
			lock (ZoneProgramsLock)
			{
				if (!ZonePrograms.Contains(program)) return;
			}
			lock (Barrier)
			{
				Barrier.RemoveParticipant();
			}
			lock (ZoneProgramsLock)
			{
				ZonePrograms.Remove(program);
			}
		}

		/// <summary>
		/// Signals the barrier and waits for the other programs to catch up or if it's the last program,
		/// then propels all the programs. To be used by programs to signal the other programs that signalling
		/// program is now waiting on the rest.
		/// </summary>
		public void SignalAndWait()
		{
			if (Barrier.ParticipantCount > 0)
				Barrier.SignalAndWait();
		}

		public void NextMove(ZoneProgram program)
		{
			lock (SyncStateLock)
			{
				DebugTools.AddEvent("SyncContext.NextMove", "SyncStateRequestedLock acquired: " + program.Name);
				WaitingPrograms.Add(program);
				if (ZonePrograms.All(zp => WaitingPrograms.Contains(zp)))
				{
					if (ProgramsToSync.Any())
					{
						DebugTools.AddEvent("SyncContext.NextMove", "Set context, add barrier, add to ZonePrograms: " + program.Name);

						ProgramsToSync.ToList().ForEach(zoneProgram =>
						{
							lock (Barrier)
							{
								Barrier.AddParticipant();
							}
						});

						ProgramsToSync.Clear();
					}

					WaitingPrograms.Clear();
					ZonePrograms.ForEach(zp => WaitForAllPrograms.Fire(this, null));
					SyncFinished.Fire(this, null);
				}
			}
			DebugTools.AddEvent("SyncContext.NextMove", "SyncStateRequestedLock released: " + program.Name);
		}

		private object ZoneProgramsLock { get; } = new object();

		private List<ZoneProgram> ProgramsToSync { get; set; } = new List<ZoneProgram>();

		public Trigger SyncFinished { get; set; } = new Trigger("SyncContext.SyncFinished");

		public Trigger WaitForAllPrograms { get; set; } = new Trigger("SyncContext.WaitForSync");

		private List<ZoneProgram> WaitingPrograms { get; set; } = new List<ZoneProgram>();

		public int GetNumberOfRemainingParticipants() => Barrier.ParticipantsRemaining;

		public int GetNumberOfTotalParticipants() => Barrier.ParticipantCount;

		#endregion
	}
}
