using System;
using System.Collections.Generic;

namespace ZoneLighting.Communication
{
	public interface ILightingController
	{
        void SendLights(IList<IPixel> lights);
    }
}