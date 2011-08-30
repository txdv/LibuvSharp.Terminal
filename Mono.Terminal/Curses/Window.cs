using System;
using System.Runtime.InteropServices;

namespace Mono.Terminal
{
	public class Window
	{
		public static Window Standard { get; private set; }
		public static Window New      { get; private set; }
		public static Window Current  { get; private set; }

		unsafe internal static void Init()
		{
			Curses.initscr();
			Curses.Init();
			IntPtr ptr;

			Curses.Module.Symbol("stdscr", out ptr);
			Standard = new Window(new IntPtr(*(int *)ptr.ToPointer()));

			Curses.Module.Symbol("newscr", out ptr);
			New = new Window(new IntPtr(*(int *)ptr.ToPointer()));

			Curses.Module.Symbol("curscr", out ptr);
			Current = new Window(new IntPtr(*(int *)ptr.ToPointer()));
		}

		internal readonly IntPtr handle;

		internal struct _win_st {
			public short _cury;
			public short _curx;

			public short _maxy;
			public short _maxx;

			public short _begy;
			public short _begx;
		}

		unsafe internal _win_st *win_st {
			get {
				return (_win_st *)(handle.ToPointer());
			}
		}

		public class WindowCursor
		{
			internal Window Window { get; set; }

			internal WindowCursor(Window win)
			{
				Window = win;
			}

			unsafe public int X {
				get {
					return Window.win_st->_curx;
				}
			}

			unsafe public int Y {
				get {
					return Window.win_st->_cury;
				}
			}

			[DllImport("ncursesw")]
			internal static extern int wmove(IntPtr window, int line, int col);

			public int Move(int x, int y)
			{
				return wmove(Window.handle, y, x);
			}
		}

		public WindowCursor Cursor { get; protected set; }

		internal Window(IntPtr ptr)
		{
			handle = ptr;
			Cursor = new WindowCursor(this);
		}

		[DllImport("ncursesw")]
		private static extern int keypad(IntPtr window, bool bf);

		[DllImport("ncursesw")]
		private static extern bool is_keypad(IntPtr window);

		public bool Keypad {
			get {
				return is_keypad(handle);
			}
			set {
				keypad(handle, value);
			}
		}

		[DllImport("ncursesw")]
		private static extern int wclear(IntPtr window);

		public void Clear()
		{
			wclear(handle);
		}

		[DllImport("ncursesw")]
		private static extern bool is_cleared(IntPtr window);

		public bool IsCleared {
			get {
				return is_cleared(handle);
			}
		}

		[DllImport("ncursesw")]
		internal static extern int wrefresh(IntPtr window);

		public int Refresh()
		{
			return wrefresh(handle);
		}

		[DllImport("ncursesw")]
		internal static extern int wredrawwin(IntPtr window);

		[DllImport("ncursesw")]
		internal static extern int wredrawln(IntPtr window, int begin_line, int lines);

		public int Redraw()
		{
			return wredrawwin(handle);
		}

		public int Redraw(int beginningLine, int lines)
		{
			return wredrawln(handle, beginningLine, lines);
		}

		[DllImport("ncursesw")]
		internal static extern bool is_notimeout(IntPtr window);

		[DllImport("ncursesw")]
		internal static extern int notimeout(IntPtr window, bool bf);

		public bool NoTimeout {
			get {
				return is_notimeout(handle);
			}
			set {
				notimeout(handle, value);
			}
		}

		[DllImport("ncursesw")]
		internal static extern int wtimeout(IntPtr window, int delay);

		public int Timeout {
			set {
				wtimeout(handle, value);
			}
		}

		[DllImport("ncursesw")]
		internal static extern int nodelay(IntPtr window, bool bf);

		[DllImport("ncursesw")]
		internal static extern bool is_nodelay(IntPtr window);

		public bool Nodelay {
			get {
				return is_nodelay(handle);
			}
			set {
				nodelay(handle, value);
			}
		}

		[DllImport("ncursesw")]
		internal static extern int meta(IntPtr window, bool bf);

		public bool Meta {
			set {
				meta(handle, value);
			}
		}


	}
}

