using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ZoneLighting.ZoneProgramNS
{
	public class ZoneProgramInput<T>
	{
		public string Name { get; set; }
		public Subject<T> InputSubject { get; private set; }
		private IDisposable InputDisposable { get; set; }

		public ZoneProgramInput(string name = "")
		{
			Name = name;
			InputSubject = new Subject<T>();
		}

		public void Subscribe(IObserver<T> toCall)
		{
			InputDisposable = InputSubject.Subscribe(toCall);
		}

		public void Subscribe(Action<T> toCall)
		{
			InputDisposable = InputSubject.Subscribe(toCall);
		}

		public void Unsubscribe()
		{
			if (InputDisposable != null)
				InputDisposable.Dispose();
		}

		/// <summary>
		/// Sends data through the input to the program it's attached to.
		/// </summary>
		/// <param name="data">The data will be deconstructed/cast to the underlying type in the program.</param>
		public void Set(T data)
		{
			InputSubject.OnNext(data);
		}
	}
}
