using System.Reactive.Subjects;

namespace ZoneLighting.ZoneProgramNS
{
	public class InterruptInfo
	{
		//public Action<object> Action { get; set; };
		public object Data { get; set; }
		public Subject<object> InputSubject { get; set; }
		public Subject<object> StopSubject { get; set; }
		public ZoneProgram ZoneProgram { get; set; }
	}
}
