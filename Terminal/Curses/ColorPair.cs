using System;
using System.Drawing;
using System.Collections.Generic;

namespace Terminal
{
	public class ColorPair
	{
		public static ushort Count { get; protected set; }

		public int Index { get; protected set; }

		public int Attribute {
			get {
				return Curses.COLOR_PAIR(Index);
			}
		}

		public ColorPair(ushort foreground, ushort background)
			: this(++Count, foreground, background)
		{
		}

		public ColorPair(ushort index, ushort foreground, ushort background)
		{
			Foreground = foreground;
			Background = background;

			Index = index;
			Curses.init_pair(index, foreground, background);
		}

		private static ColorPair[,] colors = new ColorPair[257, 257];

		private static int Accumulate(int colorPair, params int[] attributes)
		{
			foreach (var attribute in attributes) {
				colorPair += attribute;
			}
			return colorPair;
		}

		public static ColorPair From(ushort foreground, ushort background)
		{
			short fg = (short)(foreground + 1);
			short bg = (short)(background + 1);

			var col = colors[fg, bg];
			if (col == null) {
				col = new ColorPair(foreground, background);
				colors[fg, bg] = col;
			}
			return col;
		}
		public static int From(ushort foreground, ushort background, params int[] attributes)
		{
			return Accumulate(ColorPair.From(foreground, background).Attribute, attributes);
		}

		private static void CheckColor(int color, string name)
		{
			if (color < -1 || color >= Curses.Colors) {
				throw new ArgumentException("foreground");
			}
		}

		public static ColorPair From(int foreground, int background)
		{
			CheckColor(foreground, "foreground");
			CheckColor(foreground, "background");

			unchecked {
				return From((ushort)foreground, (ushort)background);
			}
		}
		public static int From(int foreground, int background, params int[] attributes)
		{
			return Accumulate(ColorPair.From(foreground, background).Attribute, attributes);
		}

		public static ColorPair From(Color foreground, Color background)
		{
			return ColorPair.From(ConvertBasic(foreground), ConvertBasic(background));
		}

		public static ColorPair From(Color foreground)
		{
			return From(foreground, Color.Transparent);
		}

		public static int From(Color foreground, params int[] attributes)
		{
			return From(foreground, Color.Transparent, attributes);
		}

		public static int From(Color foreground, Color background, params int[] attributes)
		{
			return Accumulate(ColorPair.From(foreground, background).Attribute, attributes);
		}

		public ushort Foreground { get; set; }
		public ushort Background { get; set; }
		public short Pair { get; set; }

		public bool ColorEquals(ColorPair cp)
		{
			return (cp.Foreground == Foreground) && (cp.Background == Background);
		}

		public bool PairEquals(ColorPair cp)
		{
			return (Pair == cp.Pair) && ColorEquals(cp);
		}

		public static void ReleaseAll()
		{
			colors = new ColorPair[257, 257];
			Count = 0;
		}

		public override string ToString()
		{
			return string.Format ("\x0000{0},{1} ", Foreground, Background);
		}

		static Dictionary<Color, int> colorsBasic = new Dictionary<Color, int>() {
			{ Color.Transparent,  -1 },
			{ Color.Black,         0 },
			{ Color.Red,           1 },
			{ Color.Green,         2 },
			{ Color.Yellow,        3 },
			{ Color.Blue,          4 },
		};

		public static int ConvertBasic(Color color)
		{
			int value;
			if (!colorsBasic.TryGetValue(color, out value)) {
				throw new ArgumentException("Can't convert to ncurses color", "color");
			}
			return value;
		}
	}
}

