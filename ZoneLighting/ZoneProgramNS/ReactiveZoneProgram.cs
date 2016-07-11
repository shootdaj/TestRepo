using System;
using System.Diagnostics.CodeAnalysis;

namespace ZoneLighting.ZoneProgramNS
{
	public abstract class ReactiveZoneProgram : ZoneProgram
	{
		protected override void StartCore(dynamic parameters = null)
		{
			
		}

		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		protected ReactiveZoneProgram()
		{
			// ReSharper disable once VirtualMemberCallInConstructor
			Setup();
		}

		protected virtual void Setup()
		{
			
		}

		/// <summary>
		/// Subclass can have Unsetup, but doesn't need to.
		/// </summary>
		protected virtual void Unsetup()
		{

		}

		/// <summary>
		/// This method is only here so that subclasses don't need to unnecessarily override this method
		/// if they don't need to.
		/// </summary>
		protected override void StopCore(bool force)
		{
			
		}

		public override void Dispose(bool force)
		{
			base.Dispose(force);
			Unsetup();
		}

		//public override void Resume()
		//{
		//	//TODO: Implement resume logic
		//}

		//protected override void Pause()
		//{
		//	//TODO: Implement pause logic
		//}

		/// <summary>
		/// Adds a live input to the zone program. A live input is an input that can be controlled while
		/// the program is running and the program will respond to it in the way it's designed to.
		/// </summary>
		/// <param name="name">Name of the input.</param>
		/// <param name="action">The action that should occur when the input is set to a certain value. This will be defined by the 
		/// subclasses of this class to perform certain actions when the this input is set to a value.</param>
		/// <returns>The input that was just added.</returns>
		protected ZoneProgramInput AddInterruptingInput<T>(string name, Action<dynamic> action)
		{
			var input = new InterruptingInput(name, typeof(T), this);
			Inputs.Add(input);

			////if sync is requested, go into synchronizable state
			//if (syncContext != null)
			//{
			//	IsSynchronizable.Fire(this, null);
			//	WaitForSync.WaitForFire();
			//	IsSyncStateRequested = false;
			//}

			//input.AttachBarrier(syncContext?.Barrier);

			input.Subscribe(data =>				//when the input's OnNext is called, do whatever it was programmed to do and then fire the StopSubject
			{
				input.StartTrigger.Fire(this, null);
				action(data);
				SyncContext?.SignalAndWait();
				input.StopSubject.OnNext(null);
				input.StopTrigger.Fire(this, null);
			});
			return input;
		}
	}
}
