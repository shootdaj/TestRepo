using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoneLighting.TriggerDependencyNS;

namespace ZoneLighting.ZoneProgramNS
{
	public class InterruptInfo
	{
		//public Action<object> Action { get; set; };
		public object Data { get; set; }
		public Subject<object> InputSubject { get; set; }
		public Subject<object> StopSubject { get; set; }
	}
}
