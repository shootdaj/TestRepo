using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Communication;

namespace ZoneLighting.ZoneProgram
{
	public class ProgramCommon
	{
		public static void Delay(int milliseconds)
		{
			if (milliseconds > 0)
				Task.WaitAll(Task.Delay(milliseconds));
		}












		/// <summary>
		/// Fades the entire strip from one set of colors to another set of colors
		/// </summary>
		/// <param name="color1">First set of colors (array of LEDs with length 32)</param>
		/// <param name="color2"></param>
		/// <param name="speed">The higher the speed, the more abruptly the colors will change. Max is 127.</param>
		/// <param name="sleepTime">How long each color set is displayed</param>
		/// <param name="loop">Whether or not to loop forever</param>
		public static void Fade(Color color1, Color color2, int speed, int sleepTime, bool loop, Action<Color> outputMethod, out Color? endingColor)
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
				}

				//if looping, loop back from 2nd color to 1st color before looping back
				if (loop)
				{
					Fade(color2, color1, speed, sleepTime, false, outputMethod, out endingColor);
				}
			}

			endingColor = currentColor;
		}
	}

	public static class ProgramExtensions
	{
		public static void SetColor(this IList<ILogicalRGBLight> lights, Color color)
		{
			lights.ToList().ForEach(x => x.SetColor(color));
		}

		public static void Send(this IList<ILogicalRGBLight> lights, LightingController lc)
		{
			lc.SendLights(lights);
		}
	}
}
