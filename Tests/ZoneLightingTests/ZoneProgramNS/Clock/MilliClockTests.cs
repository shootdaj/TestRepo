using System;
using System.Threading;
using NUnit.Framework;
using ZoneLighting.ZoneProgramNS.Clock;

namespace ZoneLightingTests.ZoneProgramNS.Clock
{
	public class MilliClockTests
	{
		[Ignore]
		[TestCase(30, 2)]
		[TestCase(60, 2)]
		public void MilliClock_ClockTicksAreCloseToThreadTimerTicks(int sleepSeconds, int msThreshold)
		{
			var ticks = 0;
			var clock = new MilliClock(1000, args => ticks++, 0);
			clock.Start();

			Thread.Sleep(sleepSeconds * 1000);
			//=== Manual Step : Check CPU to see processor efficiency.

			clock.Stop();
			Console.WriteLine(ticks);

			Assert.That(Math.Abs(ticks - (sleepSeconds * 1000)) < msThreshold);
		}
	}
}
