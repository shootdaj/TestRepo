using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
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
		public LightingController LightingController { get; set; }

		/// <summary>
		/// Inputs for this program.
		/// </summary>
		[DataMember]
		public ZoneProgramInputCollection Inputs { get; private set; } = new ZoneProgramInputCollection();

		public Trigger StopTestingTrigger { get; } = new Trigger("ZoneProgram.StopTestingTrigger");

		public ProgramState State { get; private set; } = ProgramState.Stopped;

		public SyncContext SyncContext { get; set; }
		
		#region Triggers

		public Trigger StartTrigger { get; } = new Trigger("StartTrigger");
		
		#endregion

		#endregion CORE

		#region C+I+D

		protected ZoneProgram(string name, SyncContext syncContext = null, ActionBlock<InterruptInfo> interruptQueue = null)
		{
			Name = name;
			Construct(syncContext, interruptQueue);
		}

		protected ZoneProgram(SyncContext syncContext = null, ActionBlock<InterruptInfo> interruptQueue = null)
		{
			Type thisType = this.GetType();
			//set the name of the program based on attribute, if any
			if (thisType.GetCustomAttributes(typeof (ExportMetadataAttribute), false).Any())
				Name =
					(string) thisType.GetCustomAttributes(typeof (ExportMetadataAttribute), false)
						.Cast<ExportMetadataAttribute>().First(attr => attr.Name == "Name").Value;
			Construct(syncContext, interruptQueue);
		}

		private void Construct(SyncContext syncContext = null, ActionBlock<InterruptInfo> interruptQueue = null)
		{
			StopTrigger = new Trigger("StopTrigger");
			SyncContext = syncContext;

			//set the interrupt queue
			if (interruptQueue != null)
				SetInterruptQueue(interruptQueue);
		}

		/// <summary>
		/// Starts the zone program.
		/// </summary>
		/// <param name="inputStartingValues">Starting values for the program.</param>
		/// <param name="interruptQueue">InterruptQueue to be used for interrupting inputs.</param>
		/// <param name="isSyncRequested"></param>
		public void Start(InputStartingValues inputStartingValues = null, bool liveSync = false)
		{
			if (State == ProgramState.Stopped)
			{
				if (liveSync && SyncContext != null)
					SyncContext?.SyncAndStartLive(this);
				else
				{
					//subclass processing
					StartCore(); //isSyncRequested);

					//set starting values
					SetStartingValues(inputStartingValues);

					//set program state
					State = ProgramState.Started;
				}
			}
			else
			{
				throw new Exception("Program is already started.");
			}
		}

		public void Stop(bool force = false)
		{
			if (State == ProgramState.Started)
			{
				//subclass processing
				StopCore(force);

				//remove from synccontext
				SyncContext?.Unsync(this);

				//set program state
				State = ProgramState.Stopped;
			}
			else
			{
				throw new Exception("Program is already stopped.");
			}
		}

		public void Dispose()
		{
			Dispose(false);
		}

		public virtual void Dispose(bool force)
		{
			Stop(force);
			RemoveAllInputs();
            UnsetInterruptQueue();		//unset the interrupt queue
			SyncContext = null;
			Name = null;
			Zone = null;
			StopTrigger.Dispose();
			StopTestingTrigger.Dispose();
			StartTrigger.Dispose();
		}

		public void SetSyncContext(SyncContext syncContext)
		{
			if (State == ProgramState.Stopped)
				SyncContext = syncContext;	
			else
				throw new Exception("Can only set sync context while program is stopped.");
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
		protected abstract void StartCore();//bool isSyncRequested);

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
		public void SetInterruptQueue(ActionBlock<InterruptInfo> interruptQueue)
		{
			if (Inputs.Any(input => input is InterruptingInput))
			{
				Inputs.Where(input => input is InterruptingInput)
					.ToList()
					.ForEach(input => ((InterruptingInput)input).SetInterruptQueue(interruptQueue));
			}
		}

		private void UnsetInterruptQueue()
		{
			if (Inputs.Any(input => input is InterruptingInput))
			{
				Inputs.Where(input => input is InterruptingInput)
					.ToList()
					.ForEach(input => ((InterruptingInput) input).UnsetInterruptQueue());
			}
		}

		#endregion

		#region API

		#region Transport Controls

		




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
		/// Removes all inputs
		/// </summary>
		protected void RemoveAllInputs()
		{
			Inputs.ToList().ForEach(input =>
			{
				Inputs.Remove(input);
				input.Dispose();
			});
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
		public void SetInput(string name, object data)
		{
			GetInput(name).SetValue(data);
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

		private void SetColor(Color color, int? index = null)
		{
			Zone.SetColor(color, index);
		}

		private void SendLights()
		{
			if (LightingController != null)
			{
				Zone.SendLights(LightingController);
			}
		}

		public void SendColors(Dictionary<int, Color> colors)
		{
			if (LightingController != null)
			{
				colors.Keys.ToList().ForEach(index => SetColor(colors[index], index));
				SendLights();
			}
		}

		public void SendColor(Color color)
		{
			if (LightingController != null)
			{
				SetColor(color);
				SendLights();
			}
		}


		#endregion

		#endregion
	}

	public enum ProgramState
	{
		None,
		Started,
		Stopped
	}
}
