using System;
using System.Threading;
using NUnit.Framework;
using ZoneLighting.ZoneProgramNS.Clock;

namespace ZoneLightingTests.ZoneProgramNS.Clock
{
	public class TimerClockTests
	{
		[TestCase(30)]
		[Ignore]
		public void TimerClock_ClockTicksEqualThreadTimerTicks(int sleepSeconds)
		{
			var ticks = 0;
			var clock = new TimerClock(1000, args => ticks++);
			clock.Start();

			Thread.Sleep(sleepSeconds * 1000);
			//=== Manual Step : Check CPU to see processor efficiency.

			clock.Stop();
			Console.WriteLine(ticks);
		}
	}
}
