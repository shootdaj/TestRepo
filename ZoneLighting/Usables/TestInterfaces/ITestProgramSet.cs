using System.Collections.Generic;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.Usables.TestInterfaces
{
	public interface ITestProgramSet
	{
		IEnumerable<ZoneProgram> ZoneProgramsTest { get; }
	}
}
