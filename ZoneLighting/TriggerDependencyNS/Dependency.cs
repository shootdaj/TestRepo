using System;

namespace ZoneLighting.TriggerDependencyNS
{
	/// <summary>
	/// Represents an event-based dependency between two classes/object.
	/// </summary>
	[Serializable]
	public class Dependency<T> : IDisposable, IRefresher, IFireable, ISubscribable<T>
	{
		#region CORE

		#region Trigger + Event Handles

		/// <summary>
		/// Backing field for the Trigger.
		/// </summary>
		private Trigger<T> _trigger;

		/// <summary>
		/// The Trigger for the Dependency.
		/// </summary>
		public Trigger<T> Trigger
		{
			get { return _trigger; }
			set
			{
				_trigger = value;
				_trigger.Subscribe(Trigger_FireTrigger);
			}
		}

		/// <summary>
		/// Executes when the Trigger is fired.
		/// </summary>
		protected void Trigger_FireTrigger(object sender, T e)
		{
			if (Active)
			{
				Action(sender, e);
			}
		}

		/// <summary>
		/// Makes the caller wait until the dependency has been refreshed and ready to catch another event - if that is supports the refresh model.
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public bool WaitForRefresh(int? timeout = null)
		{
			IRefreshable refreshableTrigger = _trigger as IRefreshable;
			if (refreshableTrigger != null)
			{
				return refreshableTrigger.WaitForRefresh(timeout);
			}
			throw new Exception("WaitForRefresh not supported because this Dependency's Trigger does not inherit from IRefreshable.");
		}

		#endregion

		public string Name;

		/// <summary>
		/// Indicates whether or not the Dependency is active.
		/// </summary>
		public bool Active { get; set; }

		/// <summary>
		/// The action to perform when the Dependency Trigger is fired.
		/// </summary>
		public EventHandler<T> Action { get; set; }

		/// <summary>
		/// Adds to the delegate to be called when the Dependency's Trigger is fired.
		/// </summary>
		/// <param name="actionToPerform"></param>
		public void AddAction(EventHandler<T> actionToPerform)
		{
			Subscribe(actionToPerform);
		}

		/// <summary>
		/// Adds to the delegate to be called when the Dependency's Trigger is fired.
		/// </summary>
		/// <param name="actionToPerform"></param>
		public void Subscribe(EventHandler<T> actionToPerform)
		{
			Action += actionToPerform;
		}

		/// <summary>
		/// Removes from the delegate to be called when the Dependency's Trigger is fired.
		/// </summary>
		/// <param name="actionToPerform"></param>
		public void RemoveAction(EventHandler<T> actionToPerform)
		{
			Unsubscribe(actionToPerform);
		}

		/// <summary>
		/// Removes from the delegate to be called when the Dependency's Trigger is fired.
		/// </summary>
		/// <param name="actionToPerform"></param>
		public void Unsubscribe(EventHandler<T> actionToPerform)
		{
			Action -= actionToPerform;
		}

		/// <summary>
		/// Resets the Trigger
		/// </summary>
		public void Reset()
		{
			_trigger.Reset();
		}

		/// <summary>
		/// Blocks the calling thread until this Dependency's Trigger is fired.
		/// </summary>
		/// <param name="timeout">Milliseconds to wait until the wait times out and returns false</param>
		/// <returns>True if Trigger was fired within the alloted time, false otherwise.</returns>
		public bool WaitForFire(int? timeout = null)
		{
			return _trigger.WaitForFire(timeout);
		}

		#endregion

		#region C+I+D

		/// <summary>
		/// Constructor for a Dependency
		/// </summary>
		/// <param name="name">Name of Dependency</param>
		/// <param name="action">Action to perform when the dependency trigger fires</param>
		/// <param name="trigger">Trigger to hook the dependency on to</param>
		/// <param name="active">True if Dependency should be automatically activated. If false, it will have to be manually activate by calling Activate().</param>
		public Dependency(string name, EventHandler<T> action, Trigger<T> trigger, bool active = true)
			: this(name, trigger, active)
		{
			if (action != null)
			{
				Action += action;
			}
		}

		public Dependency(string name, Trigger<T> trigger, bool active = true)
		{
			Name = name;
			if (trigger == null)
				throw new Exception(Properties.ResErrorMsgs.DependencyTriggerCannotBeNull);
			Trigger = trigger;
			Active = active;
		}

		/// <summary>
		/// Disposes all the events/wait handles/triggers that are associated with this dependency.
		/// </summary>
		public void Dispose()
		{
			Dispose(false);
		}

		/// <summary>
		/// Call to properly dispose off this Dependency instance
		/// </summary>
		/// <param name="recursive">If true, the trigger will be disposed also. If false, trigger will not be disposed. Default = true.</param>
		public void Dispose(bool recursive)
		{
			//if requested, dispose the trigger to which this depedency is attached
			if (recursive && _trigger != null)
				_trigger.Dispose(true);

			if (Action != null)
			{
				//remove all subscriptions to the underlying event
				foreach (var action in Action.GetInvocationList())
				{
					RemoveAction((EventHandler<T>)action);
				}

				Active = false;
				Name = null;
				if (_trigger != null)
				{
					_trigger.Unsubscribe(Trigger_FireTrigger);
					_trigger = null;
				}
			}
		}

		#endregion

		#region Activation/Deactivation

		/// <summary>
		/// Starts the Dependency
		/// </summary>
		public void Start()
		{
			Active = true;
		}

		/// <summary>
		/// Stops the Dependency
		/// </summary>
		public void Stop()
		{
			Active = false;
		}

		/// <summary>
		/// Alias for Start()
		/// </summary>
		public void Activate()
		{
			Start();
		}

		/// <summary>
		/// Alias for Stop()
		/// </summary>
		public void Deactivate()
		{
			Stop();
		}

		#endregion
	}

	public class Dependency : Dependency<EventArgs>
	{
		public Dependency(string name, EventHandler<EventArgs> action, Trigger trigger, bool active = true)
			: base(name, action, trigger, active)
		{
		}

		public Dependency(string name, Trigger trigger, bool active = true)
			: base(name, trigger, active)
		{
		}
	}
}