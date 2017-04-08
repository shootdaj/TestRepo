using System;
using System.Threading;
using NUnit.Framework;
using ZoneLighting.ZoneProgramNS.Clock;

namespace ZoneLightingTests.ZoneProgramNS.Clock
{
    [Explicit("Manual Test")]
    public class StopwatchClockTests
	{
		[TestCase(30)]
		public void StopwatchClock_ClockTicksEqualStopwatchTicks(int sleepSeconds)
		{
			var ticks = 0;
			var clock = new StopwatchClock(10, () => ticks++, 0);
			clock.Start();

			Thread.Sleep(sleepSeconds * 1000);
			//=== Manual Step : Check CPU to see processor efficiency.

			clock.Stop();

			Thread.Sleep(1000);

			Console.WriteLine(ticks);
		}
	}
}
