using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

		public static Color GetRandomPrimarySchemeColor()
		{
			return GetRandomSchemeColor(Primaries);
		}

		public static Color GetRandomSchemeColor(ColorScheme colorScheme)
		{
			return colorScheme == All ? GetRandomColor() : colorScheme.ElementAt(ProgramCommon.RandomIntBetween(0, colorScheme.Count));
		}

		public static Color GetRandomColor()
		{
			return Color.FromArgb(ProgramCommon.RandomIntBetween(0, 255), ProgramCommon.RandomIntBetween(0, 255), ProgramCommon.RandomIntBetween(0, 255));
		}
	}
}