using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using ZoneLighting.Communication;
using ZoneLighting.ZoneNS;

namespace ZoneLighting.ZoneProgramNS
{
	public class ProgramCommon
	{
		#region Non-Trivial

		/// <summary>
		/// Delays the execution for the specified amount of milliseconds.
		/// </summary>
		/// <param name="milliseconds"></param>
		public static void Delay(int milliseconds)
		{
			if (milliseconds > 0)
				Thread.Sleep(milliseconds);
		}

		#endregion

		#region Colors

		public static Color GetRandomColor()
		{
			return ColorScheme.GetRandomColor();
		}

		/// <summary>
		/// Returns a Dictionary from int to Color with all colors set to Black. This dictionary gets all the ints from the zone passed in.
		/// </summary>
		public static Dictionary<int, Color> GetBlackedOutZone(Zone zone)
		{
			var colors = new Dictionary<int, Color>();
			zone.SortedLights.Keys.ToList().ForEach(lightIndex => colors.Add(lightIndex, Color.Black));
			return colors;
		}

		#endregion

		public static int RandomIntBetween(int low, int high)
		{
			return RandomGen.Next(low, high);
		}
	}

    /// <summary>
    /// Use this class for Random generation. This is to avoid cross-thread issues with Random.
    /// </summary>
	public static class RandomGen
	{
		private static Random _global = new Random();

		[ThreadStatic]
		private static Random _local;

		public static int Next(int maxValue)
		{
			Random inst = _local;
			if (inst == null)
			{
				int seed;
				lock (_global)
                    seed = _global.Next();
				_local = inst = new Random(seed);
			}
			return inst.Next(maxValue);
		}

		public static int Next(int minValue, int maxValue)
		{
			Random inst = _local;
			if (inst == null)
			{
				int seed;
				lock (_global)
                    seed = _global.Next();
				_local = inst = new Random(seed);
			}
			return inst.Next(minValue, maxValue);
		}
	}

	public static class ProgramExtensions
	{
		public static void Send(this IList<IPixel> lights, ILightingController lc)
		{
			lc.SendLights(lights.ToList());
		}

		/// <summary>
		/// Determines if input is in the between the low and high bound (low and high bounds are included in range by default).
		/// </summary>
		/// <param name="input">Input value</param>
		/// <param name="low">Low bound</param>
		/// <param name="high">High bound</param>
		/// <param name="lowInclusive">Whether or not to include the low bound in calculation. True by default.</param>
		/// <param name="highInclusive">Whether or not to include the high bound in calculation. True by default.</param>
		/// <returns></returns>
		public static bool IsInRange(this double input, double low, double high, bool lowInclusive = true, bool highInclusive = true)
		{
			if (lowInclusive)
			{
				if (highInclusive)
					return input >= low && input <= high;

				return input >= low && input < high;
			}

			if (highInclusive)
				return input > low && input <= high;

			return input > low && input < high;
		}

		public static bool IsInRange(this int input, int low, int high, bool lowInclusive = true,
			bool highInclusive = true)
		{
			return ((double)input).IsInRange(low, high, lowInclusive, highInclusive);
		}

		//public static Color ReduceBrightness(this Color color, int drop)
		//{
		//	return Color.FromArgb(Math.Max(color.R - drop, 0), Math.Max(color.G - drop, 0), Math.Max(color.B - drop, 0));
		//}

		public static Color Darken(this Color color, double darkenAmount)
		{
			var hslColor = new HSLColor(color);
			hslColor.Luminosity *= darkenAmount; // 0 to 1
			return hslColor;
		}

	}

	public static class TupleListExtensions
	{
		public static void Add<T1, T2>(
				this IList<Tuple<T1, T2>> list, T1 item1, T2 item2)
		{
			list.Add(Tuple.Create(item1, item2));
		}

		public static void Add<T1, T2, T3>(
				this IList<Tuple<T1, T2, T3>> list, T1 item1, T2 item2, T3 item3)
		{
			list.Add(Tuple.Create(item1, item2, item3));
		}
	}
}
