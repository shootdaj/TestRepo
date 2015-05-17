﻿using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneProgramNS.Factories;

namespace ZoneLightingTests
{
	public static class TestHelpers
	{
		public static void InitializeZoneScaffolder()
		{
			ZoneScaffolder.Instance.Initialize(ConfigurationManager.AppSettings["TestProgramModuleDirectory"]);
		}

		public static void ValidateSteppersInSync(IEnumerable<IStepper> steppers, int numberOfChecks, int msToWaitBeforeStart = 10, int msToWaitBetweenChecks = 1, bool print = false)
		{
			int[,] stepperSteps;
		    var stepperArray = steppers as IStepper[] ?? steppers.ToArray();
		    var invalidStepIndex = SyncContextTests.ValidateStepperSyncPhase(stepperArray.ToArray(), out stepperSteps, numberOfChecks);
            if (print) SyncContextTests.PrintStepperSteps(stepperArray.ToArray(), stepperSteps);
			Assert.True(invalidStepIndex.Length == 0 && stepperSteps.Length != 0);
		}

		public static void ValidateSteppersRunning(IEnumerable<IStepper> steppers, int numberOfChecks, int msToWaitBeforeStart = 10, int msToWaitBetweenChecks = 1)
		{
			int[,] stepperSteps;
			SyncContextTests.ValidateStepperSyncPhase(steppers.ToArray(), out stepperSteps, numberOfChecks);
			Assert.True(stepperSteps.Length != 0);
		}

		public static void ValidateSteppersOutOfSync(IEnumerable<IStepper> steppers, int numberOfChecks,
			int msToWaitBeforeStart = 10, int msToWaitBetweenChecks = 1, int minOutOfSyncStepsThreshold = 0)
		{
			int[,] stepperSteps;
			var invalidStepIndex = SyncContextTests.ValidateStepperSyncPhase(steppers.ToArray(), out stepperSteps, numberOfChecks);
			Assert.True(invalidStepIndex.Length > minOutOfSyncStepsThreshold);
		}
	}
}
