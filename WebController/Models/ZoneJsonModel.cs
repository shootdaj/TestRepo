namespace WebController.Models
{
	public class ZoneJsonModel
	{
		//public ZoneJsonModel(Zone zone)
		//{
		//	//this = Container.AutoMapper.Map<Zone, ZoneJsonModel>(zone); //how to make this work?
		//}

		public ZoneJsonModel()
		{ }
		
		public string Name { get; private set; }
		
		public string ZoneProgramName { get; private set; }
		
		public double Brightness { get; private set; }

		public bool Running { get; private set; }

		public int LightCount { get; private set; }

		//public Dictionary<string, ZoneProgramInputCollection> ZoneProgramInputs { get; set; } = new Dictionary<string, ZoneProgramInputCollection>();

		//public bool IsProgramLooping => ZoneProgram is LoopingZoneProgram;

		//public IList<ReactiveZoneProgram> InterruptingPrograms { get; private set; } = new List<ReactiveZoneProgram>();
	}
}