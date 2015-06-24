﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;

namespace ZoneLighting.StockPrograms
{
	/// <summary>
	/// Outputs a looping rainbow to the zone (currently only works with FadeCandy).
	/// </summary>
	[Export(typeof(ZoneProgram))]
	[ExportMetadata("Name", "Shimmer")]
	public class Shimmer : LoopingZoneProgram
	{
		public int MaxFadeSpeed { get; set; } = 127;
		public int MaxFadeDelay { get; set; } = 1;
		public int Density { get; set; } = 1;
		public double Brightness { get; set; } = 1.0;

		private readonly Random Random = new Random();

		public override SyncLevel SyncLevel { get; set; } = SyncLevel.None;

		public override void Setup()
		{
			AddMappedInput<int>(this, "MaxFadeSpeed", i => i.IsInRange(1, 127));
			AddMappedInput<int>(this, "MaxFadeDelay", i => i.IsInRange(0, 100));
			AddMappedInput<int>(this, "Density", i => i.IsInRange(1, Zone.LightCount * 2));
			AddMappedInput<double>(this, "Brightness", i => i.IsInRange(0, 1));
		}

		private readonly List<Task> Tasks = new List<Task>();

		private bool[] PixelStates;

		private Dictionary<int, Color> ColorsToSend { get; } = new Dictionary<int, Color>();

		protected override void StartSubCore()
		{
			for (int i = 0; i < Density; i++)
			{
				Tasks.Add(new Task(SingleShimmer, TaskCreationOptions.LongRunning));
			}
			PixelStates = new bool[Zone.LightCount];
			ShimmerCTS = new CancellationTokenSource();
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

		protected override void StopSubCore(bool force)
		{
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
			int pixelToShine = Random.Next(Zone.LightCount);
			while (PixelStates[pixelToShine])
			{
				pixelToShine = Random.Next(Zone.LightCount);
			}
			PixelStates[pixelToShine] = true;
			var fadeSpeed = Random.Next(MaxFadeSpeed);
			var delayTime = Random.Next(MaxFadeDelay);
			Color? endingColor;

			ProgramCommon.Fade(Color.Black, ProgramCommon.GetRandomColor().Darken(Brightness), fadeSpeed, delayTime, false, color =>
			{
				lock (ColorsToSend)
				{
					ColorsToSend[pixelToShine] = color;
				}

			}, out endingColor, cts: ShimmerCTS);

			ProgramCommon.FadeToBlack(GetColor(pixelToShine), fadeSpeed, delayTime, false, color =>
			{
				lock (ColorsToSend)
				{
					ColorsToSend[pixelToShine] = color;
				}

			}, out endingColor, cts: ShimmerCTS);

			PixelStates[pixelToShine] = false;

			if (ShimmerCTS.IsCancellationRequested)
				return;
		}
	}
}
