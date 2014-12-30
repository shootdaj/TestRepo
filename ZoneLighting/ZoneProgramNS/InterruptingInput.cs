using System;
using System.Dynamic;
using System.Reactive.Subjects;
using System.Threading.Tasks.Dataflow;
using ZoneLighting.TriggerDependencyNS;

namespace ZoneLighting.ZoneProgramNS
{
	public class InterruptingInput : ZoneProgramInput
	{
		public InterruptingInput(string name, Type type) : base(name, type)
		{

		}

		public Subject<object> StopSubject { get; set; } = new Subject<object>();

		public Trigger StopTrigger { get; } = new Trigger("InterruptingInputStopTrigger");

		public Trigger StartTrigger { get; } = new Trigger("InterruptingInputStartTrigger");

		private ActionBlock<InterruptInfo> InterruptQueue { get; set; }

		/// <summary>
		/// Sets the interrupt queue to be post interrupts to when the input is set.
		/// </summary>
		public void SetInterruptQueue(ActionBlock<InterruptInfo> interruptQueue)
		{
			InterruptQueue = interruptQueue;
		}

		public void RemoveInterruptQueue()
		{
			InterruptQueue = null;
		}

		public override void Unsubscribe()
		{
			InterruptQueue = null;
			InputDisposable?.Dispose();
		}

		/// <summary>
		/// Sends data through the input to the program it's attached to.
		/// </summary>
		/// <param name="data">The data will be deconstructed/cast to the underlying type in the program.</param>
		public override void SetValue(object data)
		{
			if (InterruptQueue == null)
				throw new Exception("Interrupt Queue has not been set or has been set to null.");

			//Console.WriteLine("START Sending IRQ to Zone");

			InterruptQueue.Post(new InterruptInfo()
			{
				Data = data,
				InputSubject = InputSubject,
				StopSubject = StopSubject
			});

			//Console.WriteLine("FINISHED Sending IRQ to Zone");

			Value = data;
		}
	}
}
