using System;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ZoneLighting.ZoneProgramNS
{
	[DataContract]
	[JsonConverter(typeof(UnderlyingTypeConverter))]
	public class ZoneProgramInput<T> : IZoneProgramInput
	{
		[DataMember]
		public string Name { get; set; }

		public Subject<T> InputSubject { get; }

		private IDisposable InputDisposable { get; set; }

		[DataMember]
		public Type Type { get; private set; }

		[DataMember]
		public object Value { get; private set; }

		public ZoneProgramInput(string name, Type type)
		{
			Name = name;
			Type = type;
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
			InputDisposable?.Dispose();
		}

		/// <summary>
		/// Sends data through the input to the program it's attached to.
		/// </summary>
		/// <param name="data">The data will be deconstructed/cast to the underlying type in the program.</param>
		public void Set(T data)
		{
			InputSubject.OnNext(data);
			Value = data;
		}
	}

	public interface IZoneProgramInput
	{
		string Name { get; }
		Type Type { get; }
		object Value { get; }
	}
}
