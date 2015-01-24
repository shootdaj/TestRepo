using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ZoneProgramNS
{
	public class ProgramCommon
	{
		/// <summary>
		/// Delays the execution for the specified amount of milliseconds.
		/// </summary>
		/// <param name="milliseconds"></param>
		public static void Delay(int milliseconds)
		{
			if (milliseconds > 0)
				Task.WaitAll(Task.Delay(milliseconds));
		}

		/// <summary>
		/// Fades from one color to another color.
		/// </summary>
		/// <param name="color1">First set of colors (array of LEDs with length 32)</param>
		/// <param name="color2"></param>
		/// <param name="speed">The higher the speed, the more abruptly the colors will change. Max is 127.</param>
		/// <param name="sleepTime">How long each color set is displayed</param>
		/// <param name="loop">Whether or not to loop forever</param>
		public static void Fade(Color color1, Color color2, int speed, int sleepTime, bool loop, Action<Color> outputMethod, out Color? endingColor, SyncContext syncContext = null, ZoneProgram zoneProgram = null)
		{
			if (speed > 127)
				throw new Exception("Speed cannot exceed 127.");

			bool firstLoop = true;
			Color? currentColor = null;
			
			while (firstLoop || loop)
			{
				firstLoop = false;

				float redDiff = (color2.R - color1.R);
				float greenDiff = (color2.G - color1.G);
				float blueDiff = (color2.B - color1.B);

				float redJump = redDiff / (128 - speed);
				float greenJump = greenDiff / (128 - speed);
				float blueJump = blueDiff / (128 - speed);

				float redLevel = color1.R;
				float greenLevel = color1.G;
				float blueLevel = color1.B;

				//fade
				for (int a = 0; a < (128 - speed); a++)
				{
					redLevel += redJump;
					greenLevel += greenJump;
					blueLevel += blueJump;

					Color colorToOutput = Color.FromArgb(255, (int)redLevel, (int)greenLevel, (int)blueLevel);
					outputMethod(colorToOutput);
					currentColor = colorToOutput;

					Delay(sleepTime);

					syncContext?.SignalAndWait();
				}

				//if looping, loop back from 2nd color to 1st color before looping back
				if (loop)
				{
					Fade(color2, color1, speed, sleepTime, false, outputMethod, out endingColor);
				}
			}

			endingColor = currentColor;
		}

		/// <summary>
		/// Blinks a set of colors holding the color for the given amount of milliseconds.
		/// </summary>
		/// <param name="colorsAndHoldTimes">List of tuples of colors and their hold times</param>
		/// <param name="outputMethod">Method to use to output the blinks</param>
		public static void Blink(List<Tuple<Color, int>> colorsAndHoldTimes, Action<Color> outputMethod, SyncContext syncContext = null, ZoneProgram zoneProgram = null)
		{
			colorsAndHoldTimes.ForEach(tuple =>
			{
				syncContext?.SignalAndWait();
				outputMethod(tuple.Item1);
				Delay(tuple.Item2);
			});
		}
	}

	public static class ProgramExtensions
	{
		

		public static void Send(this IList<ILogicalRGBLight> lights, LightingController lc)
		{
			lc.SendLights(lights.Cast<ILightingControllerPixel>().ToList());
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
