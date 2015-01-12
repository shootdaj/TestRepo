using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ZoneLighting.ZoneProgramNS
{
	public abstract class ReactiveZoneProgram : ZoneProgram
	{
		protected abstract override void StartCore(Barrier barrier);

		protected override void StopCore(bool force)
		{
			
		}

		public override void Resume(Barrier barrier)
		{
			//TODO: Implement resume logic
		}

		protected override void Pause()
		{
			//TODO: Implement pause logic
		}

		/// <summary>
		/// Adds a live input to the zone program. A live input is an input that can be controlled while
		/// the program is running and the program will respond to it in the way it's designed to.
		/// </summary>
		/// <param name="name">Name of the input.</param>
		/// <param name="action">The action that should occur when the input is set to a certain value. This will be defined by the 
		/// subclasses of this class to perform certain actions when the this input is set to a value.</param>
		/// <returns>The input that was just added.</returns>
		protected ZoneProgramInput AddInterruptingInput<T>(string name, Action<object> action, Barrier barrier = null)
		{
			var input = new InterruptingInput(name, typeof(T));
			Inputs.Add(input);
			
			input.Subscribe(data =>				//when the input's OnNext is called, do whatever it was programmed to do and then fire the StopSubject
			{
				input.StartTrigger.Fire(this, null);
				action(data);
				DetachBarrier();
				input.StopSubject.OnNext(null);
				input.StopTrigger.Fire(this, null);
			});
			return input;
		}

		public void SetInterruptQueue(ActionBlock<InterruptInfo> interruptQueue)
		{
			Inputs.Where(input => input is InterruptingInput)
				.ToList()
				.ForEach(input => ((InterruptingInput)input).SetInterruptQueue(interruptQueue));
		}

		public void RemoveInterruptQueue()
		{
			Inputs.Where(input => input is InterruptingInput)
				.ToList()
				.ForEach(input => ((InterruptingInput)input).RemoveInterruptQueue());
		}

		//protected Task RunProgram { get; set; }
		//protected Thread RunProgramThread { get; set; }
	}
}
