using System.Threading;

namespace ZoneLighting.ZoneProgram
{
	public abstract class LoopingZoneProgram : ZoneProgram
	{
		protected LoopingZoneProgram()
		{
			LoopCTS = new CancellationTokenSource();
		}

		protected void StartLoop(IZoneProgramParameter parameter)
		{
			while (!LoopCTS.IsCancellationRequested)
			{
				Loop(parameter);
			}
		}

		public abstract void Loop(IZoneProgramParameter parameter);

		public CancellationTokenSource LoopCTS;

		public override void Start(IZoneProgramParameter parameter)
		{
			StartLoop(parameter);
		}

		public override void Stop()
		{
			LoopCTS.Cancel();
		}

		
	}
}
