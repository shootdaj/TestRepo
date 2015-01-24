using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.ZoneNS
{
	/// <summary>
	/// A wrapper on top of Barrier to have easier manageability.
	/// </summary>
	public sealed class SyncContext : IDisposable
	{
		public string Name { get; private set; }

		private Barrier Barrier { get; set; } = new Barrier(0);

		private List<ZoneProgram> ZonePrograms { get; set; } = new List<ZoneProgram>();

		/// <param name="name">Name of synchronization context for handy referencing.</param>
		public SyncContext(string name = null)
		{
			if (name != null)
				Name = name + "SyncContext";
		}

		public void Dispose()
		{
			Barrier.Dispose();
			Name = null;
		}

		/// <summary>
		/// Synchronization when all the programs to be synced are known beforehand and they are all stopped.
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
					zonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp => zp.RequestSyncState());	//sync request
					zonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp => zp.Start());						//start
					zonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp => zp.IsSynchronizable.WaitForFire());   //wait for sync state
					zonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp => Barrier.AddParticipant());	//add participant for each program
					zonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp => ZonePrograms.Add(zp));		//add each program to list of programs that are actively using this sync context
					zonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp => zp.WaitForSync.Fire(null, null)); //release from sync state
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

		public void SyncAndStartLive(ZoneProgram zoneProgram)
		{
			//incoming program must be stopped 
			if (zoneProgram.State == ProgramState.Stopped)
			{
				if (zoneProgram is ReactiveZoneProgram)
				{
					Barrier.AddParticipant();		 //add participant for each program
					ZonePrograms.Add(zoneProgram);
				}
				else if (zoneProgram is LoopingZoneProgram && ZonePrograms.All(zp => zp is LoopingZoneProgram))
				{
					((LoopingZoneProgram)zoneProgram).RequestSyncState();
					zoneProgram.Start(liveSync: false);
					ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp => zp.RequestSyncState());


					((LoopingZoneProgram)zoneProgram).IsSynchronizable.WaitForFire();
					ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp => zp.IsSynchronizable.WaitForFire());
					
					Barrier.AddParticipant();
					ZonePrograms.Add(zoneProgram);

					ZonePrograms.Cast<LoopingZoneProgram>().ToList().ForEach(zp => zp.WaitForSync.Fire(null, null));
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

		public void SyncAndStart(params ZoneProgram[] zonePrograms)
		{
			SyncAndStart(zonePrograms.ToList());
		}

		public void AddParticipant(ZoneProgram zoneProgram)
		{
			Barrier.AddParticipant();
			ZonePrograms.Add(zoneProgram);
		}

		/// <summary>
		/// A program can request itself to be removed from the synchronization by calling this method
		/// </summary>
		/// <param name="program"></param>
		public void Unsync(ZoneProgram program)
		{
			if (ZonePrograms.Contains(program))
			{
				if (Barrier.ParticipantsRemaining == 3)	
					Console.WriteLine("4 Barriers, 3 Remaining");
				Barrier.RemoveParticipant();
				ZonePrograms.Remove(program);
			}
		}

		///// <summary>
		///// Makes the given zone a participant of this sync context. 
		///// </summary>
		///// <param name="zone"></param>
		//public void MakeZoneParticipant(Zone zone)
		//{
		//	Barrier.AddParticipant();
		//}

		//public void RemoveZoneParticipant(ZoneProgram zoneProgram)
		//{
		//	Barrier.RemoveParticipant();
		//}

		public void SignalAndWait()
		{
			if (ZonePrograms.Any())
				Barrier.SignalAndWait();
		}

		//public void Reset()
		//{
		//	var participantCount = Barrier.ParticipantCount;
		//	Barrier = new Barrier(participantCount);
		//}
	}
}
