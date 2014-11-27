using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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


		Zone Zone { get; set; }

		/// <summary>
		/// This allows the ZLM to check which parameter types are allowed for this specific type of program.
		/// </summary>
		IEnumerable<Type> AllowedParameterTypes { get; } 
	}
}
