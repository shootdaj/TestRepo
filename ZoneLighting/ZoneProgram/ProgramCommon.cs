using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneLighting.Communication;

namespace ZoneLighting.ZoneProgram
{
	public class ProgramCommon
	{
		public static void Delay(int milliseconds)
		{
			if (milliseconds > 0)
				Task.WaitAll(Task.Delay(milliseconds));
		}
	}

	public static class ProgramExtensions
	{
		public static void SetColor(this IList<ILogicalRGBLight> lights, Color color)
		{
			lights.ToList().ForEach(x => x.SetColor(color));
		}

		public static void Send(this IList<ILogicalRGBLight> lights, ILightingController lc)
		{
			lc.SendLights(lights);
		}
	}
}
