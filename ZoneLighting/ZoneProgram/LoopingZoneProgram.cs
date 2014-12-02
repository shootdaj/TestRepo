using System.Threading;
using System.Threading.Tasks;
using ZoneLighting.TriggerDependencyNS;

namespace ZoneLighting.ZoneProgram
{
	public abstract class LoopingZoneProgram : ZoneProgram
	{
		#region Looping Stuff

		protected LoopingZoneProgram()
		{
			LoopCTS = new CancellationTokenSource();
			_stopTrigger = new Trigger();
		}

		protected void StartLoop(IZoneProgramParameter parameter)
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
			StopTrigger.WaitForFire();
		}

		#endregion
	}
}
