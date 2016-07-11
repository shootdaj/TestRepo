using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ZoneLighting.ZoneProgramNS.Clock
{
	public class StopwatchClock
	{
		public enum ClockState
		{
			Started,
			Stopped
		}

		public ClockState State { get; set; } = ClockState.Stopped;

		private Stopwatch Stopwatch { get; set; } = new Stopwatch();

		public int Interval	{ get; set; }

		public Action Action { get; set; }
		public int ThreadSleepTime { get; set; }

		/// <summary>
		/// Creates a new instance of the StopwatchClock
		/// </summary>
		/// <param name="interval">Tick time defines how often the action will be called</param>
		/// <param name="action">Action to perform for every tick of the clock</param>
		/// <param name="threadSleepTime">Amount of time the loop will sleep for. This determines the higher bound for accuracy.</param>
		public StopwatchClock(int interval, Action action, int threadSleepTime = 1)
		{
			Interval = interval;
			Action = action;
			ThreadSleepTime = threadSleepTime;
		}

		private bool StopFlag { get; set; } = false;

		private void Loop()
		{
			StopFlag = false;
			Stopwatch.Start();
			State = ClockState.Started;
			long lastElapsedMS = 0;

			while (!StopFlag)
			{
				var currentElapsedMS = Stopwatch.ElapsedMilliseconds;
				if (currentElapsedMS % Interval < lastElapsedMS)
				{
					//Task.Run(Action);
					Action();
				}
				lastElapsedMS = currentElapsedMS % Interval;
				Thread.Sleep(ThreadSleepTime);
			}

			State = ClockState.Stopped;
			Stopwatch.Stop();
		}

		public void Start()
		{
			var task = new Task(Loop, TaskCreationOptions.LongRunning);
			task.Start();
		}

		public void Stop()
		{
			StopFlag = true;
		}
	}
}
