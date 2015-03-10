using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
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
		[TestCase(30)]
		public void SyncAndStartLive_OneStepperSyncingWithThree_Works(int timeout)
		{
			RunTimeboundTest(new Thread(() =>
			{
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

			}), timeout);
		}

		[TestCase(30)]
		public void SyncAndStartLive_ThreeStepperSyncingWithOne_Works(int timeout)
		{
			RunTimeboundTest(new Thread(() =>
			{
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
			}), timeout);
		}

		[TestCase(30)]
		public void SyncAndStartLive_TwoStepperSyncingWithTwo_Works(int timeout)
		{
			RunTimeboundTest(new Thread(() =>
			{
				DebugTools.AddEvent("SyncAndStartLive_TwoStepperSyncingWithTwo_Works", "START");

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
			}), timeout);

		}
		
		[TestCase(30)]
		public void SyncAndStartLive_TwoSteppers_Works(int timeout)
		{
			RunTimeboundTest(new Thread(() =>
			{
				DebugTools.AddEvent("SyncAndStartLive_TwoSteppers_Works", "START");

				//create a sync context to conduct the test with
				var testContext = new SyncContext();

				//create two programs to be synced
				var stepperA = new StepperInternalLoop("A");
				var stepperB = new StepperInternalLoop("B");

				//sync and start A
				DebugTools.AddEvent("SyncAndStartLive_TwoSteppers_Works", "Sync-Starting Stepper A");
				testContext.SyncAndStart(stepperA);

				//start B as a live sync with testContext when stepper A is back to the beginning
				DebugTools.AddEvent("SyncAndStartLive_TwoSteppers_Works", "LiveSync-Starting Stepper B");
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
					Assert.Fail("The two programs are not within one step of each other --> " + stepperA.Name + "=" + stepperSteps[0] +
					            ":" +
					            stepperB.Name + "=" + stepperSteps[1]);

			}), timeout);
		}

		[TestCase(30)]
		public void SyncAndStart_FourSteppers_Works(int timeout)
		{
			RunTimeboundTest(new Thread(() =>
			{
				DebugTools.AddEvent("SyncAndStart_FourSteppers_Works", "START");

				//create a sync context to conduct the test with
				var testContext = new SyncContext();

				//create two programs to be synced
				var stepperA = new Stepper("A");
				var stepperB = new Stepper("B");
				var stepperC = new Stepper("C");
				var stepperD = new Stepper("D");

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

			}), timeout);
		}

		[TestCase(30)]
		public void SyncAndStart_TwoSteppers_Works(int timeout)
		{
			RunTimeboundTest(new Thread(() =>
			{
				DebugTools.AddEvent("SyncAndStart_TwoSteppers_Works", "START");

				//create a sync context to conduct the test with
				var testContext = new SyncContext();

				//create two programs to be synced
				var stepperA = new Stepper("A");
				var stepperB = new Stepper("B");

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
					Assert.Fail("The two programs are not within one step of each other --> " + stepperA.Name + "=" + stepperSteps[0] +
								":" +
								stepperB.Name + "=" + stepperSteps[1]);
			}), timeout);
		}



		/// <summary>
		/// Checks to make sure that the steppers provided are in within 1 step of each other.
		/// </summary>
		private static bool ValidateStepperSyncPhase(out int[] stepperSteps, params Stepper[] steppers)
		{
			DebugTools.AddEvent("ValidateStepperSyncPhase", "START");

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

			DebugTools.AddEvent("ValidateStepperSyncPhase", "END result: " + result);

			return result;// && steppers.All(stepper => stepper.StepStateActive);
		}

		private void RunTimeboundTest(Thread testThread, int timeout)
		{
			testThread.Start();
			var didTestFinish = testThread.Join(TimeSpan.FromSeconds(timeout));
			if (!didTestFinish)
			{
				DebugTools.AddEvent("SyncContextTests.RunTimeboundTest", "TEST FAILED - DID NOT FINISH");

				testThread.Abort();
				DebugTools.Print();
				Assert.Fail();
			}
			//i think this should never get called, because it the test finishes, it should never get here because of the asserts
			else
			{
				DebugTools.Print();
			}
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

		public int StartStep { get; } = 1;
		public int EndStep { get; set; } = 10;
		public int CurrentStep { get; set; }

		public Stepper(string name)
		{
			Name = name;
		}

		public override void Setup()
		{
			CurrentStep = StartStep;
		}

		public bool StepStateActive { get; private set; } = false;

		private void Activate()
		{
			if (!StepStateActive)
				StepStateActive = true;
		}

		public override void Loop()
		{
			//Activate();

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
