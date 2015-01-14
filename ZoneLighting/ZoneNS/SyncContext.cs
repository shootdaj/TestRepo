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

		public Barrier Barrier { get; } = new Barrier(0);

		public Zone LeadingZone { get; set; }

		public IList<Zone> Zones { get; } = new List<Zone>();

		//public LoopingZoneProgram LeadingZoneProgram { get; private set; }

		//public IList<LoopingZoneProgram> ZonePrograms { get; } = new List<LoopingZoneProgram>();

		public SyncLevel SyncLevel { get; private set; }

		/// <summary>
		/// At least one zone program is needed to lead the synchronization.
		/// </summary>
		/// <param name="zoneProgram">A zone program to create a sync context over.</param>
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
				SyncLevel = ((LoopingZoneProgram)zone.ZoneProgram).SyncLevel;
			}

			//if sync level is given, then override the existing one
			if (syncLevel != null)
			{
				if (syncLevel.GetType() == SyncLevel.GetType())
					SyncLevel = syncLevel;
				else
				{
					throw new Exception("Type mismatch in provided SyncLevel subtype. Keeping existing SyncLevel.");
				}
			}

			//add itself to the barrier's participant list
			//zone.SetBackgroundBarrier(Barrier); TODO: This enables the barrier for interrupts.. or something
			zone.ZoneProgram.AttachBarrier(Barrier);
		}

		public void RemoveZone(Zone zone)
		{
			if (zone.ZoneProgram.Barrier == Barrier)
				zone.ZoneProgram.DetachBarrier();

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
	}
}
