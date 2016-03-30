using System.Collections.Generic;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace WebController.Models
{
	public class ProgramSetJsonModel
	{
		//public ProgramSetJsonModel(ProgramSet programSet)
		//{
		//	//this = Container.AutoMapper.Map<ProgramSet, ProgramSetJsonModel>(programSet); //how to make this work?
		//}

		public ProgramSetJsonModel()
		{ }
		
		public string Name { get; private set; }
		
		public List<ZoneJsonModel> Zones { get; private set; }

		public string ProgramName { get; private set; }
		
		public bool Sync { get; private set; }

		public ProgramState State { get; private set; }
	}
}