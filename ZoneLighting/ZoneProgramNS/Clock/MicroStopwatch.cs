using System;

namespace ZoneLighting.ZoneProgramNS.Clock
{
	/// <summary>
	/// MicroStopwatch class
	/// Source: http://www.codeproject.com/Articles/98346/Microsecond-and-Millisecond-NET-Timer
	/// </summary>
	public class MicroStopwatch : System.Diagnostics.Stopwatch
	{
		readonly double _microSecPerTick =
			1000000D / System.Diagnostics.Stopwatch.Frequency;

		public MicroStopwatch()
		{
			if (!System.Diagnostics.Stopwatch.IsHighResolution)
			{
				throw new Exception("On this system the high-resolution " +
				                    "performance counter is not available");
			}
		}

		public long ElapsedMicroseconds
		{
			get
			{
				return (long)(ElapsedTicks * _microSecPerTick);
			}
		}
	}
}