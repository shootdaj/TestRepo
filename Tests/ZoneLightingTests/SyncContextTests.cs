using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using ZoneLighting;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	public class SyncContextTests
	{
		[TestCase(1000)]
		[Timeout(30000)]
		public void Sync_OneStepperSyncingWithThree_Works(int numberOfChecks)
		{
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("A");
			var stepperB = new StepperInternalLoop("B");
			var stepperC = new StepperInternalLoop("C");
			var stepperD = new StepperInternalLoop("D");
			Stepper[] steppers = { stepperA, stepperB, stepperC, stepperD };

			//sync and start ABC
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Sync-Starting Stepper A, B, C");
			testContext.Sync(stepperA, stepperB, stepperC);
			testContext.SyncFinished.WaitForFire();

			//start D in sync with testContext when steppers ABC is back to the beginning
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Syncing Stepper D");
			testContext.Sync(stepperD);
			testContext.SyncFinished.WaitForFire();

			int[,] stepperSteps;
			var invalidStepIndex = ValidateStepperSyncPhase(steppers, out stepperSteps, numberOfChecks);
			var result = invalidStepIndex.Length == 0;

			//cleanup
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Disposing Stepper A");
			stepperA.Dispose(true);
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Disposing Stepper B");
			stepperB.Dispose(true);
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Disposing Stepper C");
			stepperC.Dispose(true);
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Disposing Stepper D");
			stepperD.Dispose(true);
			testContext.Dispose();

			if (result)
			{
				Assert.Pass();
			}
			else
			{
				var failStringBuilder = BuildFailString(invalidStepIndex, steppers, stepperSteps);
				Assert.Fail(failStringBuilder.ToString());
			}
		}

		[TestCase(1000)]
		[Timeout(30000)]
		public void Sync_ThreeStepperSyncingWithOne_Consecutive_Works(int numberOfChecks)
		{
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("A");
			var stepperB = new StepperInternalLoop("B");
			var stepperC = new StepperInternalLoop("C");
			var stepperD = new StepperInternalLoop("D");
			Stepper[] steppers = { stepperA, stepperB, stepperC, stepperD };

			//sync and start A
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Sync-Starting Stepper A");
			testContext.Sync(stepperA);
			testContext.SyncFinished.WaitForFire();

			//start BCD in sync with testContext when steppers A is back to the beginning
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Syncing Stepper B");
			testContext.Sync(stepperB);
			testContext.SyncFinished.WaitForFire();
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Syncing Stepper C");
			testContext.Sync(stepperC);
			testContext.SyncFinished.WaitForFire();
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Syncing Stepper D");
			testContext.Sync(stepperD);
			testContext.SyncFinished.WaitForFire();

			int[,] stepperSteps;
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Validating Sync Phases");
			var invalidStepIndex = ValidateStepperSyncPhase(steppers, out stepperSteps, numberOfChecks);
			var result = invalidStepIndex.Length == 0;

			//cleanup
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Disposing Stepper A");
			stepperA.Dispose(true);
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Disposing Stepper B");
			stepperB.Dispose(true);
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Disposing Stepper C");
			stepperC.Dispose(true);
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Disposing Stepper D");
			stepperD.Dispose(true);
			testContext.Dispose();

			//assert
			if (result)
			{
				Assert.Pass();
			}
			else
			{
				var failStringBuilder = BuildFailString(invalidStepIndex, steppers, stepperSteps);
				Assert.Fail(failStringBuilder.ToString());
			}
			
		}

		[TestCase(1000)]
		[Timeout(30000)]
		public void Sync_TwoStepperSyncingWithTwo_Works(int numberOfChecks)
		{
			DebugTools.AddEvent("Sync_TwoStepperSyncingWithTwo_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("a");
			var stepperB = new StepperInternalLoop("b");
			var stepperC = new StepperInternalLoop("c");
			var stepperD = new StepperInternalLoop("d");
			Stepper[] steppers = { stepperA, stepperB, stepperC, stepperD };

			//sync and start AB
			testContext.Sync(stepperA, stepperB);
			testContext.SyncFinished.WaitForFire();

			//start CD in sync with testContext when steppers AB is back to the beginning
			DebugTools.AddEvent("Sync_TwoStepperSyncingWithTwo_Works", "Syncing Stepper C");
			testContext.Sync(stepperC);
			testContext.SyncFinished.WaitForFire();
			DebugTools.AddEvent("Sync_TwoStepperSyncingWithTwo_Works", "Syncing Stepper D");
			testContext.Sync(stepperD);
			testContext.SyncFinished.WaitForFire();

			int[,] stepperSteps;
			var invalidStepIndex = ValidateStepperSyncPhase(steppers, out stepperSteps, numberOfChecks);
			var result = invalidStepIndex.Length == 0;

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
			{
				var failStringBuilder = BuildFailString(invalidStepIndex, steppers, stepperSteps);
				Assert.Fail(failStringBuilder.ToString());
			}
		}

		[TestCase(1000)]
		[Timeout(30000)]
		public void Sync_TwoSteppersInternalLoop_Consecutive_Works(int numberOfChecks)
		{
			DebugTools.AddEvent("Sync_TwoSteppers_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("A");
			var stepperB = new StepperInternalLoop("B");
			Stepper[] steppers = { stepperA, stepperB };

			//sync and start A
			DebugTools.AddEvent("Sync_TwoSteppers_Works", "Sync-Starting Stepper A");
			testContext.Sync(stepperA);
			testContext.SyncFinished.WaitForFire();

			//start B in sync with testContext when stepper A is back to the beginning
			DebugTools.AddEvent("Sync_TwoSteppers_Works", "Syncing Stepper B");
			testContext.Sync(stepperB);
			testContext.SyncFinished.WaitForFire();

			int[,] stepperSteps;
			var invalidStepIndex = ValidateStepperSyncPhase(steppers, out stepperSteps, numberOfChecks);
			var result = invalidStepIndex.Length == 0;

			PrintStepperSteps(steppers, stepperSteps);

			//cleanup
			stepperA.Dispose(true);
			stepperB.Dispose(true);
			testContext.Dispose();

			//assert
			if (result)
				Assert.Pass();
			else
			{
				var failStringBuilder = BuildFailString(invalidStepIndex, steppers, stepperSteps);
				Assert.Fail(failStringBuilder.ToString());
			}
		}

		[TestCase(1000)]
		[Timeout(30000)]
		public void Sync_FourSteppers_Simultaneous_Works(int numberOfChecks)
		{
			DebugTools.AddEvent("Sync_FourSteppers_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new Stepper("A");
			var stepperB = new Stepper("B");
			var stepperC = new Stepper("C");
			var stepperD = new Stepper("D");
			Stepper[] steppers = { stepperA, stepperB, stepperC, stepperD };

			//sync and start
			testContext.Sync(stepperA, stepperB, stepperC, stepperD);
			testContext.SyncFinished.WaitForFire();

			int[,] stepperSteps;
			var invalidStepIndex = ValidateStepperSyncPhase(steppers, out stepperSteps, numberOfChecks);
			var result = invalidStepIndex.Length == 0;

			PrintStepperSteps(steppers, stepperSteps);

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
			{
				var failStringBuilder = BuildFailString(invalidStepIndex, steppers, stepperSteps);
				Assert.Fail(failStringBuilder.ToString());
			}	
		}

		[TestCase(1000)]
		[Timeout(30000)]
		public void Sync_TwoSteppers_Simultaneous_Works(int numberOfChecks)
		{
			DebugTools.AddEvent("Sync_TwoSteppers_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new Stepper("A");
			var stepperB = new Stepper("B");
			Stepper[] steppers = {stepperA, stepperB};

			//sync and start
			testContext.Sync(stepperA, stepperB);
			testContext.SyncFinished.WaitForFire();

			int[,] stepperSteps;
			var invalidStepIndex = ValidateStepperSyncPhase(steppers, out stepperSteps, numberOfChecks);
			var result = invalidStepIndex.Length == 0;

			PrintStepperSteps(steppers, stepperSteps);

			//cleanup
			stepperA.Dispose(true);
			stepperB.Dispose(true);
			testContext.Dispose();

			//assert
			if (result)
				Assert.Pass();
			else
			{
				var failStringBuilder = BuildFailString(invalidStepIndex, steppers, stepperSteps);
				Assert.Fail(failStringBuilder.ToString());
			}
		}

		[TestCase(100)]
		[Timeout(30000)]
		public void ZoneProgram_StartWithSync_OneStepperSyncingWithThree_Works(int numberOfChecks)
		{
			DebugTools.AddEvent("ZoneProgram_StartWithSync_OneStepperSyncingWithThree_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("A");
			var stepperB = new StepperInternalLoop("B");
			var stepperC = new StepperInternalLoop("C");
			var stepperD = new StepperInternalLoop("D");
			Stepper[] steppers = { stepperA, stepperB, stepperC, stepperD };

			//sync and start ABC
			DebugTools.AddEvent("ZoneProgram_StartWithSync_OneStepperSyncingWithThree_Works", "Sync-Starting Stepper A, B, C");
			testContext.Sync(stepperA, stepperB, stepperC);
			testContext.SyncFinished.WaitForFire();

			//start D as a sync with testContext when steppers ABC is back to the beginning
			DebugTools.AddEvent("ZoneProgram_StartWithSync_OneStepperSyncingWithThree_Works", "Syncing Stepper D");
			stepperD.Start(sync: true, syncContext:testContext);
			testContext.SyncFinished.WaitForFire();

			int[,] stepperSteps;
			var invalidStepIndex = ValidateStepperSyncPhase(steppers, out stepperSteps, numberOfChecks);
			var result = invalidStepIndex.Length == 0;

			//cleanup
			DebugTools.AddEvent("ZoneProgram_StartWithSync_OneStepperSyncingWithThree_Works", "Disposing Stepper A");
			stepperA.Dispose(true);
			DebugTools.AddEvent("ZoneProgram_StartWithSync_OneStepperSyncingWithThree_Works", "Disposing Stepper B");
			stepperB.Dispose(true);
			DebugTools.AddEvent("ZoneProgram_StartWithSync_OneStepperSyncingWithThree_Works", "Disposing Stepper C");
			stepperC.Dispose(true);
			DebugTools.AddEvent("ZoneProgram_StartWithSync_OneStepperSyncingWithThree_Works", "Disposing Stepper D");
			stepperD.Dispose(true);
			testContext.Dispose();

			if (result)
			{
				Assert.Pass();
			}
			else
			{
				var failStringBuilder = BuildFailString(invalidStepIndex, steppers, stepperSteps);
				Assert.Fail(failStringBuilder.ToString());
			}
		}

		[TestCase(100)]
		[Timeout(30000)]
		public void ZoneProgram_StartWithSync_StepperConstructorSyncContext_OneStepperSyncingWithThree_Works(int numberOfChecks)
		{
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("A");
			var stepperB = new StepperInternalLoop("B");
			var stepperC = new StepperInternalLoop("C");
			var stepperD = new StepperInternalLoop("D", testContext);
			Stepper[] steppers = { stepperA, stepperB, stepperC, stepperD };

			//sync and start ABC
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Sync-Starting Stepper A, B, C");
			testContext.Sync(stepperA, stepperB, stepperC);
			testContext.SyncFinished.WaitForFire();

			//start D in sync with testContext when steppers ABC is back to the beginning
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Syncing Stepper D");
			stepperD.Start(sync: true);
			testContext.SyncFinished.WaitForFire();

			int[,] stepperSteps;
			var invalidStepIndex = ValidateStepperSyncPhase(steppers, out stepperSteps, numberOfChecks);
			var result = invalidStepIndex.Length == 0;

			//cleanup
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Disposing Stepper A");
			stepperA.Dispose(true);
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Disposing Stepper B");
			stepperB.Dispose(true);
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Disposing Stepper C");
			stepperC.Dispose(true);
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Disposing Stepper D");
			stepperD.Dispose(true);
			testContext.Dispose();

			if (result)
			{
				Assert.Pass();
			}
			else
			{
				var failStringBuilder = BuildFailString(invalidStepIndex, steppers, stepperSteps);
				Assert.Fail(failStringBuilder.ToString());
			}
		}
		
		[TestCase(100)]
		[Timeout(30000)]
		public void Sync_OneStepper_Works(int numberOfChecks)
		{
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("A");
			Stepper[] steppers = { stepperA };

			//sync and start ABC
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Sync-Starting Stepper A, B, C");
			testContext.Sync(stepperA);
			testContext.SyncFinished.WaitForFire();
			
			int[,] stepperSteps;
			var invalidStepIndex = ValidateStepperSyncPhase(steppers, out stepperSteps, numberOfChecks);
			var result = invalidStepIndex.Length == 0 && stepperSteps.Length != 0;

			//cleanup
			DebugTools.AddEvent("Sync_OneStepperSyncingWithThree_Works", "Disposing Stepper A");
			stepperA.Dispose(true);
			testContext.Dispose();

			if (result)
			{
				Assert.Pass();
			}
			else
			{
				var failStringBuilder = BuildFailString(invalidStepIndex, steppers, stepperSteps);
				Assert.Fail(failStringBuilder.ToString());
			}
		}

		[TestCase(100)]
		[Timeout(30000)]
		public void Sync_SimultaneousSync_ThreeStepperSyncingWithOne_Simultaneous_Works(int numberOfChecks)
		{
			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("A");
			var stepperB = new StepperInternalLoop("B");
			var stepperC = new StepperInternalLoop("C");
			var stepperD = new StepperInternalLoop("D");
			Stepper[] steppers = { stepperA, stepperB, stepperC, stepperD };

			//sync and start A
			testContext.Sync(stepperA);
			testContext.SyncFinished.WaitForFire();

			//start BCD in sync with testContext when steppers A is back to the beginning
			testContext.Sync(stepperB, stepperC, stepperD);
			testContext.SyncFinished.WaitForFire();

			int[,] stepperSteps;
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Validating Sync Phases");
			var invalidStepIndex = ValidateStepperSyncPhase(steppers, out stepperSteps, numberOfChecks);
			var result = invalidStepIndex.Length == 0;

			//cleanup
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Disposing Stepper A");
			stepperA.Dispose(true);
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Disposing Stepper B");
			stepperB.Dispose(true);
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Disposing Stepper C");
			stepperC.Dispose(true);
			DebugTools.AddEvent("Sync_ThreeStepperSyncingWithOne_Works", "Disposing Stepper D");
			stepperD.Dispose(true);
			testContext.Dispose();

			//assert
			if (result)
			{
				Assert.Pass();
			}
			else
			{
				var failStringBuilder = BuildFailString(invalidStepIndex, steppers, stepperSteps);
				Assert.Fail(failStringBuilder.ToString());
			}
		}

		#region Helpers

		private static StringBuilder BuildFailString(int[] invalidStepIndex, Stepper[] steppers, int[,] stepperSteps)
		{
			var failStringBuilder = new StringBuilder();
			failStringBuilder.Append("The programs are not within one step of each other:");
			failStringBuilder.Append(Environment.NewLine);
			for (var i = 0; i < invalidStepIndex.Length; i++)
			{
				for (var j = 0; j < steppers.Length; j++)
				{
					failStringBuilder.Append(steppers[j].Name + "stepperSteps[" + invalidStepIndex[i] + "," + j + "]" + "=" +
											 stepperSteps[invalidStepIndex[i], j]);
					if (j + 1 != steppers.Length)
						failStringBuilder.Append(" | ");
				}
				failStringBuilder.Append(Environment.NewLine);
			}
			return failStringBuilder;
		}

		private static void PrintStepperSteps(Stepper[] steppers, int[,] stepperSteps)
		{
			//output stepperSteps
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Environment.NewLine);

			foreach (var stepper in steppers)
			{
				stringBuilder.Append("   ");
				stringBuilder.Append(stepper.Name);
				stringBuilder.Append("   ");
				stringBuilder.Append("|");
			}

			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Environment.NewLine);

			foreach (var stepper in steppers)
			{
				stringBuilder.Append("--------");
			}

			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Environment.NewLine);

			for (var i = 0; i < stepperSteps.GetLength(0); i++)
			{
				for (var j = 0; j < stepperSteps.GetLength(1); j++)
				{
					stringBuilder.Append("   ");
					stringBuilder.Append(stepperSteps[i, j]);
					stringBuilder.Append("   ");
					stringBuilder.Append("|");
				}

				stringBuilder.Append(Environment.NewLine);
			}

			DebugTools.AddEvent("Sync_TwoSteppers_Works", stringBuilder.ToString());
		}

		/// <summary>
		/// Checks to make sure that the steppers provided are in within 1 step of each other.
		/// </summary>
		private static int[] ValidateStepperSyncPhase(Stepper[] steppers, out int[,] stepperSteps, int numberOfChecks = 30, int msToWaitBeforeStart = 10, int msToWaitBetweenChecks = 1)
		{
			//sleep cuz we want the programs to get going 
			Thread.Sleep(msToWaitBeforeStart);

			DebugTools.AddEvent("ValidateStepperSyncPhase", "START");

			List<int> invalidStepIndex = new List<int>();
			stepperSteps = new int[numberOfChecks, steppers.Length];

			for (var i = 0; i < numberOfChecks; i++)
			{
				for (var j = 0; j < steppers.Length; j++)
				{
					stepperSteps[i, j] = 0;
				}
			}

			//every time we check the step, it should be within 1 for both programs
			//because they should have been synced and then started - one program may be ahead of
			//another because this test may read in between the transactions (since this test is not
			//aware of the sync itself, it's reading values intermittently, so being off by one 
			//does not imply that the programs are out of sync. if the programs do go out of sync, 
			//with enough repetitions they will go out of sync even more, which means their steps
			//will eventually differ by more than 1, which is what this for loop is testing. therefore, 
			//the higher the number of checks, the higher the chance of failure, IF the sync algorithm has
			//a race condition or some other kind of flaw.
			for (int i = 0; i < numberOfChecks; i++)
			{
				foreach (var stepper in steppers)
				{
					stepper.PauseForTest = true;
				}
				for (var j = 0; j < steppers.Length; j++)
				{
					stepperSteps[i, j] = steppers[j].CurrentStep;
				}
				foreach (var stepper in steppers)
				{
					stepper.PauseForTest = false;
				}

				//check to make sure the difference in steps in no more than 1 (and check for wrapping)
				for (int comparisonSource = 0; comparisonSource < steppers.Length; comparisonSource++)
				{
					for (int comparisonTarget = 0; comparisonTarget < steppers.Length; comparisonTarget++)
					{
						if (Math.Abs(stepperSteps[i, comparisonSource] - stepperSteps[i, comparisonTarget]) > 1 &&
							!(stepperSteps[i, comparisonSource] == steppers[comparisonSource].EndStep &&
							  stepperSteps[i, comparisonTarget] == steppers[comparisonTarget].StartStep) &&
							!(stepperSteps[i, comparisonSource] == steppers[comparisonSource].StartStep &&
							  stepperSteps[i, comparisonTarget] == steppers[comparisonTarget].EndStep))
						{
							if(!invalidStepIndex.Contains(i))
								invalidStepIndex.Add(i);
						}
					}
				}

				//sleep cuz we want some cycles to execute
				if (msToWaitBetweenChecks > 0)
					Thread.Sleep(msToWaitBetweenChecks);
			}

			DebugTools.AddEvent("ValidateStepperSyncPhase", "END result: " + (!invalidStepIndex.Any()));

			return invalidStepIndex.ToArray();
		}

		#endregion

		[TearDown]
		public void TearDown()
		{
			if (TestContext.CurrentContext.Result.Status != TestStatus.Failed) return;
			DebugTools.AddEvent("TearDown", "DebugTools.Print");
			DebugTools.Print(clearEvents: true);
		}

	}

	public class StepperInternalLoop : Stepper
	{
		public StepperInternalLoop(string name, SyncContext syncContext = null) : base(name, syncContext)
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

	public class Stepper : LoopingZoneProgram
	{
		#region CORE + C + I

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
