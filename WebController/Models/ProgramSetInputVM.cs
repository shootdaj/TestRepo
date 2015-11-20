using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZoneLighting.ZoneProgramNS;

namespace WebController.Models
{
	public class ProgramSetInputVM
	{
		public List<ZoneProgram> AvailablePrograms { get; set; }
		
		public ZoneProgram SelectedProgram { get; set; }
		
		public List<ZoneProgramInput> Inputs { get; set; }  
	}
}