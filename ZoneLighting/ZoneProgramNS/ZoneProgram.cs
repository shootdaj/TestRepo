using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks.Dataflow;
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
	public abstract class ZoneProgram : IDisposable
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
		public IList<ILogicalRGBLight> Lights => Zone.Lights;

		//[DataMember]
		//private UntypedZoneProgramInputCollection UntypedInputs { get; set; } = new UntypedZoneProgramInputCollection();

		/// <summary>
		/// Inputs for this program.
		/// </summary>
		[DataMember]
		public ZoneProgramInputCollection Inputs { get; private set; } = new ZoneProgramInputCollection();

		public Trigger StopTestingTrigger { get; } = new Trigger("ZoneProgram.StopTestingTrigger");

		#endregion CORE

		#region C+I+D

		protected ZoneProgram(string name)
		{
			Name = name;
			Construct();
		}

		protected ZoneProgram()
		{
			Type thisType = this.GetType();
			//set the name of the program based on attribute, if any
			if (thisType.GetCustomAttributes(typeof (ExportMetadataAttribute), false).Any())
				Name =
					(string) thisType.GetCustomAttributes(typeof (ExportMetadataAttribute), false)
						.Cast<ExportMetadataAttribute>().First(attr => attr.Name == "Name").Value;
			Construct();
		}

		private void Construct()
		{
			StopTrigger = new Trigger("StopTrigger");
		}

		public void PauseCore()
		{
			PauseTrigger.Fire(this, null);
			Pause();
		}

		protected abstract void Pause();

		public void ResumeCore()
		{

			ResumeTrigger.Fire(this, null);
			Resume();
		}

		public abstract void Resume();

		public void Dispose()
		{
			Name = null;
			Zone = null;
			StopTrigger.Dispose(true);
			StartTrigger.Dispose();
			PauseTrigger.Dispose();
			ResumeTrigger.Dispose();
		}

		#endregion

		#region Base Methods

		public virtual void Start(InputStartingValues inputStartingValues = null, ActionBlock<InterruptInfo> interruptQueue = null)
		{
			StartTrigger.Fire(this, null);

			StartCore();

			if (Inputs.Any(input => input is InterruptingInput))
			{
				Inputs.Where(input => input is InterruptingInput).ToList().ForEach(input =>
				((InterruptingInput)input).SetInterruptQueue(interruptQueue));
			}

			if (inputStartingValues != null)
				SetInputs(inputStartingValues);
		}

		#endregion

		#region Overridables

		protected abstract void StartCore();
		public abstract void Stop(bool force);

		#endregion

		#region Triggers

		public Trigger StartTrigger { get; } = new Trigger("StartTrigger");
		public Trigger PauseTrigger { get; } = new Trigger("PauseTrigger");
		public Trigger ResumeTrigger { get; } = new Trigger("ResumeTrigger");

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

		public object GetInputValue(string name)
		{
			return GetInputValues()[name];
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
		/// <param name="action">The action that should occur when the input is set to a certain value. This will be defined by the 
		/// subclasses of this class to perform certain actions when the this input is set to a value.</param>
		/// <returns>The input that was just added.</returns>
		protected ZoneProgramInput AddInput<T>(string name, Action<object> action)
		{
			var input = new ZoneProgramInput(name, typeof(T));
			Inputs.Add(input);
			input.Subscribe(action);
			return input;
		}

		protected ZoneProgramInput AddMappedInput<T>(object instance, string propertyName)
		{
			var propertyInfo = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
			var input = new ZoneProgramInput(propertyInfo.Name, propertyInfo.PropertyType);
			Inputs.Add(input);
			input.Subscribe(incomingValue => propertyInfo.SetValue(instance, incomingValue));
			return input;
		}

		protected void RemoveInput(string name)
		{
			GetInput(name).Unsubscribe();
		}

		//public void AddInputSubscription(string name, Action<object> action)
		//{
		//	GetInput(name).Subscribe(action);
		//}

		public ZoneProgramInput GetInput(string name)
		{
			if (Inputs.Contains(name))
				return Inputs[name];
			else
			{
				throw new Exception("No input with the name '" + name + "' found in this program.");
			}
		}

		public void SetInput(string name, object data)
		{
			GetInput(name).SetValue(data);
		}

		public void SetInputs(InputStartingValues inputStartingValues)
		{
			inputStartingValues.Keys.ToList().ForEach(key => SetInput(key, inputStartingValues[key]));
		}

		#endregion
	}
}
