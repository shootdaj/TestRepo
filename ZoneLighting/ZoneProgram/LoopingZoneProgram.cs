using System.Threading;
using System.Threading.Tasks;

namespace ZoneLighting.ZoneProgram
{
	public abstract class LoopingZoneProgram : ZoneProgram
	{
		#region Looping Stuff

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

		#endregion

		#region ZoneProgram's Abstract Members

		protected override void Start(IZoneProgramParameter parameter)
		{
			StartLoop(parameter);
		}

		public override void Stop()
		{
			LoopCTS.Cancel();
		}

		#endregion
	}
}
