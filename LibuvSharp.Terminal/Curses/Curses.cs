using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LibuvSharp.Terminal
{
	unsafe public class Curses
	{
		internal static DynamicLibrary Module { get; set; }

		internal static void Init()
		{
			Module = DynamicLibrary.Open();
			IntPtr ptr;
			Module.TryGetSymbol("COLORS", out ptr);
			colors = (int *)ptr.ToPointer();

			Module.TryGetSymbol("COLOR_PAIRS", out ptr);
			color_pairs = (int *)ptr.ToPointer();

			Terminal.Init();
		}

		internal static int *colors;
		public static int Colors {
			get {
				return *colors;
			}
		}

		internal static int *color_pairs;
		public static int ColorPairs {
			get {
				return *color_pairs;
			}
		}

		public const int Error = unchecked((int)0xffffffff);

		public static class Cursor
		{
			public static int Move(int x, int y)
			{
				return Window.Standard.Cursor.Move(x, y);
			}

			public static int X {
				get {
					return Window.Standard.Cursor.X;
				}
			}

			public static int Y {
				get {
					return Window.Standard.Cursor.Y;
				}
			}
		}

		public static class Key
		{
			class Range
			{
				public int Key { get; protected set; }
				public int Start { get; protected set; }
				public int End { get; protected set; }

				public Range(int key, int start, int end)
				{
					Key = key;
					Start = start;
					End = end;
				}

				public bool Fits(int key)
				{
					return (Start <= key) && (key <= End);
				}

				public bool Exclusive(Range r)
				{
					if (r.Start < Start) {
						return r.End < r.Start;
					} else if (r.End >= End) {
						return r.Start >= End;
					} else {
						return !(r.Start > Start && r.End < End);
					}
				}
			}

			public const int Alt = 0x2000;

			public static bool IsAlt(int key)
			{
				return (key & Alt) > 0;
			}

			public static int KeyAlt(int key)
			{
				return key & ~Alt;
			}

			public static int Escape(int key)
			{
				return key & (~Alt);
			}

			public static bool IsNumber(int key)
			{
				key = Escape(key);
				return true;
			}

			static Range Get(int key)
			{
				foreach (var range in ranges) {
					if (range.Fits(key)) {
						return range;
					}
				}
				return null;
			}

			public static bool Is(int key, int @base)
			{
				return Get(key) != null;
			}

			public static int Base(int key)
			{
				var range = Get(key);
				if (range != null) {
					return key - range.Start;
				} else {
					return -1;
				}
			}

			public const int Backspace = 263;

			public const int Up       = unchecked((int)0x103);
			public const int Down     = unchecked((int)0x102);
			public const int Left     = unchecked((int)0x104);
			public const int Right    = unchecked((int)0x105);
			public const int NextPage = unchecked((int)0x152);
			public const int PrevPage = unchecked((int)0x153);
			public const int Home     = unchecked((int)0x106);
			public const int Mouse    = unchecked((int)0x199);
			public const int End      = unchecked((int)0x168);
			public const int Delete   = unchecked((int)0x14a);
			public const int Insert   = unchecked((int)0x14b);

			public const int Resize   = unchecked((int)0x19a);

			public const int MaxKeys  = 511;

			public static string Name(int key)
			{
				return new string(keyname(key));
			}

			[DllImport("ncursesw")]
			internal static extern sbyte *keyname(int key);

			static int maxKeys = MaxKeys;

			static List<Range> ranges = new List<Range>();

			public static void Register(int key)
			{
				Register(key, 0);
			}

			public static void Register(int key, int start)
			{
				Register(key, start, start + 255);
			}

			static void Register(int key, int start, int end)
			{
				ranges.Add(new Range(key, start + maxKeys, end + maxKeys));

				char[] str = new char [] { (char)0, (char)0 };
				str[0] = (char)key;
				for (int i = start; i < end; i++) {
					str[1] = (char)i;
					define_key(new string(str), maxKeys);
					maxKeys++;
				}
			}

			static public bool IsEscape(int key)
			{
				return key > MaxKeys;
			}

			static public int EscapeKey(int key, out int normalKey)
			{
				foreach (var range in ranges) {
					if (range.Fits(key)) {
						normalKey = 0;
						return range.Key;
					}
				}
				normalKey = 0;
				return -1;
			}

			[DllImport("ncursesw")]
			internal static extern int define_key(string ch, int map);
		}

		public static class Terminal
		{
			public static void Init()
			{
				IntPtr ptr;
				Module.TryGetSymbol("LINES", out ptr);
				height = (int *)ptr.ToPointer();

				Module.TryGetSymbol("COLS", out ptr);
				width = (int *)ptr.ToPointer();

				Module.TryGetSymbol("ospeed", out ptr);
				speed = (short *)ptr.ToPointer();

				Module.TryGetSymbol("ttytype", out ptr);
				ttytype = (sbyte *)ptr.ToPointer();
			}

			internal static int *width;
			public static int Width {
				get {
					return *width;
				}
			}

			internal static int *height;
			public static int Height {
				get {
					return *height;
				}
			}

			internal static short *speed;
			public static short Speed {
				get {
					return *speed;
				}
			}

			internal static sbyte *ttytype;
			public static string Type {
				get {
					return new string(ttytype);
				}
			}
				/*
			double factor = 3.92156862745098;
			double rfactor = 0.255;
				SetColor(ccolor, (short)(color.R * factor),
					             (short)(color.G * factor),
					             (short)(color.B * factor));
				*/

			public static bool CanChangeColor {
				get {
					return Curses.can_change_color();
				}
			}

			public static void SetColor(ushort color, short r, short g, short b)
			{
				Curses.init_color(color, r, g, b);
			}

			public static void GetColor(ushort color, out short r, out short g, out short b)
			{
				Curses.color_content(color, out r, out g, out b);
			}

			public static void SetColor(ushort ccolor, Curses.Color color)
			{
				SetColor(ccolor, color.R, color.G, color.B);
			}

			public static Curses.Color GetColor(ushort ccolor)
			{
				Curses.Color c;
				GetColor(ccolor, out c.R, out c.G, out c.B);
				return c;
			}

			public static IDictionary<ushort, Tuple<short, short, short>> GetColors()
			{
				Dictionary<ushort, Tuple<short, short, short>> dict = new Dictionary<ushort, Tuple<short, short, short>>();
				for (ushort color = 0; color <= ushort.MaxValue; color++) {
					short r, g, b;
					GetColor(color, out r, out g, out b);
					dict[color] = Tuple.Create(r, g, b);
				}
				return dict;
			}

			public static void SetColors(IDictionary<ushort, Tuple<short, short, short>> colors)
			{
				for (ushort color = 0; color <= ushort.MaxValue; color++) {
					var c = colors[color];
					SetColor(color, c.Item1, c.Item2, c.Item3);
				}
			}

			public static IDictionary<ushort, Tuple<short, short, short>> Colors {
				get {
					return GetColors();
				}
				set {
					SetColors(value);
				}
			}

			public static void GetPair(short index, out ushort foreground, out ushort background)
			{
				Curses.pair_content(index, out foreground, out background);
			}
		}

		public enum StaticColor : int
		{
			Black   = 0,
			Red     = 1,
			Green   = 2,
			Yellow  = 3,
			Blue    = 4,
			Magenta = 5,
			Cyan    = 6,
			White   = 7,
		}

		public static class Attributes {
			public const int Normal    = unchecked((int)0x0);
			public const int Standout  = unchecked((int)0x10000);
			public const int Underline = unchecked((int)0x20000);
			public const int Reverse   = unchecked((int)0x40000);
			public const int Blink     = unchecked((int)0x80000);
			public const int Dim       = unchecked((int)0x100000);
			public const int Bold      = unchecked((int)0x200000);
			public const int Protect   = unchecked((int)0x1000000);
			public const int Invisible = unchecked((int)0x800000);
		}

		public struct Color
		{
			public short R;
			public short G;
			public short B;
		}

		[DllImport("ncursesw")]
		internal static extern IntPtr initscr();

		[DllImport("ncursesw")]
		internal static extern bool noecho();

		[DllImport("ncursesw")]
		internal static extern bool echo();

		public static bool Echo {
			set {
				if (value) {
					echo();
				} else {
					noecho();
				}
			}
		}

		[DllImport("ncursesw")]
		internal static extern bool raw();

		[DllImport("ncursesw")]
		internal static extern bool noraw();

		public static bool Raw {
			set {
				if (value) {
					raw();
				} else {
					noraw();
				}
			}
		}
		
		[DllImport("ncursesw", EntryPoint="addch")]
		internal static extern int addch(int ch);
				
		public static int Add(int ch)
		{
			if (ch < 127 || ch > 0xffff) {
				return addch(ch);
			}

			return Add((char)ch);
		}
		
		public static int Add(char ch)
		{
			return addstr(new string(ch, 1));
		}
		
		[DllImport("ncursesw")]
		internal static extern int addstr(string str);
		
		public static int Add(string str)
		{
			return addstr(str);
		}

		public static int Add(string str, params object[] args)
		{
			return addstr(string.Format(str, args));
		}

		public static int Refresh()
		{
			return Window.Standard.Refresh();
		}

		[DllImport("ncursesw")]
		internal static extern int getch();

		public int WindowWidth {
			get {
				return Console.WindowWidth;
			}
		}

		public int WindowHeight {
			get {
				return Console.WindowHeight;
			}
		}

		[DllImport("ncursesw")]
		internal static extern int start_color();

		[DllImport("ncursesw")]
		internal static extern int use_default_colors();

		[DllImport("ncursesw")]
		internal static extern int curs_set(int visibility);

		[DllImport("ncursesw")]
		public static extern int init_pair(ushort pair, ushort f, ushort b);

		[DllImport("ncursesw")]
		internal static extern bool can_change_color();

		[DllImport("ncursesw")]
		internal static extern int attrset(int attr);

		[DllImport("ncursesw")]
		public static extern int attron(int attr);

		[DllImport("ncursesw")]
		public static extern int attroff(int attr);

		[DllImport("ncursesw")]
		internal static extern int init_color(ushort color, short r, short g, short b);

		[DllImport("ncursesw")]
		internal static extern int color_content(ushort color, out short r, out short g, out short b);

		[DllImport("ncursesw")]
		internal static extern int pair_content(short index, out ushort foreground, out ushort background);

		[DllImport("ncursesw")]
		public static extern int COLOR_PAIR(int z);

		[DllImport("ncursesw")]
		public static extern int resizeterm(int y, int x);

		[DllImport("ncursesw")]
		internal static extern int keypad(IntPtr window, bool bf);
	}
}

