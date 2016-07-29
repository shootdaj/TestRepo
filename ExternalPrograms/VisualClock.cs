using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using ZoneLighting.ZoneProgramNS;
using ZoneLighting.ZoneProgramNS.Clock;

namespace ExternalPrograms
{
    [Export(typeof(ZoneProgram))]
    [ExportMetadata("Name", "VisualClock")]
    public class VisualClock : ReactiveZoneProgram
	{
		private Color BackgroundColor { get; set; } = Color.Black.Darken(.5);

        private Color SecHandColor { get; set; } = Color.Green.Darken(.3);

        private Color MinHandColor { get; set; } = Color.Blue.Darken(.3);

        private Color HourHandColor { get; set; } = Color.Orange.Darken(.2);

        private int CurrentSecondPosition { get; set; }

        private int CurrentMinutePosition { get; set; }

        private int CurrentHourPosition { get; set; }

        private ZoneLighting.Graphics.Drawing.Shape Shape { get; set; } 

		private TimerClock Clock { get; set; } 

		protected override void StartCore(dynamic parameters = null)
		{

            //Shape = new Shape(3, 4, 13, 22, 31, 39, 46, 53, 60, 59, 50, 41, 32, 24, 17, 10);

		    var currentTime = DateTime.Now;

		    var hour = currentTime.Hour;
		    if (hour > 11)
		    {
		        hour = hour - 12;
		    }
		    var min = currentTime.Minute;
		    var sec = currentTime.Second;

		    CurrentSecondPosition = sec;
		    CurrentMinutePosition = min;
            CurrentHourPosition = (hour*60 + min)/12;

            Clock = new TimerClock(1000, args =>
			{
                var sendColors = new Dictionary<int, Color>();

			    for (var i = 0; i < 60; i++)
			    {
			        if (i == CurrentHourPosition)
			        {
                        sendColors.Add(CurrentHourPosition, HourHandColor);
                    }
                    else if (i == CurrentMinutePosition)
                    {
                        sendColors.Add(CurrentMinutePosition, MinHandColor);
                    }
                    else if (i == CurrentSecondPosition)
			        {
			            sendColors.Add(CurrentSecondPosition, SecHandColor);
			        }
			        else
			        {
                        sendColors.Add(i, BackgroundColor);
                    }
			    }
               
                SendColors(sendColors);
				CurrentSecondPosition++;
			    if (CurrentSecondPosition == 60)
			    {
                    CurrentSecondPosition = 0;

                    currentTime = DateTime.Now;

                    hour = currentTime.Hour;
                    if (hour > 11)
                    {
                        hour = hour - 12;
                    }
                    min = currentTime.Minute;

                    CurrentMinutePosition = min;
                    CurrentHourPosition = (hour * 60 + min) / 12;
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

			SendColors(colorsToSend);
		}
	}
}
