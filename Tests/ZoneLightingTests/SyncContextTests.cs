using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	public class SyncContextTests
	{
		[Test]
		public void SyncAndStart_Works()
		{
			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new Stepper("a");
			var stepperB = new Stepper("b");

			//sync and start
			testContext.SyncAndStart(stepperA, stepperB);

			int[] stepperSteps;
			var result = ValidateStepperSyncPhase(out stepperSteps, stepperA, stepperB);
			var stepperBStep = stepperSteps[0];
			var stepperAStep = stepperSteps[1];

			//cleanup
			stepperA.Stop(true);
			stepperB.Stop(true);

			//assert
			if (result)
				Assert.Pass();
			else
				Assert.Fail("The two programs are not within one step of each other --> " + stepperA.Name + "=" + stepperAStep + ":" +
				            stepperB.Name + "=" + stepperBStep);
		}

		[Test]
		public void SyncAndStartLive_Works()
		{
			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("a");
			var stepperB = new StepperInternalLoop("b");

			//sync and start A
			testContext.SyncAndStart(stepperA);
			
			//start B as a live sync with testContext when stepper A is back to the beginning
			testContext.SyncAndStartLive(stepperB);

			int[] stepperSteps;
			var result = ValidateStepperSyncPhase(out stepperSteps, stepperA, stepperB);
			var stepperBStep = stepperSteps[0];
			var stepperAStep = stepperSteps[1];

			//cleanup
			stepperA.Stop(true);
			stepperB.Stop(true);

			//assert
			if (result)
				Assert.Pass();
			else
				Assert.Fail("The two programs are not within one step of each other --> " + stepperA.Name + "=" + stepperAStep + ":" +
				            stepperB.Name + "=" + stepperBStep);
		}

		/// <summary>
		/// Checks to make sure that the steppers provided are in within 1 step of each other.
		/// </summary>
		/// <param name="stepperSteps"></param>
		/// <param name="steppers"></param>
		/// <returns></returns>
		private static bool ValidateStepperSyncPhase(out int[] stepperSteps, params Stepper[] steppers)
		{
			var result = true;
			stepperSteps = new int[steppers.Length];

			for (var i = 0; i < steppers.Length; i++)
			{
				stepperSteps[i] = 0;
			}

			//every time we check the step, it should be within 1 for both programs
			//because they should have been synced and then started - one program may be ahead of
			//another because this test may read in between the transactions (since this test is not
			//aware of the sync itself, it's reading values intermittently, so being off by one 
			//does not imply that the programs are out of sync. if the programs do go out of sync, 
			//with enough repetitions they will go out of sync even more, which means their steps
			//will eventually differ by more than 1, which is what this for loop is testing
			for (int i = 0; i < 100; i++)
			{
				for (var x = 0; x < steppers.Length; x++)
				{
					stepperSteps[x] = steppers[x].CurrentStep;
				}

				//check to make sure the difference in steps in no more than 1 (and check for wrapping)
				for (int comparisonSource = 0; comparisonSource < steppers.Length; comparisonSource++)
				{
					for (int comparisonTarget = 0; comparisonTarget < steppers.Length; comparisonTarget++)
					{
						if (Math.Abs(stepperSteps[comparisonSource] - stepperSteps[comparisonTarget]) > 1 &&
							!(stepperSteps[comparisonSource] == steppers[comparisonSource].EndStep &&
							  stepperSteps[comparisonTarget] == steppers[comparisonTarget].StartStep) &&
							!(stepperSteps[comparisonSource] == steppers[comparisonSource].StartStep &&
							  stepperSteps[comparisonTarget] == steppers[comparisonTarget].EndStep))
						{
							result = false;
							break;
						}
					}
				}

				//sleep cuz we want some cycles of execution to happen in the programs
				Thread.Sleep(10);
			}

			return result;
		}

	}

	public class StepperInternalLoop : Stepper
	{
		public StepperInternalLoop(string name) : base(name)
		{
		}

		public override void Loop()
		{
			SyncContext?.SignalAndWait();

			while (true)
			{
				if (CurrentStep + 1 > EndStep)
				{
					Debug.Print(this.Name + " --");
					CurrentStep = StartStep;
					break;
				}
				else
				{
					Debug.Print(this.Name + " ++");
					CurrentStep++;
				}

				SyncContext?.SignalAndWait();
				Debug.Print(this.Name + " " + CurrentStep);
			}

			SyncContext?.SignalAndWait();
		}
	}

	public class Stepper : LoopingZoneProgram
	{
		public override SyncLevel SyncLevel { get; set; } = StepperSyncLevel.Step;

		public int StartStep { get; set; } = 1;
		public int EndStep { get; set; } = 10;
		public int CurrentStep;

		public Stepper(string name)
		{
			Name = name;
		}

		public override void Setup()
		{
			CurrentStep = StartStep;
		}

		public override void Loop()
		{
			SyncContext?.SignalAndWait();

			if (CurrentStep + 1 > EndStep)
			{
				Debug.Print(this.Name + " --");
				CurrentStep = StartStep;
			}
			else
			{
				Debug.Print(this.Name + " ++");
				CurrentStep++;
			}

			SyncContext?.SignalAndWait();
			Debug.Print(this.Name + " " + CurrentStep);
		}

		public static class StepperSyncLevel
		{
			public static SyncLevel Step = new SyncLevel("Step");
		}
	}
}
