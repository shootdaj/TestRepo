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

		public CancellationTokenSource LoopCTS;
		protected Task RunProgram { get; set; }
		protected Thread RunProgramThread { get; set; }

		protected void StartLoop()
		{
			RunProgram = new Task(() =>
			{
				RunProgramThread = Thread.CurrentThread;
				while (true)
				{
					Loop();
					if (LoopCTS.IsCancellationRequested)
						break;
				}
				StopTrigger.Fire(null, null);
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

		#endregion
	}
}
