using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLighting.ZoneProgramNS
{
	public class ProgramSet : IDisposable
	{
		public string Name { get; private set; }
		public SyncContext SyncContext { get; } = new SyncContext();
		public List<Zone> Zones { get; }
		public string ProgramName { get; private set; }

		public ProgramSet(string programName, IEnumerable<Zone> zones, bool sync, ISV isv, string setName)
		{
			if (!ZoneScaffolder.Instance.DoesProgramExist(programName))
				throw new Exception(string.Format("No program by the name '{0}' exists.", programName));

			Name = setName;
			Zones = zones.ToList();
			ProgramName = programName;

			Zones.ForEach(zone =>	
			{
				//zone.Uninitialize();	//todo: test this
				ZoneScaffolder.Instance.InitializeZone(zone, programName, isv, sync, SyncContext);
			});
		}

		public void Dispose()
		{
			
		}
	}
}
