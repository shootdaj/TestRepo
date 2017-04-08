using System;
using System.Threading;
using NUnit.Framework;
using ZoneLighting.ZoneProgramNS.Clock;

namespace ZoneLightingTests.ZoneProgramNS.Clock
{
    [Explicit("Manual Test")]
    public class MicroTimerTests
	{
		[TestCase(30, 10000, 1000, 2)]
		[TestCase(30, 5000, 1000, 2)]
		[TestCase(30, 5000, 500, 2)]
		[TestCase(30, 2000, 500, 2)]
		[TestCase(30, 2000, 200, 2)]
		[TestCase(30, 1000, 500, 2)]
		[TestCase(30, 1000, 200, 2)]
		[TestCase(30, 1000, 100, 2)]
		[TestCase(30, 1000, 50, 2)]
		public void MicroTimer_ClockTicksAreCloseToThreadTimerTicks(int sleepSeconds, long interval, long ignoreIfLateBy, int msThreshold)
		{
			var ticks = 0;
			var clock = new MicroTimer(interval);
			clock.IgnoreEventIfLateBy = ignoreIfLateBy;
			clock.MicroTimerElapsed += (sender, args) => ticks++;
			clock.Start();

			Thread.Sleep(sleepSeconds * 1000);
			//=== Manual Step : Check CPU to see processor efficiency.

			clock.Stop();
			Console.WriteLine(ticks);

			Assert.That(Math.Abs(ticks - (sleepSeconds * 1000000/interval)) <= msThreshold);
		}
	}
}
