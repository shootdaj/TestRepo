using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Communication;

namespace ZoneLighting
{
	/// <summary>
	/// Represents a single frame of lighting - i.e., a set of Lights in a specific arrangement.
	/// </summary>
	public class LightingFrame
	{
		#region CORE

		public IList<ILogicalRGBLight> Lights { get; private set; }

		public int NumberOfLights
		{
			get { return Lights.Count; }
		}

		#endregion

		#region API

		///// <summary>
		///// Sends this Pixel Frame instance using the given Lighting Controller.
		///// </summary>
		///// <param name="controller"></param>
		//public void Send(ILightingController controller)
		//{
		//	controller.SendPixelFrame(this);
		//}

		public static LightingFrame CreateFromRGBByteArray<T>(byte[] array) where T : ILogicalRGBLight
		{
			if (typeof(T) == typeof(LED))
			{
				var lightingFrame = new LightingFrame();

				for (int i = 0; i < array.Length; i+=3)
				{
					Color color = Color.FromArgb(array[i], array[i + 1], array[i + 2]);
					LED led = new LED(color);	
					lightingFrame.Lights.Add(led);
				}

				return lightingFrame;
			}
			else
			{
				return null;
			}
		}

		#endregion
	}
}
