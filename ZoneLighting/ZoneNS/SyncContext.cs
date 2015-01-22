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
	public class SyncContext : IDisposable
	{
		public string Name { get; private set; }

		private Barrier Barrier { get; set; } = new Barrier(0);

		private List<ZoneProgram> ZonePrograms { get; set; }

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

		public void NonLiveSync(List<ZoneProgram> zonePrograms)
		{
			if (zonePrograms.All(p => p.State == ProgramState.Stopped))
			{
				foreach (var zoneProgram in zonePrograms)
				{
					zoneProgram.Start();
				}
			}
		}

		/// <summary>
		/// Makes the given zone a participant of this sync context. 
		/// </summary>
		/// <param name="zone"></param>
		public void MakeZoneParticipant(Zone zone)
		{
			Barrier.AddParticipant();
		}

		public void RemoveZoneParticipant(Zone zone)
		{
			Barrier.RemoveParticipant();
		}

		public void SignalAndWait()
		{
			Barrier.SignalAndWait();
		}

		public void Reset()
		{
			var participantCount = Barrier.ParticipantCount;
			Barrier = new Barrier(participantCount);
		}
	}
}
