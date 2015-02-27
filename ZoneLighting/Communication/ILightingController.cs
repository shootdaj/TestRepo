using System;
using System.Collections.Generic;

namespace ZoneLighting.Communication
{
	public interface ILightingController
	{
		void SendPixelFrame(IPixelFrame opcPixelFrame);
		void SendLEDs(IList<ILightingControllerPixel> leds);
		Type PixelType { get; }

		/// <summary>
		/// Sends a list of Lights to the implementing lighting controller
		/// </summary>
		void SendLights(IList<ILightingControllerPixel> lights);
	}
}