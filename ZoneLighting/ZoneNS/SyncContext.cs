using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;
using Newtonsoft.Json;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.ZoneNS
{
	/// <summary>
	/// A way to automatically synchronize the rate at which the zone programs in the zones are running.
	/// </summary>
	public class SyncContext : IDisposable
	{
		public string Name { get; private set; }

		private Barrier Barrier { get; set; } = new Barrier(0);

		public Zone LeadingZone { get; set; }

		private IList<Zone> Zones { get; } = new List<Zone>();

		//public Dictionary<Zone, bool> ZoneIsParticipant { get; private set; } = new Dictionary<Zone, bool>();

		public SyncLevel SyncLevel { get; private set; }

		/// <summary>
		/// At least one zone program is needed to lead the synchronization.
		/// </summary>
		/// <param name="zone">A zone to create a sync context over.</param>
		/// <param name="name">Name of synchronization context for handy referencing.</param>
		public SyncContext(Zone zone = null, string name = null)
		{
			if (name != null)
				Name = name + "SyncContext";
			if (zone != null)
				AddZone(zone);
		}

		public void AddZone(Zone zone, SyncLevel syncLevel = null)
		{
			Zones.Add(zone);
			if (LeadingZone == null)
			{
				LeadingZone = zone;
				SyncLevel = LeadingZone.SyncLevel;
			}

			//if sync level is given, then override the existing one
			if (syncLevel != null)
			{
				if (syncLevel.GetType() == SyncLevel.GetType())
					zone.SyncLevel = syncLevel;
				else
				{
					throw new Exception("Type mismatch in provided SyncLevel subtype. Keeping existing SyncLevel.");
				}
			}

			//add itself to the barrier's participant list
			//zone.SetBackgroundBarrier(Barrier); TODO: This enables the barrier for interrupts.. or something
			zone.SetupSyncContext(this);
		}

		public void RemoveZone(Zone zone)
		{
			zone.UnsetupSyncContext();
			if (zone == LeadingZone)
			{
				Dispose();
			}
			else
			{
				Zones.Remove(zone);
			}
		}

		public void Dispose()
		{
			Zones.Clear();
			LeadingZone = null;
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

		public bool ContainsZone(Zone zone)
		{
			return Zones.Contains(zone);
		}

		public void SignalAndWait()
		{
			Barrier.SignalAndWait();
		}

		public void Reset()
		{
			//Barrier.Dispose();
			Barrier = new Barrier(Zones.Count);
		}
	}
}
