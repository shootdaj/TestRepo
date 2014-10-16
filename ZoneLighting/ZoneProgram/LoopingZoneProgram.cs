using System.Threading;
using System.Threading.Tasks;

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
			Task.Run(() =>
			{
				while (!LoopCTS.Token.IsCancellationRequested)
				{
					Loop(parameter);
				}
			}, LoopCTS.Token);
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
