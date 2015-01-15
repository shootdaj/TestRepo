using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using ZoneLighting.Communication;
using ZoneLighting.TriggerDependencyNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.ZoneNS
{
	/// <summary>
	/// Represents a zone that contains the lights to be controlled. A zone is the unit of control 
	/// from the ZoneLightingManager's point of view.
	/// </summary>
	[DataContract]
	public class Zone : IDisposable
	{
		#region CORE

		/// <summary>
		/// Name of the zone.
		/// </summary>
		[DataMember]
		public string Name;

		/// <summary>
		/// All lights in the zone.
		/// </summary>
		private IList<ILogicalRGBLight> Lights { get; set; }

		public void SetLights()
		{
			
		}

		/// <summary>
		/// The Lights list as a dictionary with the logical index as the key and the light as the value.
		/// </summary>
		public Dictionary<int, ILogicalRGBLight> SortedLights
		{
			get { return Lights.ToDictionary(x => x.LogicalIndex); }
		}

		/// <summary>
		/// The Lighting Controller used to control this Zone.
		/// </summary>
		public LightingController LightingController { get; private set; }

		/// <summary>
		/// The program that is active on this zone.
		/// </summary>
		[DataMember]
		public ZoneProgram ZoneProgram { get; private set; }

		/// <summary>
		/// Programs that can interrupt the main program for things such as notification.
		/// </summary>
		[DataMember]
		public IList<ReactiveZoneProgram> InterruptingPrograms { get; private set; } = new List<ReactiveZoneProgram>();

		/// <summary>
		/// This queue is used to 
		/// </summary>
		private ActionBlock<InterruptInfo> InterruptQueue { get; set; }

		/// <summary>
		/// Brightness of this zone.
		/// </summary>
		public double Brightness { get; set; }

		/// <summary>
		/// This sync context will be responsible for synchronizing the background program for this zone.
		/// </summary>
		public SyncContext SyncContext { get; set; }

		/// <summary>
		/// This sync context will be the source of synchronizations that is passed to the interrupting program routines.
		/// </summary>
		public SyncContext InterruptingProgramSyncContext { get; set; }

		#endregion

		#region C+I

		/// <summary>
		/// If inputStartingValues is provided, the zone is initialized automatically.
		/// </summary>
		/// <param name="lightingController"></param>
		/// <param name="name"></param>
		/// <param name="program"></param>
		/// <param name="inputStartingValues"></param>
		/// <param name="barrier"></param>
		/// <param name="syncContext"></param>
		/// <param name="brightness">Brightness for this zone.</param>
		public Zone(LightingController lightingController, string name = "", ZoneProgram program = null, InputStartingValues inputStartingValues = null, double? brightness = 1.0)
		{
			Lights = new List<ILogicalRGBLight>();
			LightingController = lightingController;
			Name = name;
			Brightness = brightness ?? 1.0;

			//start program, if one is passed in
			if (program == null) return;
			if (inputStartingValues == null)
			{
				SetProgram(program);
			}
			else
			{
				Initialize(program, inputStartingValues);
			}
		}

		public void SetupInterruptProcessing()
		{
			//configure interrupt processing
			InterruptQueue = new ActionBlock<InterruptInfo>((interruptInfo) =>
			{
				InterruptingProgramSyncContext?.SignalAndWait();
				ProcessInterrupt(interruptInfo);
			}, new ExecutionDataflowBlockOptions()
			{
				MaxDegreeOfParallelism = 1,
			});
		}
		
		private void ProcessInterrupt(InterruptInfo interruptInfo)
		{
			//DebugTools.AddEvent("InterruptQueue.Method", "START IRQ processing");

			//when a new request to interrupt the background program is detected, check if the request is coming from the BG program. 
			//if it is, then no need to pause the bg program -- really?? check the TODO on the next line
			if (!ZoneProgram.Inputs.Any(input => input.IsInputSubjectSameAs(interruptInfo.InputSubject)))
			//TODO: Is this check needed? If the BG program's interrupting input is set, what to do? What problem is this if statement solving?
			{
				DebugTools.AddEvent("InterruptQueue.Method", "IRQ is from a foreground program");

				if (!interruptInfo.StopSubject.HasObservers) //only subscribe if the stopsubject isn't already subscribed
				{
					interruptInfo.StopSubject.Subscribe(data =>
					{
						if (InterruptQueue.InputCount < 1)
						{
							DebugTools.AddEvent("InterruptingInput.StopSubject.Method", "START Resume BG Program");
							SyncContext?.SignalAndWait();
							ZoneProgram.ResumeCore();
							DebugTools.AddEvent("InterruptingInput.StopSubject.Method", "END Resume BG Program");
						}
					});	//hook up the stop call of the interrupting input's program to resume the BG program
				}

				DebugTools.AddEvent("InterruptQueue.Method", "START Pause BG Program");
				ZoneProgram.PauseCore(); //pause the bg program
				SyncContext?.SignalAndWait();
				DebugTools.AddEvent("InterruptQueue.Method", "END Pause BG Program");
			}
			else
			{
				DebugTools.AddEvent("InterruptQueue.Method", "IRQ is from the background program. No ");
			}

			DebugTools.AddEvent("InterruptQueue.Method", "START Interrupting Action");
			interruptInfo.InputSubject.OnNext(interruptInfo.Data);	 //start the routine that was requested

			DebugTools.AddEvent("InterruptQueue.Method", "END Interrupting Action");

			//DebugTools.AddEvent("InterruptQueue.Method", "END Interrupt request processing");

			//TODO: Add capability to have a timeout in case the interrupting program never calls the StopSubject
		}

		private void Initialize(InputStartingValues inputStartingValues = null, bool dontStart = false)
		{
			if (!Initialized)
			{
				if (ZoneProgram != null && !dontStart)
				{
					StartProgram(inputStartingValues);
				}
				Initialized = true;
			}
		}

		public void Initialize(ZoneProgram zoneProgram, InputStartingValues inputStartingValues = null, bool dontStart = false)
		{
			if (!Initialized)
			{
				SetProgram(zoneProgram);
				Initialize(inputStartingValues, dontStart);
			}
		}

		public bool Initialized { get; private set; }

		public void Uninitialize(bool force = false)
		{
			if (Initialized)
			{
				StopProgram(force);
				RemoveAllInterruptingPrograms();
				UnsetProgram();
				UnsetupSyncContext();
				Initialized = false;
			}
		}

		public void Dispose(bool force)
		{
			Uninitialize(force);
			Lights.Clear();
			Lights = null;
			ZoneProgram = null;
			InterruptingPrograms.ToList().ForEach(p => p.Dispose());
			InterruptingPrograms.Clear();
			InterruptingPrograms = null;
			InterruptQueue = null;
			LightingController = null;
			Name = null;
		}

		public void Dispose()
		{
			Dispose(false);
		}

		#endregion

		#region MISC

		public override string ToString()
		{
			return Name;
		}

		#endregion

		#region API

		#region ZoneProgram

		public bool IsProgramLooping => ZoneProgram is LoopingZoneProgram;
		public Trigger WaitForSync => ((LoopingZoneProgram)ZoneProgram).WaitForSync;
		public Trigger IsSynchronizable => ((LoopingZoneProgram) ZoneProgram).IsSynchronizable;
		public void RequestSyncState()
		{
			((LoopingZoneProgram) ZoneProgram).RequestSyncState();
		}
		
		public SyncLevel SyncLevel
		{
			get { return ((LoopingZoneProgram) ZoneProgram).SyncLevel; }
			set { ((LoopingZoneProgram) ZoneProgram).SyncLevel = value; }
		}

		#endregion

		#region Transport Controls

		/// <summary>
		/// Sets this zone's program to the given program.
		/// </summary>
		/// <param name="program"></param>
		public void SetProgram(ZoneProgram program)
		{
			ZoneProgram = program;
			ZoneProgram.Zone = this;
		}

		/// <summary>
		/// Unhooks the zone program from this zone.
		/// </summary>
		public void UnsetProgram()
		{
			ZoneProgram.Zone = null;
			ZoneProgram = null;
		}

		public void SetupSyncContext(SyncContext syncContext)
		{
			//sync context setup
			AttachSyncContext(syncContext);
			syncContext?.MakeZoneParticipant(this);
		}

		public void UnsetupSyncContext()
		{
			SyncContext?.RemoveZoneParticipant(this);
			DetachSyncContext();
		}

		/// <summary>
		/// Starts this zone's program.
		/// </summary>
		public void StartProgram(InputStartingValues inputStartingValues = null, bool isSyncRequested = false)
		{
			//setup interrupt processing
			SetupInterruptProcessing();

			//start program
			ZoneProgram.Start(inputStartingValues, InterruptQueue, isSyncRequested);
		}
	
		/// <summary>
		/// Stops this zone's program.
		/// </summary>
		public void StopProgram(bool force = false)
		{
			//sync context removal
			SyncContext?.RemoveZoneParticipant(this);

			//stop program
			ZoneProgram.Stop(force);
		}

		#endregion

		#region Colors/Light Controls

		/// <summary>
		/// Sets all lights in zone to a given color.
		/// </summary>
		/// <param name="color"></param>
		public void SetAllLightsColor(Color color)
		{
			Lights.ToList().ForEach(x => x.SetColor(color));
		}

		/// <summary>
		/// Adds a new light to this zone.
		/// </summary>
		/// <param name="light"></param>
		public void AddLight(ILogicalRGBLight light)
		{
			Lights.Add(light);
		}

		/// <summary>
		/// Gets the number of lights in this zone.
		/// </summary>
		public int LightCount => Lights.Count;

		/// <summary>
		/// Gets the color of the light at the given index.
		/// </summary>
		public Color GetColor(int index)
		{
			return Lights[index].GetColor();
		}

		public void SetColor(Color color, int? index)
		{
			//TODO: This equation is not correct. Check the following links
			//1. http://www.nbdtech.com/Blog/archive/2008/04/27/Calculating-the-Perceived-Brightness-of-a-Color.aspx
			//2. http://stackoverflow.com/questions/596216/formula-to-determine-brightness-of-rgb-color
			var brightnessAdjustedColor = color; //TODO: Change //Color.FromArgb((int) (255*Brightness), color.R, color.G, color.B);

			if (index == null)
				Lights.SetColor(brightnessAdjustedColor);
			else
			{
				Lights[(int)index].SetColor(brightnessAdjustedColor);
			}

		}

		public void SendLights()
		{
			Lights.Send(LightingController);
		}

		#endregion

		#region Interrupting Program
		
		/// <summary>
		/// Starts the given interrupting program.
		/// </summary>
		/// <param name="interruptingProgram">Interrupting program to start</param>
		/// <param name="inputStartingValues">Input values to start program with</param>
		public void StartInterruptingProgram(ReactiveZoneProgram interruptingProgram, InputStartingValues inputStartingValues = null, bool isSyncRequested = false)
		{
			//tell the interrupting to start, saying "use these input starting values to start the program, report back to this queue when 
			//you need to output something (interrupt), and I'll take care of the rest. Also use this barrier to synchronize yourself
			//with everyone else on this barrier's participant list.
			interruptingProgram.Start(inputStartingValues, InterruptQueue, isSyncRequested: isSyncRequested);
		}

		/// <summary>
		/// Stops the given interrupting program.
		/// </summary>
		public void StopInterruptingProgram(ReactiveZoneProgram interruptingProgram, bool force = false)
		{
			interruptingProgram.Stop(force);
		}


		/// <summary>
		/// Adds an interrupting program to the zone.
		/// </summary>
		/// <param name="interruptingProgram"></param>
		public void AddInterruptingProgram(ReactiveZoneProgram interruptingProgram, bool startProgram = true, InputStartingValues inputStartingValues = null, SyncContext syncContext = null, bool isSyncRequested = false)
		{
			InterruptingPrograms.Add(interruptingProgram);
			interruptingProgram.Zone = this;
			if (startProgram)

				//TODO: Should the InterruptingProgramSyncContext be set here? 

				StartInterruptingProgram(interruptingProgram, inputStartingValues, isSyncRequested);
		}

		public void RemoveInterruptingProgram(string name)
		{
			var interruptingProgram = InterruptingPrograms.First(program => program.Name == name);
			StopInterruptingProgram(interruptingProgram);
			interruptingProgram.Zone = null;
			interruptingProgram.RemoveInterruptQueue();
			InterruptingPrograms.Remove(interruptingProgram);
		}

		public void RemoveAllInterruptingPrograms()
		{
			InterruptingPrograms.ToList().ForEach(program => RemoveInterruptingProgram(program.Name));
		}

		#endregion

		#region SyncContext

		/// <summary>
		/// Attaches the given sync context to this program.
		/// </summary>
		/// <param name="syncContext"></param>
		public void AttachSyncContext(SyncContext syncContext)
		{
			SyncContext = syncContext;
		}

		//public void IncrementSyncContextIfNeeded()
		//{
		//	if (SyncContext == null)
		//		throw new Exception("No SyncContext has been attached.");
		//	SyncContext.MakeZoneParticipant(this);
		//}

		public void DetachSyncContext()
		{
			SyncContext = null;
		}

		//public void DecrementSyncContextIfNeeded()
		//{
		//	if (SyncContext == null)
		//		throw new Exception("No SyncContext has been attached.");
		//	SyncContext.RemoveZoneParticipant(this);
		//}


		#endregion

		#endregion
	}
}
