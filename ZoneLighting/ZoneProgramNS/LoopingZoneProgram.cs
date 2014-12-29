using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ZoneLighting.ZoneProgramNS
{
	public abstract class LoopingZoneProgram : ZoneProgram
	{
		protected LoopingZoneProgram()
		{
			LoopCTS = new CancellationTokenSource();
		}

		#region Looping Stuff

		public bool Running { get; private set; } = false;
		public CancellationTokenSource LoopCTS;
		protected Task RunProgram { get; set; }
		protected Thread RunProgramThread { get; set; }

		protected void StartLoop()
		{
			RunProgram = new Task(() =>
			{
				try
				{
					RunProgramThread = Thread.CurrentThread;
					Running = true;
					while (true)
					{
						Loop();
						if (LoopCTS.IsCancellationRequested)
							break;

					}
					StopTrigger.Fire(this, null);
					Running = false;

				}
				catch
				{
					Running = false;
				}
			}, LoopCTS.Token);
			RunProgram.Start();
		}

		#region Overrideables

		public abstract void Setup();
		public abstract void Loop();


		#endregion

		#endregion

		#region Overridden
		
		//public override void StartBase(InputStartingValues inputStartingValues = null)
		//{
		//	Start();
		//}
		
		protected override void StartCore()
		{
			Setup();
			StartLoop();
		}

		public override void Stop(bool force)
		{
			if (Running)
			{
				if (force)
				{
					RunProgramThread.Abort();
				}
				else
				{
					LoopCTS.Cancel();
					StopTrigger.WaitForFire();
				}
			}

			StopTestingTrigger.Fire(this, null);
		}

		public override void Resume()
		{
			//TODO: Implement resume logic
		}

		protected override void Pause()
		{
			//TODO: Implement pause logic
		}

		#endregion
	}
}
