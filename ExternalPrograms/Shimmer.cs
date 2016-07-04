using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MIDIator;
using Sanford.Multimedia.Midi;
using ZoneLighting.StockPrograms.MIDI;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ExternalPrograms
{
	/// <summary>
	/// Outputs a shimmery pattern
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "Shimmer")]
	public class Shimmer : LoopingZoneProgram
	{
		int MaxFadeSpeed { get; set; } = 1;
		int MaxFadeDelay { get; set; } = 20;
		double Density { get; set; } = 1.0;
		double Brightness { get; set; } = 0.3;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Shimmer"/> will exhibit the Sparkle effect.
		/// This effect means that shimmer of lengths in between SparkleLow and SparkleHigh will be brighter by a factor of SparkleIntensity. 
		/// </summary>
		bool Sparkle { get; set; } = false;
		int SparkleLow { get; set; } = 1;
		int SparkleHigh { get; set; } = 4;

		/// <summary>
		/// Gets or sets the sparkle intensity.
		/// </summary>
		int SparkleIntensity { get; set; } = 3;

		/// <summary>
		/// Set to true to set the delay and speed to be randomly generated with the MaxFadeDelay and MaxFadeSpeed being the maximum value.
		/// </summary>
		bool Random { get; set; } = true;

		ColorScheme ColorScheme { get; set; } = null;

		//private Random RandomGen { get; } = new Random();

		private MIDIDevice MidiInput { get; set; }

		public override SyncLevel SyncLevel { get; set; } = SyncLevel.None;

	    protected override int LoopWaitTime { get; set; } = 10;

		/// <summary>
		/// This overrides Setup() in ZoneProgram. This happens during constructor call.
		/// </summary>
		public override void Setup()
		{
			AddMappedInput<int>(this, "MaxFadeSpeed", i => i.IsInRange(1, 127));
			AddMappedInput<int>(this, "MaxFadeDelay", i => i.IsInRange(0, 100));
			AddMappedInput<double>(this, "Density", i => i.IsInRange(0, 1));
			AddMappedInput<double>(this, "Brightness", i => i.IsInRange(0, 1));
			AddMappedInput<bool>(this, "Random");
			AddMappedInput<ColorScheme>(this, "ColorScheme");
			AddMappedInput<bool>(this, "Sparkle");
			AddMappedInput<int>(this, "SparkleHigh", i => i.IsInRange(0, Zone == null ? int.MaxValue : LightCount) && i > SparkleLow);
			AddMappedInput<int>(this, "SparkleLow", i => i.IsInRange(0, Zone == null ? int.MaxValue : LightCount) && i < SparkleHigh);
			AddMappedInput<int>(this, "SparkleIntensity", i => i.IsInRange(0, 100));
		}

		private readonly List<Task> Tasks = new List<Task>();

		private bool[] PixelStates;

		private Dictionary<int, Color> ColorsToSend { get; } = new Dictionary<int, Color>();

		protected override void PreLoopStart()
		{
			for (int i = 0; i < Math.Floor(Density * Zone.LightCount); i++)
			{
				Tasks.Add(new Task(SingleShimmer, TaskCreationOptions.LongRunning));
			}
			PixelStates = new bool[Zone.LightCount];
			ShimmerCTS = new CancellationTokenSource();
			SendColor(Color.Black);
		}

		protected override void StartCore(dynamic parameters = null, bool forceStoppable = true)
		{
			if (parameters != null)
			{
				int? deviceID = parameters?.DeviceID;
				var freeDevices = MIDIManager.FreeDevices;
				var freeDeviceIDs = freeDevices?.Select(x => x.DeviceID).ToList();
				var isDeviceNull = parameters?.DeviceID == null;

				if (!isDeviceNull && freeDeviceIDs?.Contains(deviceID))
				{
					MidiInput = MIDIManager.GetDevice(parameters.DeviceID);
					MidiInput.AddChannelMessageAction(HandleMidi);
					MidiInput.StartRecording();
				}
				else
					throw new WarningException("Supplied MIDI Device ID is either in use or invalid.");
			}

			base.StartCore(null, forceStoppable);
		}

		private void HandleMidi(object sender, ChannelMessageEventArgs args)
		{
			switch (args.Message.MidiChannel)
			{
				case 0:
				{
					switch (args.Message.Data1)
					{
						case (int) NumarkOrbitMidiNote.XAxis:
						{
							var scaledValue = Anshul.Utilities.Math.Scale(args.Message.Data2, 0, 127, 1, 99);
							//Debug.Print(scaledValue.ToString());
							MaxFadeDelay = scaledValue;
							//SetInput("MaxFadeDelay", scaledValue);
						}
							break;
						case (int) NumarkOrbitMidiNote.YAxis:
						{
							var scaledValue = Anshul.Utilities.Math.Scale(args.Message.Data2, 0, 127, 0.0, 1.0);
							SetInput("Density", scaledValue);
						}
							break;
						case (int) NumarkOrbitMidiNote.K1_BigKnob:
						{
							var scaledValue = Anshul.Utilities.Math.Scale(args.Message.Data2, 0, 127, 0.0, 1.0);
							SetInput("Brightness", scaledValue);
						}
							break;
						case (int) NumarkOrbitMidiNote.A1:
							SetInput("ColorScheme", ColorScheme.All);
							break;
						case (int) NumarkOrbitMidiNote.A2:
							SetInput("ColorScheme", ColorScheme.Primaries);
							break;
						case (int) NumarkOrbitMidiNote.A3:
							SetInput("ColorScheme", ColorScheme.Secondaries);
							break;
						case (int) NumarkOrbitMidiNote.B1:
							SetInput("ColorScheme", ColorScheme.RedsBluesGreens);
							break;
						case (int) NumarkOrbitMidiNote.B2:
							SetInput("ColorScheme", ColorScheme.Reds);
							break;
						case (int) NumarkOrbitMidiNote.B3:
							SetInput("ColorScheme", ColorScheme.Blues);
							break;
						case (int) NumarkOrbitMidiNote.B4:
							SetInput("ColorScheme", ColorScheme.Greens);
							break;
					}
				}
					break;
			}
		}

		public override void Loop()
		{
			if (!ShimmerCTS.IsCancellationRequested)
			{
				for (int i = 0; i < Tasks.Count; i++)
				{
					if (Tasks[i].Status != TaskStatus.Running && Tasks[i].Status != TaskStatus.WaitingToRun)
					{
						Tasks[i] = new Task(SingleShimmer, TaskCreationOptions.LongRunning);
						Tasks[i].Start();
					}
				}
			}
			else
			{

			}

			SendColors(ColorsToSend);
		}

		private CancellationTokenSource ShimmerCTS { get; set; } = new CancellationTokenSource();

		protected override void PreStop(bool force)
		{
			ForceStoppable = force;
			ShimmerCTS.Cancel();
			Task.WaitAll(Tasks.ToArray());
			Tasks.ForEach(task =>
			{
				task.Dispose();
			});
			Tasks.Clear();
			PixelStates = null;
		}

		private void SingleShimmer()
		{
			int pixelToShine = RandomGen.Next(Zone.LightCount);
			while (PixelStates[pixelToShine])
			{
				pixelToShine = RandomGen.Next(Zone.LightCount);
			}
			PixelStates[pixelToShine] = true;
			var fadeSpeed = Random ?
				RandomGen.Next(MaxFadeSpeed) : MaxFadeSpeed;
			var delayTime = Random ?
				RandomGen.Next(MaxFadeDelay) : MaxFadeDelay;
			Color? endingColor;

			var brightness = Sparkle && delayTime >= SparkleLow && delayTime <= SparkleHigh
				? Math.Min(Brightness * SparkleIntensity, 1)
				: Brightness;

			ProgramCommon.Fade(Color.Black, ColorScheme.GetRandomSchemeColor(ColorScheme).Darken(brightness), fadeSpeed, delayTime, false, color =>
			{
				lock (ColorsToSend)
				{
					ColorsToSend[pixelToShine] = color;
				}

			}, out endingColor, cts: ShimmerCTS, forceStoppable: ForceStoppable);

			ProgramCommon.FadeToBlack(GetColor(pixelToShine), fadeSpeed, delayTime, false, color =>
			{
				lock (ColorsToSend)
				{
					ColorsToSend[pixelToShine] = color;
				}

			}, out endingColor, cts: ShimmerCTS, force: ForceStoppable);

			PixelStates[pixelToShine] = false;

			if (ShimmerCTS.IsCancellationRequested)
				return;
		}
	}
}
