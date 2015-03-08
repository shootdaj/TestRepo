using System;
using System.Diagnostics;
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
			var result = true;

			//create a sync context to conduct the test with
			var testContext = new SyncContext();

			//create two programs to be synced
			var stepperAlpha = new Stepper("a");
			var stepperBeta = new Stepper("b");

			//sync and start
			testContext.SyncAndStart(stepperAlpha, stepperBeta);

			int stepperAStep = 0, stepperBStep = 0;

			//every time we check the step, it should be within 1 for both programs
			//because they should have been synced and then started - one program may be ahead of
			//another because this test may read in between the transactions (since this test is not
			//aware of the sync itself, it's reading values intermittently, so being off by one 
			//does not imply that the programs are out of sync. if the programs do go out of sync, 
			//with enough repetitions they will go out of sync even more, which means their steps
			//will eventually differ by more than 1, which is what this for loop is testing
			for (int i = 0; i < 100; i++)
			{
				stepperAStep = stepperAlpha.CurrentStep;
				stepperBStep = stepperBeta.CurrentStep;

				//check to make sure the difference in steps in no more than 1 (and check for wrapping)
				if (Math.Abs(stepperAStep - stepperBStep) > 1 &&
					!(stepperAStep == stepperAlpha.EndStep && stepperBStep == stepperBeta.StartStep) &&
					!(stepperAStep == stepperAlpha.StartStep && stepperBStep == stepperBeta.EndStep))
				{
					result = false;
					break;
				}

				Thread.Sleep(10);
			}

			//cleanup
			stepperAlpha.Stop(true);
			stepperBeta.Stop(true);

			if (result) Assert.Pass();
			else
				Assert.Fail("The two programs are on different steps --> " + stepperAlpha.Name + "=" + stepperAStep + ":" +
				            stepperBeta.Name + "=" + stepperBStep);


			Assert.Pass();
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
					//Debug.Print(this.Name + " --");
					CurrentStep = StartStep;
				}
				else
				{
					//Debug.Print(this.Name + " ++");
					CurrentStep++;
				}

				SyncContext?.SignalAndWait();
				//Debug.Print(this.Name + " " + CurrentStep);
			}

			public static class StepperSyncLevel
			{
				public static SyncLevel Step = new SyncLevel("Step");
			}
		}
	}
}
