using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.StockPrograms;
using ZoneLighting.ZoneProgramNS.Factories;
using ZoneLightingTests.Resources.Programs;

namespace ZoneLightingTests
{
	public static class TestHelpers
	{
		public static void InitializeZoneScaffolder()
		{
			ZoneScaffolder.Instance.Initialize(ConfigurationManager.AppSettings["TestProgramModuleDirectory"]);
		}

		public static bool ValidateSteppersSync(IEnumerable<IStepper> steppers, int numberOfChecks, int msToWaitBeforeStart = 10, int msToWaitBetweenChecks = 1)
		{
			int[,] stepperSteps;
			var invalidStepIndex = SyncContextTests.ValidateStepperSyncPhase(steppers.ToArray(), out stepperSteps, numberOfChecks);
			return invalidStepIndex.Length == 0 && stepperSteps.Length != 0;
		}

		public static bool ValidateSteppersRunning(IEnumerable<IStepper> steppers, int numberOfChecks, int msToWaitBeforeStart = 10, int msToWaitBetweenChecks = 1)
		{
			int[,] stepperSteps;
			SyncContextTests.ValidateStepperSyncPhase(steppers.ToArray(), out stepperSteps, numberOfChecks);
			return stepperSteps.Length != 0;
		}
	}
}
