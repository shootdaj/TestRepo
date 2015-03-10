using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using ZoneLighting.TriggerDependencyNS;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ZoneProgramNS
{
	public abstract class LoopingZoneProgram : ZoneProgram
	{
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		protected LoopingZoneProgram()
		{
			Setup();
			LoopCTS = new CancellationTokenSource();
			Running = false;
		}

		public override void Dispose(bool force)
		{
			Unsetup();
			base.Dispose(force);
			LoopCTS.Dispose();
			IsSynchronizable.Dispose();
			WaitForSync.Dispose();
		}

		#region Looping Stuff

		protected bool IsSyncStateRequested { get; set; }
		public Trigger IsSynchronizable { get; set; } = new Trigger("LoopingZoneProgram.IsSynchronizable");
		public Trigger WaitForSync { get; set; } = new Trigger("LoopingZoneProgram.WaitForSync");
		
		public object SyncLock { get; set; } = new object();

		private bool Running { get; set; }

		public CancellationTokenSource LoopCTS;
		private Task LoopingTask { get; set; }
		private Thread RunProgramThread { get; set; }

		protected void StartLoop()
		{
			if (!Running)
			{
				SetupRunProgramTask();

				//DebugTools.AddEvent("LoopingZoneProgram.StartLoop", "Running = FALSE");
				//DebugTools.AddEvent("LoopingZoneProgram.LoopingTask.Method", "Setting Running = TRUE");
				Running = true;
				
				//DebugTools.AddEvent("LoopingZoneProgram.StartLoop", "START StartLoop()");
				LoopingTask.Start();
				//DebugTools.AddEvent("LoopingZoneProgram.StartLoop", "END StartLoop()");
			}
			else
			{
				//DebugTools.AddEvent("LoopingZoneProgram.StartLoop", "Running = TRUE");
			}
		}

		private void SetupRunProgramTask()
		{
			LoopCTS.Dispose();
			LoopCTS = new CancellationTokenSource();
			LoopingTask?.Dispose();
			LoopingTask = new Task(() =>
			{
				try
				{
					RunProgramThread = Thread.CurrentThread;
					while (true)
					{
						//if sync is requested, go into synchronizable state
						if (IsSyncStateRequested)
						{
							lock (SyncLock)
							{
								DebugTools.AddEvent("LoopingZoneProgram.LoopingTask", "Entering Sync-State: " + Name);
								IsSynchronizable.Fire(this, null);

								//SyncContext?.SignalAndWait();

								DebugTools.AddEvent("LoopingZoneProgram.LoopingTask",
									"In Sync-State - Waiting for Signal from SyncContext: " + Name);
								WaitForSync.WaitForFire();
								DebugTools.AddEvent("LoopingZoneProgram.LoopingTask", "Leaving Sync-State: " + Name);
								IsSyncStateRequested = false;
							}
						}

						//SyncContext?.SignalAndWait();

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
					//DebugTools.AddEvent("LoopingZoneProgram.LoopingTask.Method", "LoopingTask thread aborted");
					//DebugTools.AddEvent("LoopingZoneProgram.Stop", "START Setting Running = false");
					Running = false;
					StopTrigger.Fire(this, null);
					//DebugTools.AddEvent("LoopingZoneProgram.Stop", "END Setting Running = false");
				}
				catch (Exception ex)
				{
					Running = false;
					StopTrigger.Fire(this, null);
					DebugTools.AddEvent("LoopingZoneProgram.LoopingTask.Method", "Unexpected exception in LoopingTask");
				}
			}, LoopCTS.Token);
		}

		public abstract SyncLevel SyncLevel { get; set; }

		public override void SetSyncContext(SyncContext syncContext)
		{
			//remove from old sync context, if any
			SyncContext?.Unsync(this);

			//if same sync context is being passed, ignore request
			if (syncContext == SyncContext) return;

			if (State == ProgramState.Stopped || IsSyncStateRequested)
				SyncContext = syncContext;
			else
				throw new Exception("Can only set sync context while program is stopped or if it's in synchronizable state.");
		}

		#region Overrideables

		public abstract void Setup();
		public abstract void Loop();

		/// <summary>
		/// Subclass can have Unsetup, but doesn't need to.
		/// </summary>
		public virtual void Unsetup()
		{
			
		}

		#endregion

		#endregion


		#region Transport Controls

		/// <summary>
		/// Requests the program to pause when it's at its synchronizable state.
		/// </summary>
		/// <returns></returns>
		public void RequestSyncState()
		{
			lock (SyncLock)
			{
				IsSyncStateRequested = true;
			}
		}

		/// <summary>
		/// Cancels Sync State request and releases from the sync state, if in that state.
		/// </summary>
		public void CancelSyncState()
		{
			//if this program is in its sync state, release it
			if (IsSyncStateRequested)
			{
				WaitForSync.Fire(null, null);
			}
			IsSyncStateRequested = false;
		}

		protected override void StartCore()//bool isSyncRequested)
		{
			//handle sync state request
			//StartTrigger.Fire(this, null);
			//if (isSyncRequested)
			//	RequestSyncState();

			StartLoop();
		}

		protected override void StopCore(bool force)
		{
			DebugTools.AddEvent("LoopingZoneProgram.StopCore", "STOP " + Name);
			//DebugTools.AddEvent("LoopingZoneProgram.StopCore", "Canceling Sync-State on Program " + Name);

			//cancel sync state req and release from sync state
			//CancelSyncState();

			//DebugTools.AddEvent("LoopingZoneProgram.Stop", "START Stopping BG Program");

			if (Running)
			{
				//DebugTools.AddEvent("LoopingZoneProgram.Stop", "Running = TRUE");

				if (force)
				{
					if (RunProgramThread != null)
					{
						//DebugTools.AddEvent("LoopingZoneProgram.Stop", "START Force aborting BG Program thread");
						RunProgramThread.Abort();
						StopTrigger.WaitForFire();
						//DebugTools.AddEvent("LoopingZoneProgram.Stop", "END Force aborting BG Program thread");
					}
					else
					{
						//DebugTools.AddEvent("LoopingZoneProgram.Stop", "RunProgramThread was null");
						//DebugTools.Print();
					}
				}
				else
				{
					LoopCTS.Cancel();
					if (!StopTrigger.WaitForFire())
					{
						//DebugTools.AddEvent("LoopingZoneProgram.Stop", "Loop did not cancel cooperatively.");
						//DebugTools.Print();
					}
				}
			}
			else
			{
				//DebugTools.AddEvent("LoopingZoneProgram.Stop", "Running = FALSE");
			}

			//DebugTools.AddEvent("LoopingZoneProgram.Stop", "END Stopping BG Program");

			StopTestingTrigger.Fire(this, null);
		}

		#endregion
	}
}
