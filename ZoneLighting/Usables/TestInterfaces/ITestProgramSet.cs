using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.Usables.TestInterfaces
{
	public interface ITestProgramSet
	{
		IEnumerable<ZoneProgram> ZoneProgramsTest { get; }
	}
}
