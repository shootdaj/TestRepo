using System;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ZoneProgramNS
{
	[DataContract]
	public class ZoneProgramInput : IDisposable
	{
		[DataMember]
		public string Name { get; set; }

		protected Subject<object> InputSubject { get; } = new Subject<object>();

		protected IDisposable InputDisposable { get; set; }
		
		[DataMember]
		public Type Type { get; private set; }

		[DataMember]
		public object Value { get; protected set; }

		public ZoneProgramInput(string name, Type type) : this()
		{
			Name = name;
			Type = type;
			
		}

		public ZoneProgramInput()
		{
		}

		public virtual void Subscribe(Action<dynamic> toCall)
		{
			InputDisposable = InputSubject.Subscribe(toCall);
		}

		public virtual void Unsubscribe()
		{
			InputDisposable?.Dispose();
		}

		public virtual void Dispose()
		{
			Unsubscribe();
			InputSubject.Dispose();
			Name = null;
			Type = null;
			Value = null;
		}

		/// <summary>
		/// Sends data through the input to the program it's attached to.
		/// </summary>
		/// <param name="data">The data will be deconstructed/cast to the underlying type in the program.</param>
		public virtual void SetValue(object data)
		{
			InputSubject.OnNext(data);
			Value = data;
		}

		public bool IsInputSubjectSameAs(Subject<object> inputSubject)
		{
			return inputSubject == InputSubject;
		}

	}
}
