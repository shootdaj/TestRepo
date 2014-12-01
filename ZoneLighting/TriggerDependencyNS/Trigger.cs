using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ZoneLighting.TriggerDependencyNS
{
	/// <summary>
	/// Represents a trigger whose job is to notify a subscriber that an event has occurred 
	/// alongwith optionally passing the subscriber some event arguments. The subclass may override the triggering mechanism.
	/// </summary>
	[Serializable]
	public class Trigger<T> : IDisposable, IFireable, ISubscribable<T>
	{
		#region CORE

		public string Name;

		protected Dependency<T> _dependency;

		/// <summary>
		/// Indicates whether or not the Trigger is active.
		/// </summary>
		protected bool Active { get; set; }

		/// <summary>
		/// EventHandler for the firing of the Trigger
		/// </summary>
		private event EventHandler<T> _fireTrigger;

		/// <summary>
		/// Fires alongwith the Trigger. Allows for simpler notifications.
		/// </summary>
		private EventWaitHandle _waitHandle;

		#endregion

		#region C + I + D

		/// <summary>
		/// Declares a Trigger without any publications or subscriptions
		/// </summary>
		public Trigger(string name = "", bool active = true)
		{
			Name = name;
			Active = active;
			_waitHandle = new AutoResetEvent(false);
		}

		public Trigger(Dependency<T> dependency, string name = "", bool active = true)
			: this(name, active)
		{
			_dependency = dependency;
			//Action<object, EventArgs> action = _repositoryDependency.Action;
			//AddPublication(_dependency.Action);
			//if the above code doesn't work, try this:
			_dependency.Action += Fire;
		}

		/// <summary>
		/// Disposes all the events/wait handles/dependencies that are associated with this trigger.
		/// </summary>
		public virtual void Dispose()
		{
			Dispose(false);
		}

		/// <summary>
		/// Call to properly dispose off this Trigger instance
		/// </summary>
		/// <param name="recursive">If true, the dependency will be disposed also. If false, dependency will not be disposed. Default = true.</param>
		public void Dispose(bool recursive)
		{
			//if requested, dispose the dependency this trigger is attached to
			if (recursive && _dependency != null)
				_dependency.Dispose(true);

			if (_fireTrigger != null)
			{
				//remove all subscriptions to the underlying event
				foreach (var triggerEvent in _fireTrigger.GetInvocationList())
				{
					Unsubscribe((EventHandler<T>)triggerEvent);
				}

				Active = false;
				Name = null;
				if (_dependency != null)
				{
					_dependency.Action -= Fire;
					_dependency = null;
				}
			}

			//dispose the event wait handle
			_waitHandle.Close();
			_waitHandle = null;
		}

		#endregion

		#region Event Stuff

		/// <summary>
		/// This can be called by the subclass manually if the stock implementation does not meed their needs
		/// This is public by design - events cannot be passed as "first-class" C# objects,
		/// </summary>
		public void Fire(object sender, T e)
		{
			//call the notification function
			OnFireTrigger(sender, e);

			//call the extension
			FireExtend(sender, e);
		}

		/// <summary>
		/// For the subclass to extend the fire method of the trigger
		/// </summary>
		protected virtual void FireExtend(object sender, T e) { }

		#region Activation/Deactivation

		/// <summary>
		/// Returns true is Trigger is active, false otherwise
		/// </summary>
		public bool IsActive
		{
			get { return Active; }
		}

		/// <summary>
		/// Activates the Trigger.
		/// </summary>
		public void Activate()
		{
			if (!Active)
			{
				Active = true;
				ActivateExtend();
			}
		}

		/// <summary>
		/// To be implemented by the subclass in case additional things need to be done for Trigger activation
		/// </summary>
		protected virtual void ActivateExtend() { }

		/// <summary>
		/// Deactivates the Trigger.
		/// </summary>
		public void Deactivate()
		{
			if (Active)
			{
				Active = false;
				DeactivateExtend();
			}
		}

		/// <summary>
		/// To be implemented by the subclass in case additional things need to be done for Trigger deactivation
		/// </summary>
		protected virtual void DeactivateExtend() { }

		#endregion

		/// <summary>
		/// To be called by the subclass to execute FireTrigger and thus execute any methods attached to it.
		/// </summary>
		protected void OnFireTrigger(object sender, T e)
		{
			if (Active)
			{
				//DebugTools.AddEvent("*" + this.Name + ".OnFireTrigger", "START WaitHandle set.");
				_waitHandle.Set();

				if (_fireTrigger != null)
				{
					//Debug.Print(Name + ".FireTrigger delegate called.");
					//DebugTools.AddEvent(this.Name + ".OnFireTrigger", "START Trigger fired.");
					_fireTrigger(sender, e);
				}
			}
		}

		/// <summary>
		/// Resets the wait handle
		/// </summary>
		public void Reset()
		{
			_waitHandle.Reset();
		}

		/// <summary>
		/// Blocks the calling thread until the Trigger is fired.
		/// </summary>
		/// <param name="timeout">Milliseconds to wait until the wait times out and returns false</param>
		/// <returns>True if Trigger was fired within the alloted time, false otherwise.</returns>
		public bool WaitForFire(int? timeout = null)
		{
			return (timeout == null ? _waitHandle.WaitOne() : _waitHandle.WaitOne((int)timeout)) && WaitOneExtend(timeout);
		}

		/// <summary>
		/// Allows the subtype to extend the WaitOne method if it needs to wait on other things.
		/// </summary>
		/// <param name="ms"></param>
		/// <returns></returns>
		public virtual bool WaitOneExtend(int? ms = null)
		{
			return true;
		}

		#endregion

		#region Publication + Subscription
		
		/// <summary>
		/// Allows the subscriber of the Trigger to add a subscription by providing a method to add to the Trigger Event Handler
		/// </summary>
		/// <param name="fireTrigger">Method to add to the Trigger Event Handler</param>
		public void Subscribe(EventHandler<T> fireTrigger)
		{
			_fireTrigger += fireTrigger;
		}

		/// <summary>
		/// Allows the subscriber of the Trigger to remove a subscription by providing a method to remove from the Trigger Event Handler
		/// </summary>
		/// <param name="fireTrigger">Method to remove from the Trigger Event Handler</param>
		public void Unsubscribe(EventHandler<T> fireTrigger)
		{
			_fireTrigger -= fireTrigger;
		}

		/// <summary>
		/// Unsubscribes from all subscriptions
		/// </summary>
		public void UnsubscribeAll()
		{
			foreach (var eventHandlerDelegate in _fireTrigger.GetInvocationList())
			{
				var eventHandler = (EventHandler<T>)eventHandlerDelegate;
				_fireTrigger -= eventHandler;
			}
		}

		/// <summary>
		/// The list of subscriptions this Trigger has
		/// </summary>
		public List<Delegate> Subscriptions
		{
			get { return _fireTrigger != null ? _fireTrigger.GetInvocationList().ToList() : null; }
		}

		#endregion

	}

	[Serializable]
	public class Trigger : Trigger<EventArgs>, ISubscribable
	{
		public Trigger(string name = "", bool active = true)
			: base(name, active)
		{
		}

		public Trigger(Dependency dependency, string name = "", bool active = true)
			: base(dependency, name, active)
		{
		}
	}

	/// <summary>
	/// Event arguments for the Trigger's Fire event. 
	/// </summary>
	public class TriggerFireEventArgs : EventArgs
	{
		public object Data;

		public TriggerFireEventArgs()
		{ }

		public TriggerFireEventArgs(object data)
		{
			Data = data;
		}
	}
}
