using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ZoneLighting.ZoneProgramNS
{
	public abstract class LoopingZoneProgram : ZoneProgram
	{
		protected LoopingZoneProgram()
		{
			LoopCTS = new CancellationTokenSource();
			Running = false;
		}

		#region Looping Stuff

		public bool Running { get; private set; }

		public CancellationTokenSource LoopCTS;
		protected Task LoopingTask { get; set; }
		protected Thread RunProgramThread { get; set; }

		protected void StartLoop(Barrier barrier)
		{
			SetupRunProgramTask(barrier);
			if (!Running)
			{
				DebugTools.AddEvent("LoopingZoneProgram.StartLoop", "Running = FALSE");
				DebugTools.AddEvent("LoopingZoneProgram.LoopingTask.Method", "Setting Running = TRUE");
				Running = true;

				DebugTools.AddEvent("LoopingZoneProgram.StartLoop", "START StartLoop()");
				LoopingTask.Start();
				DebugTools.AddEvent("LoopingZoneProgram.StartLoop", "END StartLoop()");
			}
			else
			{
				DebugTools.AddEvent("LoopingZoneProgram.StartLoop", "Running = TRUE");
			}
		}

		private void SetupRunProgramTask(Barrier barrier)
		{
			LoopingTask = new Task(() =>
			{
				try
				{
					RunProgramThread = Thread.CurrentThread;
					while (true)
					{
						Loop(barrier);
						if (LoopCTS.IsCancellationRequested)
						{
							Running = false;
							StopTrigger.Fire(this, null);
							break;
						}
					}
				}
				catch (ThreadAbortException ex)
				{
					DebugTools.AddEvent("LoopingZoneProgram.LoopingTask.Method", "LoopingTask thread aborted");
					DebugTools.AddEvent("LoopingZoneProgram.Stop", "START Setting Running = false");
					Running = false;
					StopTrigger.Fire(this, null);
					DebugTools.AddEvent("LoopingZoneProgram.Stop", "END Setting Running = false");
				}
				catch
				{
					DebugTools.AddEvent("LoopingZoneProgram.LoopingTask.Method", "Unexpected exception in LoopingTask");
				}
			}, LoopCTS.Token);
		}

		#region Overrideables

		public abstract void Setup();
		public abstract void Loop(Barrier barrier);


		#endregion

		#endregion

		#region Overridden

		//public override void StartBase(InputStartingValues inputStartingValues = null)
		//{
		//	Start();
		//}

		protected override void StartCore(Barrier barrier)
		{
			Setup();
			StartLoop(barrier);
		}

		protected override void StopCore(bool force)
		{
			DebugTools.AddEvent("LoopingZoneProgram.Stop", "START Stopping BG Program");

			if (Running)
			{
				DebugTools.AddEvent("LoopingZoneProgram.Stop", "Running = TRUE");

				if (force)
				{
					if (RunProgramThread != null)
					{
						DebugTools.AddEvent("LoopingZoneProgram.Stop", "START Force aborting BG Program thread");
						RunProgramThread.Abort();
						StopTrigger.WaitForFire();
						DebugTools.AddEvent("LoopingZoneProgram.Stop", "END Force aborting BG Program thread");
					}
					else
					{
						DebugTools.AddEvent("LoopingZoneProgram.Stop", "RunProgramThread was null");
						DebugTools.Print();
					}
				}
				else
				{
					LoopCTS.Cancel();
					if (!StopTrigger.WaitForFire())
					{
						DebugTools.AddEvent("LoopingZoneProgram.Stop", "Loop did not cancel cooperatively.");
						DebugTools.Print();
					}
				}

				DebugTools.AddEvent("LoopingZoneProgram.Stop", "START Clearing Inputs");

				//clear inputs because they will be re-added by the setup
				foreach (var zoneProgramInput in Inputs)
				{
					zoneProgramInput.Dispose();
				}
				Inputs.Clear();

				DebugTools.AddEvent("LoopingZoneProgram.Stop", "END Clearing Inputs");
			}
			else
			{
				DebugTools.AddEvent("LoopingZoneProgram.Stop", "Running = FALSE");
			}

			DebugTools.AddEvent("LoopingZoneProgram.Stop", "END Stopping BG Program");

			StopTestingTrigger.Fire(this, null);
		}

		public override void Resume(Barrier barrier)
		{
			//TODO: Implement resume logic - for now, it's just gonna call start
			Start(barrier: barrier);

		}

		protected override void Pause()
		{
			//TODO: Implement pause logic - for now, it's just gonna call stop forcibly
			Stop(true);
		}

		#endregion
	}
}
