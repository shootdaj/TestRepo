using System;
using System.Collections.Generic;
using ZoneLighting.TriggerDependencyNS;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ZoneProgram
{
	public interface IZoneProgram
	{
		/// <summary>
		/// This is to allow preprocessing before the Start method is called.
		/// </summary>
		/// <param name="parameter"></param>
		void StartBase(IZoneProgramParameter parameter);
		
		void Stop();

		//void Start(IZoneProgramParameter parameter);
		//void Pause();

		IZoneProgramParameter ProgramParameter { get; }

		Zone Zone { get; set; }

		Trigger StopTrigger { get; }

		/// <summary>
		/// This allows the ZLM to check which parameter types are allowed for this specific type of program.
		/// </summary>
		IEnumerable<Type> AllowedParameterTypes { get; } 
	}
}
