using System;
using System.Collections.Generic;

namespace ZoneLighting.Communication
{
	public interface ILightingController : IInitializable, IDisposable
	{
		void SendPixelFrame(IPixelFrame opcPixelFrame);
		void SendLEDs(IList<LED> leds);
		void SendLights(IList<ILogicalRGBLight> lights);
	}
}