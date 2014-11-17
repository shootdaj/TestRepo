using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoneLighting.ZoneProgram
{
	public interface IZoneProgramMetadata
	{
		string Name { get; }
		string ParameterName { get; }
	}
}
