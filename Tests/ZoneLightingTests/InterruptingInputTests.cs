using System.Configuration;
using System.Drawing;
using System.Threading.Tasks.Dataflow;
using FakeItEasy;
using Xunit;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Factories;
using ZoneLightingTests.Resources.Programs;

namespace ZoneLightingTests
{
	public class InterruptingInputTests
	{
		[Fact]
		public void InterruptingInput_InterruptsBackgroundProgram()
		{
			//arrange
			var zoneScaffolder = new ZoneScaffolder();
			zoneScaffolder.Initialize(ConfigurationManager.AppSettings["TestProgramModuleDirectory"]);
			
			var leftWing =   //new FadeCandyZone("LeftWing");
				A.Fake<FadeCandyZone>();
			leftWing.AddFadeCandyLights(6, 1);

			var scrollDotDictionary = new InputStartingValues();
			scrollDotDictionary.Add("DelayTime", 30);
			scrollDotDictionary.Add("DotColor", (Color?)Color.Red);

			FadeCandyController.Instance.Initialize();	//needs to be faked somehow

			zoneScaffolder.InitializeZone(leftWing, "ScrollDot", scrollDotDictionary);

			leftWing.AddInterruptingProgram(new StaticColor());

			//act
			leftWing.InterruptingPrograms[0].Inputs[0].SetValue(Color.Blue);

			//assert - figure out how to inject these into non-fake zone or figure out how to convert leftWing into
			A.CallTo(() => leftWing.ZoneProgram.Pause()).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => leftWing.ZoneProgram.Resume()).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => leftWing.InterruptingPrograms[0].Start(null, A.Dummy<ActionBlock<InterruptInfo>>())).MustHaveHappened(Repeated.Exactly.Once);
		}
	}
}
