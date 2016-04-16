using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ZoneProgramNS
{
	public class ProgramCommon
	{
		#region Non-Trivial

		/// <summary>
		/// Delays the execution for the specified amount of milliseconds.
		/// </summary>
		/// <param name="milliseconds"></param>
		public static void Delay(int milliseconds)
		{
			if (milliseconds > 0)
				Thread.Sleep(milliseconds);
		}

		/// <summary>
		/// Fades from one color to another color.
		/// </summary>
		/// <param name="colorStart">Color to fade from.</param>
		/// <param name="colorEnd">Color to fade to.</param>
		/// <param name="speed">The higher the speed, the more abruptly the colors will change. Max is 127.</param>
		/// <param name="sleepTime">How long each color set is displayed</param>
		/// <param name="loop">Whether or not to loop forever</param>
		/// <param name="outputMethod">Method to be used for outputting the fade.</param>
		/// <param name="endingColor">Last color the fade sets.</param>
		/// <param name="syncContext">Whether or not the fade should by synchronized with a SyncContext.</param>
		/// <param name="reverse">If true, after the fade is complete, another fade will occuur in from colorEnd to colorStart</param>
		public static void Fade(Color colorStart, Color colorEnd, int speed, int sleepTime, bool loop, Action<Color> outputMethod, out Color? endingColor, SyncContext syncContext = null, bool reverse = false, CancellationTokenSource cts = null, bool forceStoppable = false)
		{
			if (speed > 127)
				throw new Exception("Speed cannot exceed 127.");

			var firstLoop = true;
			Color? currentColor = null;

			while (firstLoop || loop)
			{
				firstLoop = false;

				float redDiff = (colorEnd.R - colorStart.R);
				float greenDiff = (colorEnd.G - colorStart.G);
				float blueDiff = (colorEnd.B - colorStart.B);

				var redJump = redDiff / (128 - speed);
				var greenJump = greenDiff / (128 - speed);
				var blueJump = blueDiff / (128 - speed);

				float redLevel = colorStart.R;
				float greenLevel = colorStart.G;
				float blueLevel = colorStart.B;

				//fade
				for (var a = 0; a < (128 - speed); a++)
				{
					redLevel += redJump;
					greenLevel += greenJump;
					blueLevel += blueJump;

					var colorToOutput = Color.FromArgb(255, (int)redLevel, (int)greenLevel, (int)blueLevel);
					outputMethod(colorToOutput);
					currentColor = colorToOutput;

					Delay(sleepTime);

					syncContext?.SignalAndWait();

					if (forceStoppable && cts != null && cts.IsCancellationRequested)
					{
						endingColor = currentColor;
						return;
					}
				}

				//if looping, loop back from 2nd color to 1st color before looping back
				if (loop || reverse)
				{
					Fade(colorEnd, colorStart, speed, sleepTime, false, outputMethod, out endingColor, syncContext, false, cts, forceStoppable);
				}
			}

			endingColor = currentColor;

			if (cts != null && cts.IsCancellationRequested)
				return;
		}

		/// <summary>
		/// Fades a given color to black.
		/// </summary>
		public static void FadeToBlack(Color color, int speed, int sleepTime, bool loop, Action<Color> outputMethod,
			out Color? endingColor, SyncContext syncContext = null, bool reverse = false, CancellationTokenSource cts = null, bool force = false)
		{
			Fade(color, Color.Black, speed, sleepTime, loop, outputMethod, out endingColor, syncContext, reverse, cts, force);
		}

		/// <summary>
		/// Blinks a set of colors holding the color for the given amount of milliseconds.
		/// </summary>
		/// <param name="colorsAndHoldTimes">List of tuples of colors and their hold times</param>
		/// <param name="outputMethod">Method to use to output the blinks.</param>
		/// <param name="syncContext">SyncContext to sync the blinking through.</param>
		/// <param name="tightness">Range: [1, 3]. Determines how tightly the blink is sycned.</param>
		public static void Blink(List<Tuple<Color, int>> colorsAndHoldTimes, Action<Color> outputMethod, SyncContext syncContext = null, int tightness = 1)
		{
			colorsAndHoldTimes.ForEach(tuple =>
			{
				if (tightness < 2)
					syncContext?.SignalAndWait();
				outputMethod(tuple.Item1);
				if (tightness == 2)
					syncContext?.SignalAndWait();
				Delay(tuple.Item2);
				if (tightness >= 3)
					syncContext?.SignalAndWait();
			});
		}
		
		public static void SoftBlink(List<Tuple<Color, int>> colorsAndHoldTimes, Action<Color> outputMethod,
			SyncContext syncContext = null, bool reverse = true, CancellationTokenSource cts = null, bool forceStoppable = true, double brightness = 1)
		{
			var speed = 1;

			colorsAndHoldTimes.ForEach(tuple =>
			{
				Color? endingColor;
				Fade(Color.Black, tuple.Item1.Darken(brightness), speed, tuple.Item2, false, outputMethod, out endingColor, syncContext, reverse, cts, forceStoppable);
				syncContext?.SignalAndWait();
			});
		}

		#endregion

		#region Colors

		public static Color GetRandomColor()
		{
			return ColorScheme.GetRandomColor();
		}

		/// <summary>
		/// Returns a Dictionary from int to Color with all colors set to Black. This dictionary gets all the ints from the zone passed in.
		/// </summary>
		public static Dictionary<int, Color> GetBlackedOutZone(Zone zone)
		{
			var colors = new Dictionary<int, Color>();
			zone.SortedLights.Keys.ToList().ForEach(lightIndex => colors.Add(lightIndex, Color.Black));
			return colors;
		}

		#endregion

		public static int RandomIntBetween(int low, int high)
		{
			return RandomGen.Next(low, high);
		}
	}

	public static class RandomGen
	{
		private static Random _global = new Random();
		[ThreadStatic]
		private static Random _local;

		public static int Next(int maxValue)
		{
			Random inst = _local;
			if (inst == null)
			{
				int seed;
				lock (_global) seed = _global.Next();
				_local = inst = new Random(seed);
			}
			return inst.Next(maxValue);
		}

		public static int Next(int minValue, int maxValue)
		{
			Random inst = _local;
			if (inst == null)
			{
				int seed;
				lock (_global) seed = _global.Next();
				_local = inst = new Random(seed);
			}
			return inst.Next(minValue, maxValue);
		}
	}

	public static class ProgramExtensions
	{
		public static void Send(this IList<ILogicalRGBLight> lights, ILightingController lc)
		{
			lc.SendLights(lights.Cast<ILightingControllerPixel>().ToList());
		}

		/// <summary>
		/// Determines if input is in the between the low and high bound (low and high bounds are included in range by default).
		/// </summary>
		/// <param name="input">Input value</param>
		/// <param name="low">Low bound</param>
		/// <param name="high">High bound</param>
		/// <param name="lowInclusive">Whether or not to include the low bound in calculation. True by default.</param>
		/// <param name="highInclusive">Whether or not to include the high bound in calculation. True by default.</param>
		/// <returns></returns>
		public static bool IsInRange(this double input, double low, double high, bool lowInclusive = true, bool highInclusive = true)
		{
			if (lowInclusive)
			{
				if (highInclusive)
					return input >= low && input <= high;

				return input >= low && input < high;
			}

			if (highInclusive)
				return input > low && input <= high;

			return input > low && input < high;
		}

		public static bool IsInRange(this int input, int low, int high, bool lowInclusive = true,
			bool highInclusive = true)
		{
			return ((double)input).IsInRange(low, high, lowInclusive, highInclusive);
		}

		//public static Color ReduceBrightness(this Color color, int drop)
		//{
		//	return Color.FromArgb(Math.Max(color.R - drop, 0), Math.Max(color.G - drop, 0), Math.Max(color.B - drop, 0));
		//}

		public static Color Darken(this Color color, double darkenAmount)
		{
			var hslColor = new HSLColor(color);
			hslColor.Luminosity *= darkenAmount; // 0 to 1
			return hslColor;
		}

	}

	public static class TupleListExtensions
	{
		public static void Add<T1, T2>(
				this IList<Tuple<T1, T2>> list, T1 item1, T2 item2)
		{
			list.Add(Tuple.Create(item1, item2));
		}

		public static void Add<T1, T2, T3>(
				this IList<Tuple<T1, T2, T3>> list, T1 item1, T2 item2, T3 item3)
		{
			list.Add(Tuple.Create(item1, item2, item3));
		}
	}
}
