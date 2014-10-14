using System.Threading;

namespace ZoneLighting.ZoneProgram
{
	public abstract class LoopingZoneProgram : ZoneProgram
	{
		public void StartLoop()
		{
			while (!LoopCTS.IsCancellationRequested)
			{
				Loop();
			}
		}

		public abstract void Loop(IZoneProgramParameter parameter);

		public CancellationTokenSource LoopCTS;

		public override void Start()
		{
			StartLoop();
		}

		public override void Stop()
		{
			LoopCTS.Cancel();
		}

		
	}
}
