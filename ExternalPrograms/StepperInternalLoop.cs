using System.ComponentModel.Composition;
using System.Threading;
using ZoneLighting;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "StepperInternalLoop")]
	public class StepperInternalLoop : Stepper
	{
		public StepperInternalLoop(string name, SyncContext syncContext = null) : base(name, syncContext)
		{

		}
		public StepperInternalLoop()
		{

		}

		public override void Loop()
		{
			DebugTools.AddEvent("StepperInternalLoop.Loop", "Loop Start. Signalling and waiting on " + (SyncContext?.GetNumberOfRemainingParticipants() - 1) + ". Total participants = " + SyncContext?.GetNumberOfTotalParticipants() + " : " + this.Name);
			SyncContext?.SignalAndWait();
			//DebugTools.AddEvent("StepperInternalLoop.Loop", "Loop Start. Signal received. Continuing: " + this.Name);

			while (true)
			{
				if (CurrentStep + 1 > EndStep)
				{
					CurrentStep = StartStep;
					break;
				}
				else
				{
					CurrentStep++;
				}

				while (PauseForTest)
				{
					Thread.Sleep(1);
				}

				DebugTools.AddEvent("StepperInternalLoop.Loop", "In While. Signalling and waiting on " + (SyncContext?.GetNumberOfRemainingParticipants() - 1) + ". Total participants = " + SyncContext?.GetNumberOfTotalParticipants() + " : " + this.Name);
				SyncContext?.SignalAndWait();
				//DebugTools.AddEvent("StepperInternalLoop.Loop", "In While. Signal received. Continuing: " + this.Name);
			}

			DebugTools.AddEvent("StepperInternalLoop.Loop", "Loop End. Signalling and waiting on " + (SyncContext?.GetNumberOfRemainingParticipants() - 1) + ". Total participants = " + SyncContext?.GetNumberOfTotalParticipants() + " : " + this.Name);
			SyncContext?.SignalAndWait();
			//DebugTools.AddEvent("StepperInternalLoop.Loop", "Loop End. Signal received. Continuing: " + this.Name);
		}
	}
}