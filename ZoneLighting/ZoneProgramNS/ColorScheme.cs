using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ZoneLighting.ZoneProgramNS
{
	public class ColorScheme : List<Color>
	{
		public static ColorScheme All => new ColorScheme();

		public static ColorScheme Primaries => new ColorScheme()
		{
			Color.Red,
			Color.Green,
			Color.Blue,
		};

		public static ColorScheme Secondaries => new ColorScheme()
		{
			Color.Red,
			Color.Green,
			Color.Blue,
			Color.Yellow,
			Color.Magenta,
			Color.Cyan,
		};

		public static ColorScheme Reds => new ColorScheme()
		{
			Color.Red,
			Color.DarkRed,
			Color.IndianRed,
			Color.MediumVioletRed,
			Color.OrangeRed,
			Color.PaleVioletRed,
			Color.PaleVioletRed,
			Color.Crimson,
			Color.DeepPink,
			Color.HotPink,
			Color.Coral,
			Color.Firebrick,
		};

		public static ColorScheme RedsBluesGreens
		{
			get
			{
				var colorScheme = new ColorScheme();
				colorScheme.AddRange(Reds);
				colorScheme.AddRange(Blues);
				colorScheme.AddRange(Greens);

				return colorScheme;
			}
		}

		public static ColorScheme Blues => new ColorScheme()
		{
			Color.Blue,
			Color.Aqua,
			Color.DarkBlue,
			Color.CadetBlue,
			Color.CornflowerBlue,
			Color.Cyan,
			Color.DarkCyan,
			Color.DarkSlateBlue,
			Color.DodgerBlue,
			Color.SkyBlue,
			Color.DeepSkyBlue,
			Color.Teal,
			Color.MediumBlue,
			Color.DarkTurquoise,
			Color.Navy,
			Color.MidnightBlue
		};

		public static ColorScheme Greens=> new ColorScheme()
		{
			Color.Green,
			Color.GreenYellow,
			Color.DarkGreen,
			Color.DarkOliveGreen,
			Color.Olive,
			Color.DarkSeaGreen,
			Color.ForestGreen,
			Color.LawnGreen,
			Color.LightGreen,
			Color.LightSeaGreen,
			Color.LimeGreen,
			Color.MediumSeaGreen,
			Color.MediumSpringGreen,
			Color.PaleGreen,
			Color.Chartreuse
		};

		public static Color GetRandomPrimarySchemeColor()
		{
			return GetRandomSchemeColor(Primaries);
		}

		public static Color GetRandomSchemeColor(ColorScheme colorScheme)
		{
			if (colorScheme == null)
				return GetRandomColor();
			if (!colorScheme.Any())
				return GetRandomColor();
			else
				return colorScheme.ElementAt(ProgramCommon.RandomIntBetween(0, colorScheme.Count));
		}

		public static Color GetRandomColor()
		{
			return Color.FromArgb(ProgramCommon.RandomIntBetween(0, 255), ProgramCommon.RandomIntBetween(0, 255), ProgramCommon.RandomIntBetween(0, 255));
		}
	}
}