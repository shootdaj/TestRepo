using System;
using System.Threading;

namespace ZoneLighting.TriggerDependencyNS
{
	/// <summary>
	///	Allows an object to subscribe to an event prescribed by the implementor of this interface. For example, Trigger has a fire event 
	/// to which one can subscribe with the Subscribe method. This subscription is in the form of an action to be performed
	/// by the code that is subscribing to the Trigger's fire event. The Unsubscribe call should remove the same subscription that was added by Subscribe.
	/// </summary>
	public interface ISubscribable<T>
	{
		/// <summary>
		/// Adds to the delegate to be called when the Dependency's Trigger is fired.
		/// </summary>
		/// <param name="actionToPerform"></param>
		void Subscribe(EventHandler<T> actionToPerform);

		/// <summary>
		/// Removes from the delegate to be called when the Dependency's Trigger is fired.
		/// </summary>
		/// <param name="actionToPerform"></param>
		void Unsubscribe(EventHandler<T> actionToPerform);
	}

	public interface ISubscribable : ISubscribable<EventArgs>
	{
	}

	/// <summary>
	/// Allows the inheritor to encapsulate a wait handle (AutoResetEvent, ManualResetEvent etc.) by calling WaitForFire 
	/// directly on the inheritor instead of having to expose the wait handle as a public member. For example Trigger.WaitForFire
	/// will defer to the Trigger's internal wait handle. Reset should reset the wait handle.
	/// </summary>
	public interface IFireable
	{
		bool WaitForFire(int? timeout = null);
		void Reset();
	}

	public interface IRefreshable
	{
		bool WaitForRefresh(int? timeout = null);
		void ResetRefresh();
		EventWaitHandle RefreshWaitHandle { get; set; }
	}

	/// <summary>
	/// Allows for extending the WaitForRefresh event handle of an IRefreshable
	/// </summary>
	public interface IRefresher
	{
		bool WaitForRefresh(int? timeout = null);
	}
}
