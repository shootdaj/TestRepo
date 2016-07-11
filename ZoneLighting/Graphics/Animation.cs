using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using ZoneLighting.ZoneNS;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting;

namespace ZoneLighting.Graphics
{
	public class Animation
	{
		public class Fader
		{
			public Func<bool> GetForceStopFlag { get; set; }

			public Fader()
			{
				
			}

			public void MakeForceStoppable(Func<bool> getForceStopFlag)
			{
				GetForceStopFlag = getForceStopFlag;
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
			/// <param name="cts">CancellationTokenSource to cancel the method call. If ForceStopFlag is set to true, it will stop immediately. 
			/// If not, it will stop after the current fade is finished.</param>
			public void Fade(Color colorStart, Color colorEnd, int speed, int sleepTime, bool loop,
				Action<Color> outputMethod, out Color? endingColor, SyncContext syncContext = null, bool reverse = false,
				CancellationTokenSource cts = null)
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

						ProgramCommon.Delay(sleepTime);

						syncContext?.SignalAndWait();
							
						if (GetForceStopFlag != null && GetForceStopFlag() && cts != null && cts.IsCancellationRequested)
						{
							endingColor = currentColor;
							return;
						}
					}

					//if looping, loop back from 2nd color to 1st color before looping back
					if (loop || reverse)
					{
						FadeStatic(colorEnd, colorStart, speed, sleepTime, false, outputMethod, out endingColor, syncContext, false, cts);
					}
				}

				endingColor = currentColor;

				if (cts != null && cts.IsCancellationRequested)
					return;
			}

			public void FadeToBlack(Color color, int speed, int sleepTime, bool loop, Action<Color> outputMethod,
				out Color? endingColor, SyncContext syncContext = null, bool reverse = false, CancellationTokenSource cts = null)
			{
				FadeStatic(color, Color.Black, speed, sleepTime, loop, outputMethod, out endingColor, syncContext, reverse, cts);
			}

			public static void FadeStatic(Color colorStart, Color colorEnd, int speed, int sleepTime, bool loop, Action<Color> outputMethod, out Color? endingColor, SyncContext syncContext = null, bool reverse = false, CancellationTokenSource cts = null)
			{
				var fader = new Fader();
				fader.Fade(colorStart, colorEnd, speed, sleepTime, loop, outputMethod, out endingColor, syncContext, reverse, cts);
			}

			/// <summary>
			/// Fades a given color to black.
			/// </summary>
			public static void FadeToBlackStatic(Color color, int speed, int sleepTime, bool loop, Action<Color> outputMethod,
				out Color? endingColor, SyncContext syncContext = null, bool reverse = false, CancellationTokenSource cts = null)
			{
				var fader = new Fader();
				fader.FadeToBlack(color, speed, sleepTime, loop, outputMethod, out endingColor, syncContext, reverse, cts);
			}
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
				if (syncContext != null && tightness < 2)
					syncContext.SignalAndWait();
				outputMethod(tuple.Item1);
				if (syncContext != null && tightness == 2)
					syncContext.SignalAndWait();
				ProgramCommon.Delay(tuple.Item2);
				if (syncContext != null && tightness >= 3)
					syncContext.SignalAndWait();
			});
		}

		public static void Blink(Color color, int ms, Action<Color> outputMethod)
		{
			var colorsAndHoldTimes = new List<Tuple<Color, int>>(); 
			colorsAndHoldTimes.Add(new Tuple<Color, int>(color, ms));
			colorsAndHoldTimes.Add(new Tuple<Color, int>(Color.Black, 1));
			Blink(colorsAndHoldTimes, outputMethod);
		}

		public static void SoftBlink(List<Tuple<Color, int>> colorsAndHoldTimes, Action<Color> outputMethod,
			SyncContext syncContext = null, bool reverse = true, CancellationTokenSource cts = null, double brightness = 1)
		{
			var speed = 1;

			colorsAndHoldTimes.ForEach(tuple =>
			{
				Color? endingColor;
				Fader.FadeStatic(Color.Black, tuple.Item1.Darken(brightness), speed, tuple.Item2, false, outputMethod, out endingColor, syncContext, reverse, cts);
				syncContext?.SignalAndWait();
			});
		}

		public static void SoftBlink(Color color, int fadeSleepTime, Action<Color> outputMethod)
		{
			var colorsAndHoldTimes = new List<Tuple<Color, int>>();
			colorsAndHoldTimes.Add(new Tuple<Color, int>(color, fadeSleepTime));
			SoftBlink(colorsAndHoldTimes, outputMethod, reverse: true);
		}
	}
}
