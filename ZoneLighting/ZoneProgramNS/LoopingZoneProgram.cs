using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ZoneLighting.ZoneProgramNS
{
	public abstract class LoopingZoneProgram : ParameterizedZoneProgram
	{
		protected LoopingZoneProgram()
		{
			LoopCTS = new CancellationTokenSource();
		}

		#region Looping Stuff

		public CancellationTokenSource LoopCTS;
		protected Task RunProgram { get; set; }
		protected Thread RunProgramThread { get; set; }

		protected void StartLoop(ZoneProgramParameter parameter)
		{
			RunProgram = new Task(() =>
			{
				RunProgramThread = Thread.CurrentThread;
				while (true)
				{
					Loop(parameter);
					if (LoopCTS.IsCancellationRequested)
						break;
				}
				StopTrigger.Fire(null, null);
			}, LoopCTS.Token);

			RunProgram.Start();
		}

		#region Overrideables

		public abstract void Setup(ZoneProgramParameter parameter);
		public abstract void Loop(ZoneProgramParameter parameter);

		#endregion

		#endregion

		#region Overridden

		protected override void Start(ZoneProgramParameter parameter)
		{
			Setup(parameter);
			StartLoop(parameter);
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
