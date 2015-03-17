using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourceProxy;

namespace ZoneLighting
{
	[EventSource(Name = "ZLEventSource")]
	public abstract class ZLEventSource : EventSource
	{
		[Event(1, Message = "{0}: {1}", Level = EventLevel.Informational)]
		public abstract void AddEvent(string methodName, string message);
		
		public static ZLEventSource Log = EventSourceImplementer.GetEventSourceAs<ZLEventSource>();
	}
}
