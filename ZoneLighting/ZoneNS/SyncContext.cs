using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

		/// <summary>
		/// Synchronizes all given programs and starts them.
		/// Should be called when all the programs to be synced are known beforehand and they are all stopped.
		/// </summary>
		/// <param name="zonePrograms"></param>
		public void SyncAndStart(List<ZoneProgram> zonePrograms)
		{
			//all programs must be stopped 
			if (zonePrograms.All(p => p.State == ProgramState.Stopped))
			{
				if (zonePrograms.All(p => p is ReactiveZoneProgram))
				{
					zonePrograms.ToList().ForEach(zp => Barrier.AddParticipant());	 //add participant for each program
					zonePrograms.ToList().ForEach(zp => ZonePrograms.Add(zp));
				}
				else if (zonePrograms.All(p => p is LoopingZoneProgram))
				{
					zonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
					{
						//sync request
						DebugTools.AddEvent("SyncContext.SyncAndStart", "Requesting sync-state from Program " + zp.Name);
						zp.RequestSyncState();

						//start
						DebugTools.AddEvent("SyncContext.SyncAndStart", "Starting Program " + zp.Name);
						zp.Start();
					});

					zonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
					{
						//wait for sync state
						DebugTools.AddEvent("SyncContext.SyncAndStart", "Waiting for Sync-State from Program " + zp.Name);
						zp.IsSynchronizable.WaitForFire();
					});

					zonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
					{
						//add participant for each program
						DebugTools.AddEvent("SyncContext.SyncAndStart", "Syncing " + zp.Name);
						Barrier.AddParticipant();

						//set this context as sync context if it's not already
						//DebugTools.AddEvent("SyncContext.SyncAndStart", "Setting Sync-Context for " + zp.Name);
						zp.SetSyncContext(this);

						//add each program to list of programs that are actively using this sync context
						//DebugTools.AddEvent("SyncContext.SyncAndStart", "Adding to ZonePrograms: " + zp.Name);
						ZonePrograms.Add(zp);
					});

					//release from sync state	
					zonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
					{
						DebugTools.AddEvent("SyncContext.SyncAndStart", "Release from Sync-State " + zp.Name);
						zp.WaitForSync.Fire(null, null);
					});

					zonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp =>
					{
						DebugTools.AddEvent("SyncContext.SyncAndStart", "Wait for Leaving Sync-State: " + zp.Name);
						zp.LeftSyncTrigger.WaitForFire();
						DebugTools.AddEvent("SyncContext.SyncAndStart", "Left Sync-State: " + zp.Name);
					});
				}
				else
				{
					throw new Exception("All programs must be of the same type and must be included in the if statement that precedes this exception.");
				}
			}
			else
			{
				throw new Exception("All programs must be stopped before a non-live sync is executed.");
			}
		}

		public void SyncLive(ZoneProgram zoneProgram)
		{
			SyncLive(new[] {zoneProgram});
		}
		
		/// <summary>
		/// Synchronizes the given programs with the programs that are already attached to this context.
		/// This can be called while the other programs are running, but will wait until they can get into
		/// their synchronizable states before executing the synchronization.
		/// </summary>
		public void SyncLive(IEnumerable<ZoneProgram> zonePrograms)
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
				zoneProgramsEnumerated.ToList().ForEach(zoneProgram => zoneProgram.Start(liveSync: false));

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

		public void SyncLive(params ZoneProgram[] zonePrograms)
		{
			SyncLive(zonePrograms.ToList());
		}

		public void SyncAndStart(params ZoneProgram[] zonePrograms)
		{
			SyncAndStart(zonePrograms.ToList());
		}

		//public void AddParticipant(ZoneProgram zoneProgram)
		//{
		//	Barrier.AddParticipant();
		//	ZonePrograms.Add(zoneProgram);
		//}

		/// <summary>
		/// Removes a given program from the synchronization.
		/// </summary>
		public void Unsync(ZoneProgram program)
		{
			if (!ZonePrograms.Contains(program)) return;
			Barrier.RemoveParticipant();
			ZonePrograms.Remove(program);
		}

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

		public int GetNumberOfRemainingParticipants() => Barrier.ParticipantsRemaining;

		public int GetNumberOfTotalParticipants() => Barrier.ParticipantCount;

		#endregion
	}
}
