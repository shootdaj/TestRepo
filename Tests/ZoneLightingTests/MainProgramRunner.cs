using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console;
using NUnit.Framework;

namespace ZoneLightingTests
{
	public class MainProgramRunner
	{
		[Ignore]
		[Test]
		public void RunProgram_Main()
		{
			Program.Main(null);
		}
	}
}
