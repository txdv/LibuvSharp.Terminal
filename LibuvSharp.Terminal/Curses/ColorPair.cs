using System;
using System.Collections.Generic;

namespace LibuvSharp.Terminal
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

		public static ColorPair From(int foreground, int background)
		{
			if (foreground < -1 || foreground >= Curses.Colors) {
				throw new ArgumentException("foreground");
			} else if (background < -1 || background >= Curses.Colors) {
				throw new ArgumentException("background");
			}

			unchecked {
				return From((ushort)foreground, (ushort)background);
			}
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
	}
}

