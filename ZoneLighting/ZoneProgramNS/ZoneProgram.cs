using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
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

		/// <summary>
		/// Inputs for this program.
		/// </summary>
		[DataMember]
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

		public virtual void StartBase(InputStartingValues inputStartingValues = null)
		{
			Start();
			if (inputStartingValues != null)
				SetInputs(inputStartingValues);
		}
		
		#endregion

		#region Overridables

		protected abstract void Start();
		public abstract void Stop(bool force);

		#endregion

		#region API

		/// <summary>
		/// Returns the names of all inputs.
		/// </summary>
		/// <returns></returns>
		public List<string> GetInputNames()
		{
			return Inputs.Select(input => input.Name).ToList();
		}

		public InputStartingValues GetInputValues()
		{
			var inputStartingValues = new InputStartingValues();
			Inputs.ToList().ForEach(input => inputStartingValues.Add(input.Name, input.Value));
			return inputStartingValues;
		}

		/// <summary>
		/// Returns the name of all inputs of all types T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public List<string> GetInputNames<T>()
		{
			return Inputs.Where(i => i.Type == typeof(T)).Select(input => input.Name).ToList();
		}
		

		/// <summary>
		/// Adds a live input to the zone program. A live input is an input that can be controlled while
		/// the program is running and the program will respond to it in the way it's designed to.
		/// </summary>
		/// <param name="name">Name of the input.</param>
		/// <param name="type">Type of the input. This type must match the type of the incoming parameter of the action.</param>
		/// <param name="action">The action that should occur when the input is set to a certain value. This will be defined by the 
		/// subclasses of this class to perform certain actions when the this input is set to a value.</param>
		/// <returns>The input that was just added.</returns>
		protected ZoneProgramInput<object> AddInput<T>(string name, Action<object> action)
		{
			var input = new ZoneProgramInput<object>(name, typeof(T));
			Inputs.Add(input);
			input.Subscribe(action);
			return input;
		}

		protected ZoneProgramInput<object> AddMappedInput(object instance, string propertyName)
		{
			var propertyInfo = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
			var input = new ZoneProgramInput<object>(propertyInfo.Name, propertyInfo.PropertyType);
			Inputs.Add(input);
			input.Subscribe(incomingValue => propertyInfo.SetValue(instance, incomingValue));
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

		public void SetInput(string name, object data)
		{
			GetInput(name).Set(data);
		}

		public void SetInputs(InputStartingValues inputStartingValues)
		{
			inputStartingValues.Keys.ToList().ForEach(key => SetInput(key, inputStartingValues[key]));
		}

		#endregion
	}
}
