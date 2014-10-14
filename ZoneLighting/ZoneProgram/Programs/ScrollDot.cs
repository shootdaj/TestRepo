using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
				Lights.Values.ToList().ForEach(x => x.SetColor(Color.FromArgb(0, 0, 0))); //set all lights to black
				Lights[i].SetColor(colors[new Random().Next(0, 7)]); //set one to white

				//TODO: This is where the mapping provider would map the Lights collection to the byte order of the data in the OPCPixelFrame
				//send frame 
				LightingController.SendPixelFrame(OPCPixelFrame.CreateFromLEDCollection(0, Lights.Values.Cast<LED>().ToList()));
				Task.WaitAll(Task.Delay(scrollDotParameter.DelayTime));
			}
		}
	}

	public class ScrollDotParameter : IZoneProgramParameter
	{
		public int DelayTime { get; set; }
	}
}
