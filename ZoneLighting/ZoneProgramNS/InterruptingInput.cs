using System;
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

		//protected Subject<object> StopSubject { get; } = new Subject<object>();

		//private IDisposable StopNotifier { get; set; }

		public Subject<object> StopSubject { get; set; } = new Subject<object>();

		private ActionBlock<InterruptInfo> InterruptQueue { get; set; }

		//public override void Subscribe(Action<object> toCall)
		//{
		//	throw new Exception("Please use the Subscribe(Action<object>, ZoneProgram) method instead.");
		//}

		///// <summary>
		///// Subscribes this interrupting input to the interrupt the given program and call the given method to call.
		///// </summary>
		///// <param name="toCall">Method to be called when the input is set.</param>
		///// <param name="interruptedProgram">Program to be interrupted when the input is set.</param>
		//public void Subscribe(Action<object> toCall, ZoneProgram interruptedProgram)
		//{
		//	if (toCall == null || interruptedProgram == null)
		//		throw new Exception("toCall and interruptedProgram must both be non-null.");

		//	StartNotifier = StartSubject.Subscribe(data =>
		//	{
		//		interruptedProgram.Pause();
		//	});

		//	InputDisposable = InputSubject.Subscribe(toCall);

		//	StopNotifier = StopSubject.Subscribe(data =>
		//	{
		//		interruptedProgram.Resume();
		//	});

		//}


		/// <summary>
		/// Sets the program to be interrupted when the input is set.
		/// </summary>
		/// <param name="interruptedProgram">Program to be interrupted when the input is set.</param>
		public void SetInterruptQueue(ActionBlock<InterruptInfo> interruptQueue)
		{
			InterruptQueue = interruptQueue;


			//StartNotifier = StartSubject.Subscribe(data =>
			//{
			//	interruptedProgram.Pause();
			//});

			//StopNotifier = StopSubject.Subscribe(data =>
			//{
			//	interruptedProgram.Resume();
			//});
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

			InterruptQueue.Post(new InterruptInfo()
			{
				Data = data,
				InputSubject = InputSubject,
				StopSubject = StopSubject
			});
			
			//InputSubject.OnNext(data); //TODO: Remove. This will be done by the interruptqueue

			Value = data;
		}
	}
}
