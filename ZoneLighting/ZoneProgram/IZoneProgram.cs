using System;
using System.Collections.Generic;

namespace ZoneLighting.ZoneProgram
{
	public interface IZoneProgram
	{
		/// <summary>
		/// This is to allow preprocessing before the Start method is called.
		/// </summary>
		/// <param name="parameter"></param>
		void StartBase(IZoneProgramParameter parameter);
		//void Start(IZoneProgramParameter parameter);
		void Stop();
		//void Pause();


		Zone Zone { get; set; }

		/// <summary>
		/// This allows the ZLM to check which parameter types are allowed for this specific type of program.
		/// </summary>
		IEnumerable<Type> AllowedParameterTypes { get; } 
	}
}
