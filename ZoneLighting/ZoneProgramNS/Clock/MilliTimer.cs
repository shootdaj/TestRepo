using System.Threading;

namespace ZoneLighting.ZoneProgramNS.Clock
{
	/// <summary>
	/// MilliTimer class
	/// </summary>
	public class MilliTimer : MicroTimer
	{
		public int ThreadSleepTime { get; set; }

		public MilliTimer(long interval, int threadSleepTime = 1)
		{
			Interval = interval;
			ThreadSleepTime = threadSleepTime;
		}
		
		public event MicroTimerElapsedEventHandler MilliTimerElapsed;

		protected override void NotificationTimer(ref long timerIntervalInMicroSec, ref long ignoreEventIfLateBy, ref bool stopTimer)
		{
			int timerCount = 0;
			long nextNotification = 0;

			MicroStopwatch microStopwatch = new MicroStopwatch();
			microStopwatch.Start();
			while (!stopTimer)
			{
				long callbackFunctionExecutionTime =
					microStopwatch.ElapsedMicroseconds - nextNotification;

				long timerIntervalInMicroSecCurrent =
					System.Threading.Interlocked.Read(ref timerIntervalInMicroSec);
				long ignoreEventIfLateByCurrent =
					System.Threading.Interlocked.Read(ref ignoreEventIfLateBy);

				nextNotification += timerIntervalInMicroSecCurrent;
				timerCount++;
				long elapsedMicroseconds = 0;

				while ((elapsedMicroseconds = microStopwatch.ElapsedMicroseconds)
					   < nextNotification)
				{
					Thread.Sleep(ThreadSleepTime);
				}

				long timerLateBy = elapsedMicroseconds - nextNotification;

				if (timerLateBy >= ignoreEventIfLateByCurrent)
				{
					continue;
				}

				MicroTimerEventArgs microTimerEventArgs =
					new MicroTimerEventArgs(timerCount,
						elapsedMicroseconds,
						timerLateBy,
						callbackFunctionExecutionTime);
				MilliTimerElapsed(this, microTimerEventArgs);
			}


			microStopwatch.Stop();
		}
	}
}