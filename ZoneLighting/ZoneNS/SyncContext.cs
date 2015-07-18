using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
		/// This will determine how many milliseconds the SignalAndWait call will wait until leaving.
		/// </summary>
		public int UniversalTimeout { get; set; } = 100; //todo: grab this from config?

		/// <summary>
		/// ZonePrograms that are synchronized using this SyncContext.
		/// </summary>
		private List<ZoneProgram> ZonePrograms { get; } = new List<ZoneProgram>();

		public object SyncLock { get; } = new object();

		public Trigger SyncFinished { get; set; } = new Trigger("SyncContext.SyncFinished");

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

		public void Sync(IEnumerable<Zone> zones, bool forceStop = false, IEnumerable<ISV> isv = null)
		{
			Sync(zones.Select(zone => zone.ZoneProgram), forceStop, isv);
		}

		public void Sync(params Zone[] zones)
		{
			Sync(zones.Select(zone => zone.ZoneProgram));
		}

		/// <summary>
		/// Synchronizes the given programs with the programs that are already attached to this context.
		/// This can be called while the other programs are running, but will wait until they can get into
		/// their synchronizable states before executing the synchronization. If no programs are already attached,
		/// then this method attaches the given program(s) to this context and starts them.
		/// </summary>
		public void Sync(IEnumerable<ZoneProgram> zonePrograms, bool forceStop = false, IEnumerable<ISV> isvs = null)
		{
			var incomingZonePrograms = zonePrograms as IList<ZoneProgram> ?? zonePrograms.ToList();
			var isvsListed = isvs?.ToList();

			if (isvsListed != null && isvsListed.Count() != 1 && isvsListed.Count() != incomingZonePrograms.Count())
			{
				throw new Exception("Number of items in isvs should be either 1 or equal to number of zone programs.");
			}

			//stop programs if specified to do so
			if (forceStop)
				incomingZonePrograms.ToList().ForEach(zoneProgram =>
				{
					if (zoneProgram.State == ProgramState.Started)
					{
						zoneProgram.Stop(true);
					}
				});
			else
			{
				incomingZonePrograms.ToList().ForEach(zoneProgram =>
				{
					if (zoneProgram.State == ProgramState.Started)
					{
						zoneProgram.Stop();
					}
				});
			}

			////incoming program must be stopped if forceStop is not true
			//if (incomingZonePrograms.Any(zp => zp.State != ProgramState.Stopped))
			//	throw new Exception("Given program must be stopped before a live sync is executed.");

			if (incomingZonePrograms.All(zp => zp is ReactiveZoneProgram))
			{
				incomingZonePrograms.ToList().ForEach(zoneProgram =>
				{
					lock (Barrier)
					{
						Barrier.AddParticipant();  //add participant for each program
					}

					lock (ZonePrograms)
					{
						ZonePrograms.Add(zoneProgram);
					}
				});
			}
			else if (incomingZonePrograms.All(zp => zp is LoopingZoneProgram) && ZonePrograms.All(zp => zp is LoopingZoneProgram))
			{
				lock (SyncLock)
				{
					//request sync-state from existing programs and incoming programs
					ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
					{
						zp.RequestSyncState();
					});
					incomingZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
					{
						zp.RequestSyncState();
					});

					//start all incoming programs
					for (int i = 0; i < incomingZonePrograms.Count; i++)
					{
						var zoneProgram = incomingZonePrograms[i];
						zoneProgram.SetSyncContext(this);
						ZonePrograms.Add(zoneProgram);
						zoneProgram.Start(sync: false,
							isv: isvsListed?.Count() == incomingZonePrograms.Count() ? isvsListed.ElementAt(i) : isvsListed?.First());
					}

					//wait for sync-state from all programs (incoming and existing)
					ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
					{
						zp.IsSynchronizable.WaitForFire();
					});

					//sync all incoming programs
					incomingZonePrograms.ToList().ForEach(zoneProgram =>
					{
						Barrier.AddParticipant();
					});

					ResetBarrier();

					//release all programs from sync-state
					ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
					{
						zp.WaitForSync.Fire(null, null);
					});

					////wait until all programs have left sync-state
					//ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
					//{
					//	zp.LeftSyncTrigger.WaitForFire();
					//});
				}

				SyncFinished.Fire(this, null);
			}
			else
			{
				throw new Exception(
					"All programs must be of the same type and must be included in the if statement that precedes this exception.");
			}
		}

		private void ResetBarrier()
		{
			lock (Barrier)
			{
				while (Barrier.ParticipantsRemaining < ZonePrograms.Count && Barrier.ParticipantsRemaining < ZonePrograms.Count)
					Barrier.SignalAndWait(1);
			}
		}

		/// <summary>
		/// Removes a given program from the synchronization.
		/// </summary>
		public void Unsync(ZoneProgram program)
		{
			lock (ZonePrograms)
			{
				if (!ZonePrograms.Contains(program)) return;
			}
			lock (Barrier)
			{
				Barrier.RemoveParticipant();
			}
			lock (ZonePrograms)
			{
				ZonePrograms.Remove(program);
			}
		}

		/// <summary>
		/// Unsyncs a zone's ZoneProgram from this SyncContext.
		/// </summary>
		public void Unsync(Zone zone)
		{
			Unsync(zone.ZoneProgram);
		}

		/// <summary>
		/// Signals the barrier and waits for the other programs to catch up or if it's the last program,
		/// then propels all the programs. To be used by programs to signal the other programs that signalling
		/// program is now waiting on the rest.
		/// </summary>
		public void SignalAndWait(int? timeout = null)
		{
			//lock (Barrier)
			//{
			if (Barrier.ParticipantCount <= 0) return;
			Barrier.SignalAndWait(timeout ?? UniversalTimeout);
			//}
		}

		public int GetNumberOfRemainingParticipants() => Barrier.ParticipantsRemaining;

		public int GetNumberOfTotalParticipants() => Barrier.ParticipantCount;

		#endregion

	}
}