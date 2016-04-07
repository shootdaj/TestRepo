using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using MIDIator;
using Sanford.Multimedia.Midi;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.StockPrograms
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

		private Random RandomGen { get; } = new Random();

		private MIDIDevice MidiInput { get; set; }

		public override SyncLevel SyncLevel { get; set; } = SyncLevel.None;

		public override void Setup()
		{
			AddMappedInput<int>(this, "MaxFadeSpeed", i => i.IsInRange(1, 127));
			AddMappedInput<int>(this, "MaxFadeDelay", i => i.IsInRange(0, 100));
			AddMappedInput<double>(this, "Density", i => i.IsInRange(0, 1));
			AddMappedInput<double>(this, "Brightness", i => i.IsInRange(0, 1));
			AddMappedInput<bool>(this, "Random");
			AddMappedInput<ColorScheme>(this, "ColorScheme");
			AddMappedInput<bool>(this, "Sparkle");
			AddMappedInput<int>(this, "SparkleHigh", i => i.IsInRange(0, LightCount) && i > SparkleLow);
			AddMappedInput<int>(this, "SparkleLow", i => i.IsInRange(0, LightCount) && i < SparkleHigh);
			AddMappedInput<int>(this, "SparkleIntensity", i => i.IsInRange(0, 100));
		}

		private readonly List<Task> Tasks = new List<Task>();

		private bool[] PixelStates;

		private Dictionary<int, Color> ColorsToSend { get; } = new Dictionary<int, Color>();

		protected override void PreStart()
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
			MidiInput = MIDIManager.GetDevice(parameters?.DeviceID);
			MidiInput.AddChannelMessageAction((Action));
			MidiInput.StartRecording();

			base.StartCore(null, forceStoppable);
		}

		private void Action(object sender, ChannelMessageEventArgs args)
		{
			//do stuff
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
