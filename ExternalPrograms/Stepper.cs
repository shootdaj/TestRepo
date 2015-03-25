using System.ComponentModel.Composition;
using System.Threading;
using ZoneLighting;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "Stepper")]
	public class Stepper : LoopingZoneProgram, IStepper
	{
		#region CORE + C + I

		protected override int MaxSyncTimeout { get; } = 100;

		public override SyncLevel SyncLevel { get; set; } = StepperSyncLevel.Step;

		public int StartStep { get; } = 1;
		public int EndStep { get; set; } = 9;

		private object _currentStepLock = new object();
		public int CurrentStep
		{
			get
			{
				lock (_currentStepLock)
				{
					return _currentStep;
				}
			}
			set
			{
				lock (_currentStepLock)
				{
					_currentStep = value;
				}
			}
		}

		private object _pauseForTestLock = new object();
		private bool _pauseForTest;
		private int _currentStep;

		public bool PauseForTest
		{
			get
			{
				lock (_pauseForTestLock)
				{
					return _pauseForTest;
				}
			}
			set
			{
				lock (_pauseForTestLock)
				{
					_pauseForTest = value;
				}
			}
		}

		public Stepper(string name, SyncContext syncContext = null) : base(name, syncContext)
		{
			Name = name;
		}

		public Stepper()
		{ }

		public override void Setup()
		{
			CurrentStep = StartStep;
		}

		#endregion

		public override void Loop()
		{
			DebugTools.AddEvent("StepperInternalLoop.Loop", "Signalling and waiting on " + (SyncContext?.GetNumberOfRemainingParticipants() - 1) + ". Total participants = " + SyncContext?.GetNumberOfTotalParticipants() + " : " + this.Name);
			SyncContext?.SignalAndWait();
			DebugTools.AddEvent("StepperInternalLoop.Loop", "Signal received. Continuing: " + this.Name);

			if (CurrentStep + 1 > EndStep)
			{
				CurrentStep = StartStep;
			}
			else
			{
				CurrentStep++;
			}

			while (PauseForTest)
			{
				Thread.Sleep(1);
			}

			DebugTools.AddEvent("StepperInternalLoop.Loop", "Signalling and waiting on " + (SyncContext?.GetNumberOfRemainingParticipants() - 1) + ". Total participants = " + SyncContext?.GetNumberOfTotalParticipants() + " : " + this.Name);
			SyncContext?.SignalAndWait();
			DebugTools.AddEvent("StepperInternalLoop.Loop", "Signal received. Continuing: " + this.Name);
		}

		public static class StepperSyncLevel
		{
			public static SyncLevel Step = new SyncLevel("Step");
		}
	}
}