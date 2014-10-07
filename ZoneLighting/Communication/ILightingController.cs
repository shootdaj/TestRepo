using System;

namespace ZoneLighting.Communication
{
	public interface ILightingController : IInitializable, IDisposable
	{
		/// <summary>
		/// Sends a Pixel Frame to the connected lighting controller.
		/// </summary>
		/// <param name="opcPixelFrame"></param>
		void SendPixelFrame(OPCPixelFrame opcPixelFrame);
	}
}