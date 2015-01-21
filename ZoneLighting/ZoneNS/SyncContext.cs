using System;
using System.Threading;

namespace ZoneLighting.ZoneNS
{
	/// <summary>
	/// A wrapper on top of Barrier to have easier manageability.
	/// </summary>
	public class SyncContext : IDisposable
	{
		public string Name { get; private set; }

		private Barrier Barrier { get; set; } = new Barrier(0);

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
