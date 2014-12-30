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
			Running = false;
		}

		#region Looping Stuff

		private bool _running;

		public bool Running
		{
			get { return _running; }
			private set
			{
				//Console.WriteLine("Running = " + value);
				_running = value;
			}
		}

		public CancellationTokenSource LoopCTS;
		protected Task RunProgram { get; set; }
		protected Thread RunProgramThread { get; set; }

		protected void StartLoop()
		{
			SetupRunProgramTask();
			if (!Running)
				RunProgram.Start();
		}

		private void SetupRunProgramTask()
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
						{
							Running = false;
							break;
						}
					}
					StopTrigger.Fire(this, null);
				}
				catch
				{
					// ignored
				}
			}, LoopCTS.Token);
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
			//Console.WriteLine("START Stopping BG Program " + Name);

			//Console.WriteLine("Check if BG Program is running");

			if (Running)
			{
				//Console.WriteLine("Running = true");
				
				if (force)
				{
					Running = false;
					//Console.WriteLine("START Aborting BG Program");
					RunProgramThread.Abort();
					//Console.WriteLine("FINISHED Aborting BG Program");
				}
				else
				{
					LoopCTS.Cancel();
					StopTrigger.WaitForFire();
				}

				//Console.WriteLine("START Clearing inputs");

				//clear inputs because they will be re-added by the setup
				foreach (var zoneProgramInput in Inputs)
				{
					zoneProgramInput.Dispose();
				}
				Inputs.Clear();

				//Console.WriteLine("FINISHED Clearing inputs");
			}
			else
			{
				//Console.WriteLine("Running = false");
			}

			//Console.WriteLine("FINISHED Stopping BG Program");

			StopTestingTrigger.Fire(this, null);
		}

		public override void Resume()
		{
			//TODO: Implement resume logic - for now, it's just gonna call start
			Start();

		}

		protected override void Pause()
		{
			//TODO: Implement pause logic - for now, it's just gonna call stop forcibly
			Stop(true);
		}

		#endregion
	}
}
