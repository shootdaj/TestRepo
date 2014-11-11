using System;
using System.Collections.Generic;

namespace ZoneLighting.Communication
{
	public interface ILightingController
	{
		void SendPixelFrame(IPixelFrame opcPixelFrame);
		void SendLEDs(IList<LED> leds);
		
	}
}