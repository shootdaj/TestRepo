﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ZoneLighting.MEF;

namespace ZoneLighting.ZoneNS
{
	public static class ZoneExtensions
	{
		public static void SetColor(this IList<IPixel> lights, Color color)//, double brightness)
		{
			//var brightnessAdjustedColor = Color.FromArgb((int) (color.R*brightness), (int) (color.G*brightness),
			//	(int) (color.B*brightness));

			lights.ToList().ForEach(x => x.Color = color);
		}

		///// <summary>
		///// Fluent method that synchronizes the given target with the source zone.
		///// </summary>
		///// <param name="syncSource"></param>
		///// <param name="syncTarget"></param>
		///// <returns></returns>
		//public static Zone Synchronize(this Zone syncSource, Zone syncTarget)
		//{
		//	ZoneLightingManager.Instance.Synchronize(syncSource, syncTarget);
		//	return syncSource;
		//}

		
	}
}
