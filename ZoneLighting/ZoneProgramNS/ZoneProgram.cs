using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ZoneLighting.Communication;
using ZoneLighting.TriggerDependencyNS;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ZoneProgramNS
{
	/// <summary>
	/// Represents a "program" that can be played on a zone. Something like a loop
	/// or a periodic notification, or anything else that can be represented by lighting 
	/// the zones in a certain way.
	/// </summary>
	public abstract class ZoneProgram// : IZoneProgram
	{
		#region CORE

		public string Name { get; protected set; }
		
		public ZoneProgramParameter ProgramParameter { get; set; }

		public Zone Zone { get; set; }
		
		public Trigger StopTrigger { get; private set; }
		
		/// <summary>
		/// Lighting controller to be used by the program.
		/// </summary>
		public LightingController LightingController
		{
			get { return Zone.LightingController; }
		}

		public IList<ILogicalRGBLight> Lights
		{
			get { return Zone.Lights;  }
		}

		#endregion CORE

		#region C+I

		protected ZoneProgram(string name)
		{
			Name = name;
			Construct();
		}

		protected ZoneProgram()
		{
			Type thisType = this.GetType();
			Name =
				(string)thisType.GetCustomAttributes(typeof(ExportMetadataAttribute), false)
					.Cast<ExportMetadataAttribute>().First(attr => attr.Name == "Name").Value;
			Construct();
		}

		private void Construct()
		{
			StopTrigger = new Trigger();
		}

		#endregion

		#region Base Methods

		public void StartBase(ZoneProgramParameter parameter)
		{
			if (AllowedParameterTypes.Contains(parameter.GetType()))
			{
				ProgramParameter = parameter;
				Start(parameter);
			}
			else
			{
				throw new Exception("Input parameter type is not an allowed parameter type for this zone program.");
			}
		}


		#endregion

		#region Overridables

		protected abstract void Start(ZoneProgramParameter parameter);
		public abstract void Stop();
		public abstract IEnumerable<Type> AllowedParameterTypes { get; }

		#endregion
	}
}
