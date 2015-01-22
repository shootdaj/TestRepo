﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ZoneLighting.TriggerDependencyNS;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ZoneProgramNS
{
	public abstract class LoopingZoneProgram : ZoneProgram
	{
		protected LoopingZoneProgram()
		{
			Setup();
			SetupRunProgramTask();
			LoopCTS = new CancellationTokenSource();
			Running = false;
		}

		public override void Dispose()
		{
			Unsetup();
			base.Dispose();
		}

		#region Looping Stuff

		private bool Running { get; set; }

		public CancellationTokenSource LoopCTS;
		protected Task LoopingTask { get; set; }
		protected Thread RunProgramThread { get; set; }

		protected void StartLoop()
		{
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

		private void SetupRunProgramTask()
		{
			LoopingTask = new Task(() =>
			{
				try
				{
					RunProgramThread = Thread.CurrentThread;
					SyncContext?.SignalAndWait();
					while (true)
					{
						//if sync is requested, go into synchronizable state
						if (IsSyncStateRequested)
						{
							IsSynchronizable.Fire(this, null);
							WaitForSync.WaitForFire();
							IsSyncStateRequested = false;
						}

						//start loop
						Loop();

						//if cancellation is requested, break out of loop after setting notification parameters for the consumer
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
					SyncContext?.SignalAndWait();
					StopTrigger.Fire(this, null);
					DebugTools.AddEvent("LoopingZoneProgram.Stop", "END Setting Running = false");
				}
				catch
				{
					DebugTools.AddEvent("LoopingZoneProgram.LoopingTask.Method", "Unexpected exception in LoopingTask");
				}
			}, LoopCTS.Token);
		}

		public abstract SyncLevel SyncLevel { get; set; }

		#region Overrideables

		public abstract void Setup();
		public abstract void Loop();

		/// <summary>
		/// Subclass can have Unsetup, but doesn't need to.
		/// </summary>
		public virtual void Unsetup() { }

		#endregion

		#endregion


		#region Transport Controls

		protected override void StartCore()
		{
			StartLoop();
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
						SyncContext?.SignalAndWait();
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
			}
			else
			{
				DebugTools.AddEvent("LoopingZoneProgram.Stop", "Running = FALSE");
			}

			DebugTools.AddEvent("LoopingZoneProgram.Stop", "END Stopping BG Program");

			StopTestingTrigger.Fire(this, null);
		}

		#endregion
	}
}
