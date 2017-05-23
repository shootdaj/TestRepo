using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using ZoneLighting;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneNS;
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

		public static void ValidateSteppersNotRunning(IEnumerable<IStepper> steppers, int numberOfChecks, int msToWaitBeforeStart = 10, int msToWaitBetweenChecks = 1)
		{
			int[,] stepperSteps;
			SyncContextTests.ValidateStepperSyncPhase(steppers.ToArray(), out stepperSteps, numberOfChecks);
			Assert.True(stepperSteps.Length == 0);
		}

		public static void ValidateSteppersOutOfSync(IEnumerable<IStepper> steppers, int numberOfChecks,
			int msToWaitBeforeStart = 10, int msToWaitBetweenChecks = 1, int minOutOfSyncStepsThreshold = 0)
		{
			int[,] stepperSteps;
			var invalidStepIndex = SyncContextTests.ValidateStepperSyncPhase(steppers.ToArray(), out stepperSteps, numberOfChecks);
			Assert.True(invalidStepIndex.Length > minOutOfSyncStepsThreshold);
		}

		public static void AddFourZonesAndStepperProgramSetWithSyncToZLM(ZLM zlm)
		{
			var notificationSyncContext = new SyncContext("NotificationContext");

			//add zones
			var zoneA = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "ZoneA", 16);
			var zoneB = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "ZoneB", 16);
			var zoneC = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "ZoneC", 16);
			var zoneD = ZoneScaffolder.Instance.AddFadeCandyZone(zlm.Zones, "ZoneD", 16);

			zlm.CreateProgramSet("StepperSet", "Stepper", true, null, zlm.Zones);

			//setup interrupting inputs - in the real code this method should not be used. The ZoneScaffolder.AddInterruptingProgram should be used.
			zoneA.AddInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);
			zoneB.AddInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);
			zoneC.AddInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);
			zoneD.AddInterruptingProgram(new BlinkColorReactive(), null, notificationSyncContext);

			//synchronize and start interrupting programs
			notificationSyncContext.Sync(zoneA.InterruptingPrograms[0],
				zoneB.InterruptingPrograms[0],
				zoneC.InterruptingPrograms[0],
				zoneD.InterruptingPrograms[0]);

			zoneA.InterruptingPrograms[0].Start();
			zoneB.InterruptingPrograms[0].Start();
			zoneC.InterruptingPrograms[0].Start();
			zoneD.InterruptingPrograms[0].Start();
		}

		public static Action AddFourZonesAndStepperProgramSetWithSyncToZLMAction(ZLM zlm)
		{
			return () =>
			{
				AddFourZonesAndStepperProgramSetWithSyncToZLM(zlm);
			};
		}
	}
}
