using System.Collections.Generic;
using System.Drawing;
using ZoneLighting.Graphics.Drawing;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Clock;

namespace ZoneLighting.StockPrograms
{
	public class VisualClock : ReactiveZoneProgram
	{
		private Color CircleColor { get; set; } = Color.Red;

		private int CurrentSecondPosition { get; set; }

		private Shape Shape { get; set; }

		private TimerClock Clock { get; set; } 

		protected override void StartCore(dynamic parameters = null)
		{
			Shape = new Shape(3, 4, 10, 13, 17, 22, 24, 31, 32, 39, 41, 46, 50, 53, 59, 60);

			Clock = new TimerClock(1000, args =>
			{
				OutputCircle(CircleColor);

				var sendColors = new Dictionary<int, Color>();
				sendColors.Add(Shape.Pixels[CurrentSecondPosition], Color.Blue);
				SendColors(sendColors);
				CurrentSecondPosition++;
				if (CurrentSecondPosition == Shape.PixelCount)
				{
					CurrentSecondPosition = 0;
				}
			});

			Clock.Start();

			base.StartCore(null);
		}

		protected override void StopCore(bool force)
		{
			Clock.Stop();
			base.StopCore(force);
		}

		private void OutputCircle(Color color)
		{
			var colorsToSend = new Dictionary<int, Color>();

			for (int i = 0; i < Shape.PixelCount; i++)
			{
				colorsToSend.Add(Shape.Pixels[i], color);
			}

			//colorsToSend.Add(3, color);
			//colorsToSend.Add(4, color);
			//colorsToSend.Add(10, color);
			//colorsToSend.Add(13, color);
			//colorsToSend.Add(17, color);
			//colorsToSend.Add(22, color);
			//colorsToSend.Add(24, color);
			//colorsToSend.Add(31, color);
			//colorsToSend.Add(32, color);
			//colorsToSend.Add(39, color);
			//colorsToSend.Add(41, color);
			//colorsToSend.Add(46, color);
			//colorsToSend.Add(50, color);
			//colorsToSend.Add(53, color);
			//colorsToSend.Add(59, color);
			//colorsToSend.Add(60, color);
			SendColors(colorsToSend);
		}
	}
}
