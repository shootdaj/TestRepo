using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ZoneLighting;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	public class SyncContextTests
	{
		[Test]
		public void SyncAndStart_TwoSteppers_Works()
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

			//cleanup
			stepperA.Dispose(true);
			stepperB.Dispose(true);
			testContext.Dispose();

			//assert
			if (result)
				Assert.Pass();
			else
				Assert.Fail("The two programs are not within one step of each other --> " + stepperA.Name + "=" + stepperSteps[0] + ":" +
							stepperB.Name + "=" + stepperSteps[1]);
		}

		[Test]
		public void SyncAndStart_FourSteppers_Works()
		{
			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new Stepper("a");
			var stepperB = new Stepper("b");
			var stepperC = new Stepper("c");
			var stepperD = new Stepper("d");

			//sync and start
			testContext.SyncAndStart(stepperA, stepperB, stepperC, stepperD);

			int[] stepperSteps;
			var result = ValidateStepperSyncPhase(out stepperSteps, stepperA, stepperB, stepperC, stepperD);

			//cleanup
			stepperA.Dispose(true);
			stepperB.Dispose(true);
			stepperC.Dispose(true);
			stepperD.Dispose(true);
			testContext.Dispose();

			//assert
			if (result)
				Assert.Pass();
			else
				Assert.Fail("The programs are not within one step of each other --> " + stepperA.Name + "=" + stepperSteps[0] + ":" +
							stepperB.Name + "=" + stepperSteps[1] + ":" +
							stepperC.Name + "=" + stepperSteps[2] + ":" +
							stepperD.Name + "=" + stepperSteps[3]);
		}

		[Test]
		public void SyncAndStartLive_TwoSteppers_Works()
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

			//cleanup
			stepperA.Dispose(true);
			stepperB.Dispose(true);
			testContext.Dispose();

			//assert
			if (result)
				Assert.Pass();
			else
				Assert.Fail("The two programs are not within one step of each other --> " + stepperA.Name + "=" + stepperSteps[0] + ":" +
							stepperB.Name + "=" + stepperSteps[1]);
		}

		//[TestCase(60)]
		[Test]
		public void SyncAndStartLive_OneStepperSyncingWithThree_Works()
		{
			//var timeoutSeconds = 30;
			//var task = Task.Factory.StartNew(() =>
			//{

			DebugTools.AddEvent("SyncAndStartLive_OneStepperSyncingWithThree_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("A");
			var stepperB = new StepperInternalLoop("B");
			var stepperC = new StepperInternalLoop("C");
			var stepperD = new StepperInternalLoop("D");

			//sync and start ABC
			DebugTools.AddEvent("SyncAndStartLive_OneStepperSyncingWithThree_Works", "Sync-Starting Stepper A, B, C");
			testContext.SyncAndStart(stepperA, stepperB, stepperC);

			//start D as a live sync with testContext when steppers ABC is back to the beginning
			DebugTools.AddEvent("SyncAndStartLive_OneStepperSyncingWithThree_Works", "LiveSync-Starting Stepper D");
			testContext.SyncAndStartLive(stepperD);

			int[] stepperSteps;
			DebugTools.AddEvent("SyncAndStartLive_OneStepperSyncingWithThree_Works", "Validating Sync Phases");
			var result = ValidateStepperSyncPhase(out stepperSteps, stepperA, stepperB, stepperC, stepperD);

			//cleanup
			DebugTools.AddEvent("SyncAndStartLive_OneStepperSyncingWithThree_Works", "Disposing Stepper A");
			stepperA.Dispose(true);
			DebugTools.AddEvent("SyncAndStartLive_OneStepperSyncingWithThree_Works", "Disposing Stepper B");
			stepperB.Dispose(true);
			DebugTools.AddEvent("SyncAndStartLive_OneStepperSyncingWithThree_Works", "Disposing Stepper C");
			stepperC.Dispose(true);
			DebugTools.AddEvent("SyncAndStartLive_OneStepperSyncingWithThree_Works", "Disposing Stepper D");
			stepperD.Dispose(true);
			testContext.Dispose();

			DebugTools.Print();

			if (result)
			{
				Assert.Pass();
			}
			else
			{
				Assert.Fail("The programs are not within one step of each other --> " + stepperA.Name + "=" + stepperSteps[0] + ":" +
				            stepperB.Name + "=" + stepperSteps[1] + ":" +
				            stepperC.Name + "=" + stepperSteps[2] + ":" +
				            stepperD.Name + "=" + stepperSteps[3]);
			}

			//});

			//t.Start();
			//if (!t.Join(TimeSpan.FromSeconds(30)))
			//{
			//	t.Abort();
			//}



			//Task.WaitAny(new[]{task.ContinueWith(task1 =>
			//{
			//	DebugTools.Print();
			//	Assert.Fail();
			//})}, TimeSpan.FromSeconds(timeoutSeconds)).;

			//int timeout = 1000;

			//if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
			//{
			//	// task completed within timeout
			//}
			//else
			//{
			//	// timeout logic
			//}
		}

		[Test]
		[Ignore]
		public void SyncAndStartLive_TwoStepperSyncingWithTwo_Works()
		{
			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("a");
			var stepperB = new StepperInternalLoop("b");
			var stepperC = new StepperInternalLoop("c");
			var stepperD = new StepperInternalLoop("d");

			//sync and start AB
			testContext.SyncAndStart(stepperA, stepperB);

			//start CD as a live sync with testContext when steppers AB is back to the beginning
			testContext.SyncAndStartLive(stepperC);
			testContext.SyncAndStartLive(stepperD);

			int[] stepperSteps;
			var result = ValidateStepperSyncPhase(out stepperSteps, stepperA, stepperB, stepperC, stepperD);

			//cleanup
			stepperA.Dispose(true);
			stepperB.Dispose(true);
			stepperC.Dispose(true);
			stepperD.Dispose(true);
			testContext.Dispose();

			//assert
			if (result)
				Assert.Pass();
			else
				Assert.Fail("The programs are not within one step of each other --> " + stepperA.Name + "=" + stepperSteps[0] + ":" +
							stepperB.Name + "=" + stepperSteps[1] + ":" +
							stepperC.Name + "=" + stepperSteps[2] + ":"
							+ stepperD.Name + "=" + stepperSteps[3]
							);
		}

		[Test]
		public void SyncAndStartLive_ThreeStepperSyncingWithOne_Works()
		{
			//var task = Task.Factory.StartNew(() =>
			//{
			DebugTools.AddEvent("SyncAndStartLive_ThreeStepperSyncingWithOne_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("A");
			var stepperB = new StepperInternalLoop("B");
			var stepperC = new StepperInternalLoop("C");
			var stepperD = new StepperInternalLoop("D");

			//sync and start A
			DebugTools.AddEvent("SyncAndStartLive_ThreeStepperSyncingWithOne_Works", "Sync-Starting Stepper A");
			testContext.SyncAndStart(stepperA);

			//start BCD as a live sync with testContext when steppers A is back to the beginning
			DebugTools.AddEvent("SyncAndStartLive_ThreeStepperSyncingWithOne_Works", "LiveSync-Starting Stepper B");
			testContext.SyncAndStartLive(stepperB);
			DebugTools.AddEvent("SyncAndStartLive_ThreeStepperSyncingWithOne_Works", "LiveSync-Starting Stepper C");
			testContext.SyncAndStartLive(stepperC);
			DebugTools.AddEvent("SyncAndStartLive_ThreeStepperSyncingWithOne_Works", "LiveSync-Starting Stepper D");
			testContext.SyncAndStartLive(stepperD);

			int[] stepperSteps;
			DebugTools.AddEvent("SyncAndStartLive_ThreeStepperSyncingWithOne_Works", "Validating Sync Phases");
			var result = ValidateStepperSyncPhase(out stepperSteps, stepperA, stepperB, stepperC, stepperD);

			//cleanup
			DebugTools.AddEvent("SyncAndStartLive_ThreeStepperSyncingWithOne_Works", "Disposing Stepper A");
			stepperA.Dispose(true);
			DebugTools.AddEvent("SyncAndStartLive_ThreeStepperSyncingWithOne_Works", "Disposing Stepper B");
			stepperB.Dispose(true);
			DebugTools.AddEvent("SyncAndStartLive_ThreeStepperSyncingWithOne_Works", "Disposing Stepper C");
			stepperC.Dispose(true);
			DebugTools.AddEvent("SyncAndStartLive_ThreeStepperSyncingWithOne_Works", "Disposing Stepper D");
			stepperD.Dispose(true);
			testContext.Dispose();

			//assert
			if (result)
			{
				Assert.Pass();
			}
			else
			{
				DebugTools.Print();

				Assert.Fail("The programs are not within one step of each other --> " + stepperA.Name + "=" + stepperSteps[0] + ":" +
				            stepperB.Name + "=" + stepperSteps[1] + ":" +
				            stepperC.Name + "=" + stepperSteps[2] + ":"
				            + stepperD.Name + "=" + stepperSteps[3]);
			}


			//});

			//Task.WaitAny(new[] { task }, TimeSpan.FromSeconds(20));
			//DebugTools.Print();
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
			for (int i = 0; i < 30; i++)
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
				Thread.Sleep(1);
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
					//Debug.Print(this.Name + " --");
					CurrentStep = StartStep;
					break;
				}
				else
				{
					//Debug.Print(this.Name + " ++");
					CurrentStep++;
				}

				SyncContext?.SignalAndWait();
				//Debug.Print(this.Name + " " + CurrentStep);
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
