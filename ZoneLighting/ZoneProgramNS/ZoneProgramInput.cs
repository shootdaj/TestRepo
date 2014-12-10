using System;
using System.Drawing;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ZoneLighting.ZoneProgramNS
{
	[DataContract]
	public class ZoneProgramInput
	{
		[DataMember]
		public string Name { get; set; }

		public Subject<object> InputSubject { get; }

		private IDisposable InputDisposable { get; set; }

		[DataMember]
		public Type Type { get; private set; }

		[DataMember]
		public object Value { get; private set; }

		public ZoneProgramInput(string name, Type type) : this()
		{
			Name = name;
			Type = type;
			
		}

		public ZoneProgramInput()
		{
			InputSubject = new Subject<object>();
		}

		public void Subscribe(IObserver<object> toCall)
		{
			InputDisposable = InputSubject.Subscribe(toCall);
		}

		public void Subscribe(Action<object> toCall)
		{
			InputDisposable = InputSubject.Subscribe(toCall);
		}

		public void Unsubscribe()
		{
			InputDisposable?.Dispose();
		}

		/// <summary>
		/// Sends data through the input to the program it's attached to.
		/// </summary>
		/// <param name="data">The data will be deconstructed/cast to the underlying type in the program.</param>
		public void Set(object data)
		{
			InputSubject.OnNext(data);
			Value = data;
		}
	}
}
