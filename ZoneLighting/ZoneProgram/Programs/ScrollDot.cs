using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ZoneLighting.Communication;

namespace ZoneLighting.ZoneProgram.Programs
{
	/// <summary>
	/// Scrolls a dot across the entire length of Lights
	/// </summary>
	public class ScrollDot : LoopingZoneProgram
	{
		public override void Loop(IZoneProgramParameter parameter)
		{
			var scrollDotParameter = (ScrollDotParameter)parameter;
			
			var colors = new List<Color>();
			colors.Add(Color.Red);
			colors.Add(Color.Blue);
			colors.Add(Color.Yellow);
			colors.Add(Color.Green);
			colors.Add(Color.Purple);
			colors.Add(Color.RoyalBlue);
			colors.Add(Color.MediumSeaGreen);

			for (int i = 0; i < 6; i++)
			{
				Lights.ToList().ForEach(x => x.SetColor(Color.FromArgb(0, 0, 0))); //set all lights to black
				Lights[i].SetColor(colors[new Random().Next(0, 6)]);		//set one to white
				LightingController.SendLEDs(Lights.Cast<LED>().ToList());	//send frame
				Task.WaitAll(Task.Delay(scrollDotParameter.DelayTime));		//pause before next iteration
			}
		}
	}

	public class ScrollDotParameter : IZoneProgramParameter
	{
		public ScrollDotParameter(int delayTime)
		{
			DelayTime = delayTime;
		}
		public int DelayTime { get; set; }
	}
}
