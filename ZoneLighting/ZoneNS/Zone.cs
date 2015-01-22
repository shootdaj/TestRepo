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
		
		/// <summary>
		/// The Lights list as a dictionary with the logical index as the key and the light as the value.
		/// </summary>
		public Dictionary<int, ILogicalRGBLight> SortedLights
		{
			get { return Lights?.ToDictionary(x => x.LogicalIndex); }
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

		///// <summary>
		///// This sync context will be responsible for synchronizing the background program for this zone.
		///// </summary>
		//public SyncContext SyncContext { get; set; }

		///// <summary>
		///// This sync context will be the source of synchronizations that is passed to the interrupting program routines.
		///// </summary>
		//public SyncContext InterruptingProgramSyncContext { get; set; }

		#endregion

		#region C+I

		/// <summary>
		/// If inputStartingValues is provided, the zone is initialized automatically.
		/// </summary>
		/// <param name="lightingController"></param>
		/// <param name="name"></param>
		/// <param name="brightness">Brightness for this zone.</param>
		public Zone(LightingController lightingController, string name = "", double? brightness = 1.0)
		{
			Lights = new List<ILogicalRGBLight>();
			LightingController = lightingController;
			Name = name;
			Brightness = brightness ?? 1.0;
		}

		#region Interrupt Processing

		private void SetupInterruptProcessing()
		{
			//configure interrupt processing
			InterruptQueue = new ActionBlock<InterruptInfo>((interruptInfo) =>
			{
				ProcessInterrupt(interruptInfo);
			}, new ExecutionDataflowBlockOptions()
			{
				MaxDegreeOfParallelism = 1,
			});
		}

		private void ProcessInterrupt(InterruptInfo interruptInfo)
		{
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
							
							interruptInfo.ZoneProgram.LightingController = null;
							ZoneProgram.LightingController = LightingController;

							DebugTools.AddEvent("InterruptingInput.StopSubject.Method", "END Resume BG Program");
						}
					});	//hook up the stop call of the interrupting input's program to resume the BG program
				}

				DebugTools.AddEvent("InterruptQueue.Method", "START Pause BG Program");
				
				ZoneProgram.LightingController = null;
				interruptInfo.ZoneProgram.LightingController = LightingController;

				DebugTools.AddEvent("InterruptQueue.Method", "END Pause BG Program");
			}
			else
			{
				DebugTools.AddEvent("InterruptQueue.Method", "IRQ is from the background program. No ");
			}

			DebugTools.AddEvent("InterruptQueue.Method", "START Interrupting Action");
			//interruptInfo.ZoneProgram.LightingController = LightingController;
			interruptInfo.InputSubject.OnNext(interruptInfo.Data);	 //start the routine that was requested

			DebugTools.AddEvent("InterruptQueue.Method", "END Interrupting Action");

			//DebugTools.AddEvent("InterruptQueue.Method", "END Interrupt request processing");

			//TODO: Add capability to have a timeout in case the interrupting program never calls the StopSubject
		}

		private void UnsetupInterruptProcessing()
		{
			InterruptQueue = null;
		}

		#endregion

		public void Initialize(ZoneProgram zoneProgram, InputStartingValues inputStartingValues = null, bool isSyncRequested = false, SyncContext syncContext = null)
		{
			if (!Initialized)
			{
				SetProgram(zoneProgram);
				SetupInterruptProcessing();
				ZoneProgram.LightingController = LightingController;
				ZoneProgram.SetSyncContext(syncContext);
				ZoneProgram.Start(inputStartingValues, InterruptQueue, isSyncRequested);
				Initialized = true;
			}
		}

		public bool Initialized { get; private set; }

		public void Uninitialize(bool force = false)
		{
			if (Initialized)
			{
				ZoneProgram.Dispose();
				UnsetProgram();
				UnsetupInterruptProcessing();
				DisposeAllInterruptingPrograms();
				InterruptingPrograms.Clear();
				Initialized = false;
			}
		}

		public void Pause()
		{
			
		}

		public void Resume()
		{
			
		}

		public void Dispose(bool force)
		{
			Uninitialize(force);
			Lights.Clear();
			Lights = null;
			ZoneProgram = null;
			InterruptingPrograms = null;
			LightingController = null;
			Brightness = 0;
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
		public Trigger WaitForSync => ZoneProgram.WaitForSync;
		public Trigger IsSynchronizable => ZoneProgram.IsSynchronizable;
		public void RequestSyncState()
		{
			ZoneProgram.RequestSyncState();
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
		private void SetProgram(ZoneProgram program)
		{
			ZoneProgram = program;
			ZoneProgram.Zone = this;
		}

		/// <summary>
		/// Unhooks the zone program from this zone.
		/// </summary>
		public void UnsetProgram()
		{
			//Zone.Program = null happens during the dispose of program
			ZoneProgram = null;
		}

		//public void SetupSyncContext(SyncContext syncContext)
		//{
		//	SyncContext = syncContext;
		//	ZoneProgram.SyncContext = syncContext;
		//	syncContext?.MakeZoneParticipant(this);
		//}

		//public void UnsetupSyncContext()
		//{
		//	SyncContext?.RemoveZoneParticipant(this);
		//	SyncContext = null;
		//}

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

		public void SendLights(LightingController lightingController)
		{
			Lights.Send(lightingController);
		}

		#endregion

		#region Interrupting Program
		
		/// <summary>
		/// Adds an interrupting program to the zone.
		/// </summary>
		/// <param name="interruptingProgram"></param>
		public void StartInterruptingProgram(ReactiveZoneProgram interruptingProgram, InputStartingValues inputStartingValues = null, SyncContext syncContext = null, bool isSyncRequested = false)
		{
			InterruptingPrograms.Add(interruptingProgram);
			interruptingProgram.Zone = this;
				
			//tell the interrupting to start, saying "use these input starting values to start the program, report back to this queue when 
			//you need to output something (interrupt), and I'll take care of the rest. Also use this barrier to synchronize yourself
			//with everyone else on this barrier's participant list.
			interruptingProgram.Start(inputStartingValues, InterruptQueue, isSyncRequested);
		}

		public void DisposeInterruptingProgram(string name, bool force = false)
		{
			var interruptingProgram = InterruptingPrograms.First(program => program.Name == name);
			interruptingProgram.Dispose();
			interruptingProgram.Zone = null;
			InterruptingPrograms.Remove(interruptingProgram);
		}

		public void DisposeAllInterruptingPrograms()
		{
			InterruptingPrograms.ToList().ForEach(program => DisposeInterruptingProgram(program.Name));
		}

		//public void SetupInterruptingProgramSyncContext(SyncContext syncContext, ReactiveZoneProgram zoneProgram)
		//{
		//	InterruptingProgramSyncContext = syncContext;
		//	//zoneProgram.SyncContext.
		//	zoneProgram.SyncContext = syncContext;
		//	InterruptingProgramSyncContext?.MakeZoneParticipant(this);
		//}
		
		//private void UnsetupInterruptingProgramSyncContext(ZoneProgram zoneProgram)
		//{
		//	InterruptingProgramSyncContext?.RemoveZoneParticipant(this);
			
		//	InterruptingProgramSyncContext = null;
		//}



		#endregion

		#endregion
	}
}
