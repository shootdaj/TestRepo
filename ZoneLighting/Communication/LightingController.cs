using System;
using System.Collections.Generic;
using System.Linq;

namespace ZoneLighting.Communication
{
	public abstract class LightingController : ILightingController, IDisposable
	{
		#region ILightingController

		public abstract void SendPixelFrame(IPixelFrame opcPixelFrame);
		public abstract void SendLEDs(IList<LED> leds);

		#endregion

		#region IInitializable/IDisposable

		//public abstract void Initialize();
		//public abstract bool Initialized { get; private set; }
		//public abstract void Uninitialize();
		public abstract void Dispose();

		#endregion

		#region Helper Methods

		/// <summary>
		/// Sends a list of Lights to the implementing lighting controller
		/// </summary>
		public void SendLights(IList<ILogicalRGBLight> lights)
		{
			SendLEDs(lights.Cast<LED>().ToList());
		}

		#endregion
	}
}
