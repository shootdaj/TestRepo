using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
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
		public LightingController LightingController => Zone.LightingController;

		//[DataMember]
		//private UntypedZoneProgramInputCollection UntypedInputs { get; set; } = new UntypedZoneProgramInputCollection();

		/// <summary>
		/// Inputs for this program.
		/// </summary>
		[DataMember]
		public ZoneProgramInputCollection Inputs { get; private set; } = new ZoneProgramInputCollection();

		public Trigger StopTestingTrigger { get; } = new Trigger("ZoneProgram.StopTestingTrigger");

		public Barrier Barrier { get; protected set; }

		#region Triggers

		public Trigger StartTrigger { get; } = new Trigger("StartTrigger");
		public Trigger PauseTrigger { get; } = new Trigger("PauseTrigger");
		public Trigger ResumeTrigger { get; } = new Trigger("ResumeTrigger");

		#endregion

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

		public void ResumeCore(Barrier barrier)
		{
			ResumeTrigger.Fire(this, null);
			Resume(barrier);
		}

		public abstract void Resume(Barrier barrier);

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

		#region Overridables

		/// <summary>
		/// This is a core method, meaning this method is wrapped within another method that's required for 
		/// any pre/postprocessing that is required by this class to maintain the functions provided by this class. 
		/// This is the core method for the Start public API call, which is a public member of this class. 
		/// So the inheritor of this class must provide the core functionality, but the user of an instance
		/// of this class can only call the method that wraps this method - Start. 
		/// </summary>
		/// <param name="barrier"></param>
		protected abstract void StartCore(Barrier barrier);

		/// This is a core method, meaning this method is wrapped within another method that's required for 
		/// any pre/postprocessing that is required by this class to maintain the functions provided by this class. 
		/// This is the core method for the Stop public API call, which is a public member of this class. 
		/// So the inheritor of this class must provide the core functionality, but the user of an instance
		/// of this class can only call the method that wraps this method - Stop.
		/// <param name="force"></param>
		protected abstract void StopCore(bool force);

		#endregion

		#region Helpers

		private void SetStartingValues(InputStartingValues inputStartingValues)
		{
			if (inputStartingValues != null)
				SetInputs(inputStartingValues);
		}

		/// <summary>
		/// Assigns the interrupt queue to be posted to when an interrupting input is set.
		/// </summary>
		/// <param name="interruptQueue"></param>
		private void AssignInterruptQueue(ActionBlock<InterruptInfo> interruptQueue)
		{
			if (Inputs.Any(input => input is InterruptingInput))
			{
				Inputs.Where(input => input is InterruptingInput).ToList().ForEach(input =>
					((InterruptingInput)input).SetInterruptQueue(interruptQueue));
			}
		}

		private void ClearInterruptQueue()
		{
			if (Inputs.Any(input => input is InterruptingInput))
			{
				Inputs.Where(input => input is InterruptingInput).ToList().ForEach(input =>
					((InterruptingInput)input).ClearInterruptQueue());
			}
		}

		#endregion

		#region API

		#region Transport Controls

		/// <summary>
		/// Starts the zone program.
		/// </summary>
		/// <param name="inputStartingValues">Starting values for the program.</param>
		/// <param name="interruptQueue">InterruptQueue to be used for interrupting inputs.</param>
		/// <param name="barrier">Barrier to use to synchronize this </param>
		public virtual void Start(InputStartingValues inputStartingValues = null, ActionBlock<InterruptInfo> interruptQueue = null, Barrier barrier = null)
		{
			//preprocessing
			StartTrigger.Fire(this, null);
			
			//processing
			StartCore(barrier);

			//postprocessing
			AssignInterruptQueue(interruptQueue);
			SetStartingValues(inputStartingValues);
		}

		/// <summary>
		/// Used to attach a Barrier to this 
		/// </summary>
		/// <param name="barrier"></param>
		public void AttachBarrier(Barrier barrier)
		{
			Barrier = barrier;
			Barrier?.AddParticipant();
		}

		public void DetachBarrier()
		{
			Barrier?.RemoveParticipant();
			Barrier = null;
		}

		/// <summary>
		/// Stops the zone program.
		/// </summary>
		/// <param name="force"></param>
		public virtual void Stop(bool force)
		{
			Barrier?.RemoveParticipant();
			ClearInterruptQueue();
			StopCore(force);
		}

		#endregion

		#region Inputs

		/// <summary>
		/// Returns the names of all inputs.
		/// </summary>
		/// <returns></returns>
		public List<string> GetInputNames()
		{
			return Inputs.Select(input => input.Name).ToList();
		}

		/// <summary>
		/// Returns a key-value pair of the current values of this program's inputs.
		/// </summary>
		/// <returns></returns>
		public InputStartingValues GetInputValues()
		{
			var inputStartingValues = new InputStartingValues();
			Inputs.ToList().ForEach(input => inputStartingValues.Add(input.Name, input.Value));
			return inputStartingValues;
		}

		/// <summary>
		/// Get a specific input value by name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object GetInputValue(string name)
		{
			return GetInputValues()[name];
		}

		/// <summary>
		/// Returns the names of all inputs of all types T.
		/// </summary>
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
		/// subclasses of this class to perform certain actions when the this input is set to a certain value.</param>
		/// <returns>The input that was just added.</returns>
		protected ZoneProgramInput AddInput<T>(string name, Action<object> action)
		{
			var input = new ZoneProgramInput(name, typeof(T));
			Inputs.Add(input);
			input.Subscribe(action);
			return input;
		}

		/// <summary>
		/// Adds an input as a direct map to a member of the subclass. This essentially extends a property in 
		/// the subclass as an input, which can be set to any value by the user.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		protected ZoneProgramInput AddMappedInput<T>(object instance, string propertyName)
		{
			var propertyInfo = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
			var input = new ZoneProgramInput(propertyInfo.Name, propertyInfo.PropertyType);
			Inputs.Add(input);
			input.Subscribe(incomingValue => propertyInfo.SetValue(instance, incomingValue));
			return input;
		}

		/// <summary>
		/// Removes an input from the program.
		/// </summary>
		/// <param name="name"></param>
		protected void RemoveInput(string name)
		{
			GetInput(name).Unsubscribe();
		}

		/// <summary>
		/// Gets an input by its name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ZoneProgramInput GetInput(string name)
		{
			if (Inputs.Contains(name))
				return Inputs[name];
			else
			{
				throw new Exception("No input with the name '" + name + "' found in this program.");
			}
		}

		/// <summary>
		/// Sets the input with the given name to the given data.
		/// </summary>
		public void SetInput(string name, object data, Barrier barrier = null)
		{
			GetInput(name).SetValue(data);
			AttachBarrier(barrier);
		}

		/// <summary>
		/// Batch set inputs.
		/// </summary>
		/// <param name="inputStartingValues"></param>
		public void SetInputs(InputStartingValues inputStartingValues)
		{
			inputStartingValues.Keys.ToList().ForEach(key => SetInput(key, inputStartingValues[key]));
		}

		#endregion

		#region Lights

		public int LightCount => Zone.LightCount;

		/// <summary>
		/// Gets the color of the light at the given index.
		/// </summary>
		public Color GetColor(int index)
		{
			return Zone.GetColor(index);
		}

		public void SetColor(Color color, int? index = null)
		{
			Zone.SetColor(color, index);
		}

		public void SendLights()
		{
			Zone.SendLights();
		}

		#endregion

		#endregion
	}
}
