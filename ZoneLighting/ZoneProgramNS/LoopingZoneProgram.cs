using System.Threading;
using System.Threading.Tasks;

namespace ZoneLighting.ZoneProgramNS
{
	public abstract class LoopingZoneProgram : ZoneProgram
	{
		#region Looping Stuff

		protected LoopingZoneProgram()
		{
			LoopCTS = new CancellationTokenSource();
		}

		protected void StartLoop(ZoneProgramParameter parameter)
		{
			Task.Run(() =>
			{
				while (!LoopCTS.Token.IsCancellationRequested)
				{
					Loop(parameter);
				}
				StopTrigger.Fire(null, null);
			}, LoopCTS.Token);
		}

		public abstract void Loop(ZoneProgramParameter parameter);

		public CancellationTokenSource LoopCTS;

		#endregion

		#region ZoneProgram's Abstract Members

		protected override void Start(ZoneProgramParameter parameter)
		{
			StartLoop(parameter);
		}

		public override void Stop()
		{
			LoopCTS.Cancel();
			StopTrigger.WaitForFire();
		}

		#endregion
	}
}
