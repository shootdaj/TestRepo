using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ZoneLighting;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLightingTests
{
	public class SyncContextTests
	{
		[TestCase(1000)]
		[Timeout(30000)]
		public void SyncAndStartLive_OneStepperSyncingWithThree_Works(int numberOfChecks)
		{
			//RunTimeboundTest(new Thread(() =>
			//{
			DebugTools.AddEvent("SyncAndStartLive_OneStepperSyncingWithThree_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("A");
			var stepperB = new StepperInternalLoop("B");
			var stepperC = new StepperInternalLoop("C");
			var stepperD = new StepperInternalLoop("D");
			Stepper[] steppers = { stepperA, stepperB, stepperC, stepperD };

			//sync and start ABC
			DebugTools.AddEvent("SyncAndStartLive_OneStepperSyncingWithThree_Works", "Sync-Starting Stepper A, B, C");
			testContext.SyncAndStart(stepperA, stepperB, stepperC);

			//start D as a live sync with testContext when steppers ABC is back to the beginning
			DebugTools.AddEvent("SyncAndStartLive_OneStepperSyncingWithThree_Works", "LiveSync-Starting Stepper D");
			testContext.SyncAndStartLive(stepperD);

			int[,] stepperSteps;
			var invalidStepIndex = ValidateStepperSyncPhase(steppers, out stepperSteps, numberOfChecks);
			var result = invalidStepIndex.Length == 0;

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
				var failStringBuilder = BuildFailString(invalidStepIndex, steppers, stepperSteps);
				Assert.Fail(failStringBuilder.ToString());

				//Assert.Fail("The programs are not within one step of each other --> "
				//			+ stepperA.Name + "stepperSteps[" + invalidStepIndex[i] + ",0" + "]" + "=" + stepperSteps[invalidStepIndex, 0]
				//			+ " | "
				//			+ stepperB.Name + "stepperSteps[" + invalidStepIndex + ",1" + "]" + "=" + stepperSteps[invalidStepIndex, 1]
				//			+ " | "
				//			+ stepperC.Name + "stepperSteps[" + invalidStepIndex + ",2" + "]" + "=" + stepperSteps[invalidStepIndex, 2]
				//			+ " | "
				//			+ stepperD.Name + "stepperSteps[" + invalidStepIndex + ",3" + "]" + "=" + stepperSteps[invalidStepIndex, 3]);
			}

			//}), timeout);
		}

		[TestCase(1000)]
		[Timeout(30000)]
		public void SyncAndStartLive_ThreeStepperSyncingWithOne_Works(int numberOfChecks)
		{
			//RunTimeboundTest(new Thread(() =>
			//{
			DebugTools.AddEvent("SyncAndStartLive_ThreeStepperSyncingWithOne_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("A");
			var stepperB = new StepperInternalLoop("B");
			var stepperC = new StepperInternalLoop("C");
			var stepperD = new StepperInternalLoop("D");
			Stepper[] steppers = { stepperA, stepperB, stepperC, stepperD };

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

			int[,] stepperSteps;
			DebugTools.AddEvent("SyncAndStartLive_ThreeStepperSyncingWithOne_Works", "Validating Sync Phases");
			var invalidStepIndex = ValidateStepperSyncPhase(steppers, out stepperSteps, numberOfChecks);
			var result = invalidStepIndex.Length == 0;

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
				var failStringBuilder = BuildFailString(invalidStepIndex, steppers, stepperSteps);
				Assert.Fail(failStringBuilder.ToString());
			}
			//}), timeout);
		}

		[TestCase(1000)]
		[Timeout(30000)]
		public void SyncAndStartLive_TwoStepperSyncingWithTwo_Works(int numberOfChecks)
		{
			//RunTimeboundTest(new Thread(() =>
			//{
			DebugTools.AddEvent("SyncAndStartLive_TwoStepperSyncingWithTwo_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("a");
			var stepperB = new StepperInternalLoop("b");
			var stepperC = new StepperInternalLoop("c");
			var stepperD = new StepperInternalLoop("d");
			Stepper[] steppers = { stepperA, stepperB, stepperC, stepperD };

			//sync and start AB
			testContext.SyncAndStart(stepperA, stepperB);

			//start CD as a live sync with testContext when steppers AB is back to the beginning
			DebugTools.AddEvent("SyncAndStartLive_TwoStepperSyncingWithTwo_Works", "LiveSync-Starting Stepper C");
			testContext.SyncAndStartLive(stepperC);
			DebugTools.AddEvent("SyncAndStartLive_TwoStepperSyncingWithTwo_Works", "LiveSync-Starting Stepper D");
			testContext.SyncAndStartLive(stepperD);

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
			//}), timeout);

		}

		[TestCase(1000)]
		[Timeout(30000)]
		public void SyncAndStartLive_TwoSteppers_Works(int numberOfChecks)
		{
			//RunTimeboundTest(new Thread(() =>
			//{
			DebugTools.AddEvent("SyncAndStartLive_TwoSteppers_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new StepperInternalLoop("A");
			var stepperB = new StepperInternalLoop("B");
			Stepper[] steppers = { stepperA, stepperB };

			//sync and start A
			DebugTools.AddEvent("SyncAndStartLive_TwoSteppers_Works", "Sync-Starting Stepper A");
			testContext.SyncAndStart(stepperA);

			//start B as a live sync with testContext when stepper A is back to the beginning
			DebugTools.AddEvent("SyncAndStartLive_TwoSteppers_Works", "LiveSync-Starting Stepper B");
			testContext.SyncAndStartLive(stepperB);

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
			//}), timeout);
		}

		[TestCase(1000)]
		[Timeout(30000)]
		public void SyncAndStart_FourSteppers_Works(int numberOfChecks)
		{
			//RunTimeboundTest(new Thread(() =>
			//{
			DebugTools.AddEvent("SyncAndStart_FourSteppers_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new Stepper("A");
			var stepperB = new Stepper("B");
			var stepperC = new Stepper("C");
			var stepperD = new Stepper("D");
			Stepper[] steppers = { stepperA, stepperB, stepperC, stepperD };

			//sync and start
			testContext.SyncAndStart(stepperA, stepperB, stepperC, stepperD);

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
			//}), timeout);
		}

		[TestCase(1000)]
		[Timeout(30000)]
		public void SyncAndStart_TwoSteppers_Works(int numberOfChecks)
		{
			//RunTimeboundTest(new Thread(() =>
			//{
			DebugTools.AddEvent("SyncAndStart_TwoSteppers_Works", "START");

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperA = new Stepper("A");
			var stepperB = new Stepper("B");
			Stepper[] steppers = {stepperA, stepperB};

			//sync and start
			testContext.SyncAndStart(stepperA, stepperB);

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
			//}), timeout);
		}

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

			DebugTools.AddEvent("SyncAndStart_TwoSteppers_Works", stringBuilder.ToString());
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

		[TearDown]
		public void TearDown()
		{
			//if (TestContext.CurrentContext.Result.Status != TestStatus.Failed) return;
			DebugTools.AddEvent("TearDown", "DebugTools.Print");
			DebugTools.Print(clearEvents: true);
		}

	}

	public class StepperInternalLoop : Stepper
	{
		public StepperInternalLoop(string name) : base(name)
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

		public Stepper(string name)
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
