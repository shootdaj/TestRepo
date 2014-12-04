using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
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
	[DataContract]
	public abstract class ZoneProgram
	{
		#region CORE

		/// <summary>
		/// Name of the zone program.
		/// </summary>
		[DataMember]
		public string Name { get; protected set; }

		/// <summary>
		/// Zone on which the program is being run.
		/// </summary>
		public Zone Zone { get; set; }
		
		/// <summary>
		/// Trigger that fires when the program has fully stopped - only applies for non-force stop calls.
		/// </summary>
		public Trigger StopTrigger { get; private set; }
		
		/// <summary>
		/// Lighting controller to be used by the program.
		/// </summary>
		public LightingController LightingController
		{
			get { return Zone.LightingController; }
		}

		/// <summary>
		/// Easy accessor for Lights in Zone.
		/// </summary>
		public IList<ILogicalRGBLight> Lights
		{
			get { return Zone.Lights;  }
		}

		private ZoneProgramInputCollection Inputs { get; set; }

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
			Inputs = new ZoneProgramInputCollection();
		}

		#endregion

		#region Base Methods

		public virtual void StartBase()
		{
			Start();
		}
		
		#endregion

		#region Overridables

		protected abstract void Start();
		public abstract void Stop(bool force);

		#endregion

		#region API

		protected ZoneProgramInput<object> AddInput(string name = "", Action<object> action = null)
		{
			var input = new ZoneProgramInput<object>(name);
			Inputs.Add(input);
			if (action != null)
			{
				input.Subscribe(action);
			}
			return input;
		}

		protected void RemoveInput(string name)
		{
			GetInput(name).Unsubscribe();
		}

		protected ZoneProgramInput<object> GetInput(string name)
		{
			return Inputs[name];
		}

		#endregion
	}
}
