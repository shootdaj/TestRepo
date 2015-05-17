using System;
using NUnit.Framework;
using ZoneLighting;
using ZoneLightingTests;

namespace Console
{
	public class Program
	{
		public static void Main(string[] args)
		{
			RunTest(ZoneProgramTests.CooperativeStop_Works);

			//RunTest(() => SyncContextTests.Sync_OneStepperSyncingWithThree_Works(1000));
			//RunTest(() => SyncContextTests.Sync_ThreeStepperSyncingWithOne_Consecutive_Works(1000));
			//RunTest(() => SyncContextTests.Sync_TwoStepperSyncingWithTwo_Works(1000));
			//RunTest(() => SyncContextTests.Sync_TwoSteppersInternalLoop_Consecutive_Works(1000));
			//RunTest(() => SyncContextTests.Sync_FourSteppers_Simultaneous_Works(1000));
			//RunTest(() => SyncContextTests.Sync_TwoSteppers_Simultaneous_Works(1000));
			//RunTest(() => SyncContextTests.ZoneProgram_StartWithSync_OneStepperSyncingWithThree_Works(1000));
			//RunTest(() => SyncContextTests.ZoneProgram_StartWithSync_StepperConstructorSyncContext_OneStepperSyncingWithThree_Works(1000));
			//RunTest(() => SyncContextTests.Sync_OneStepper_Works(1000));
			//RunTest(() => SyncContextTests.Sync_SimultaneousSync_ThreeStepperSyncingWithOne_Simultaneous_Works(1000));

			//ZoneLightingManager.Instance.Initialize(false, false);

			//var task = new Task(() =>
			//{
			//	while (true)
			//	{
			//		var input = System.Console.ReadLine();
			//		var color = Color.FromName(input);
			//		if (color.IsKnownColor)
			//		{
			//			dynamic parameters = new ExpandoObject();
			//			parameters.Color = color;
			//			parameters.Time = 50;
			//			parameters.Soft = true;

			//			ZoneLightingManager.Instance.Zones.ToList().ForEach(z => z.InterruptingPrograms[0].SetInput("Blink", parameters));
			//			ZoneLightingManager.Instance.Zones.ToList().ForEach(z => z.InterruptingPrograms[0].SetInput("Blink", parameters));
			//		}
			//	}
			//});

			//task.Start();

			//Thread.Sleep(Timeout.Infinite);

			////DebugTools.Print();
		}

		public static void RunTest(Action method)
		{
			try
			{
				method();
			}
			catch (SuccessException ex)
			{
				ZLEventSource.Log.AddEvent("RunTest: " + method.Method.Name, "Test Passed");
			}
			catch (AssertionException ex)
			{
				ZLEventSource.Log.AddEvent("RunTest: " + method.Method.Name, "Test Failed: " + ex.Message);
			}
		}
	}
}
