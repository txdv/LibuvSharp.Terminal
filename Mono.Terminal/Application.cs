using System;
using System.Collections.Generic;

namespace Mono.Terminal
{
	public class Application
	{
		public static int QuitKey { get; set; }
		public static int Timeout { get; set; }
		public static IntPtr Screen { get; protected set; }
		public static bool Exit { get; set; }

		private static IKeyDispatcher KeyDispatcher { get; set; }

		public static bool Running { get; private set; }

		static Application()
		{
			Timeout = 1000;
			QuitKey = 17;
		}

		private static IDictionary<ushort, Tuple<short, short, short>> colors;

		public static void SaveColors()
		{
			colors = Curses.Terminal.GetColors();
		}

		public static void Init()
		{
			Window.Init();

			Running = true;

			Curses.Raw = true;
			Curses.Echo = false;

			Curses.start_color();
			Curses.use_default_colors();

			Window.Standard.Keypad = true;
		}

		public static void Run(IKeyDispatcher keyDispatcher, Container container)
		{
			if (container.CanFocus) {
				container.HasFocus = true;
			}


			// draw everything and refresh curses
			container.SetDim(0, 0, Curses.Terminal.Width, Curses.Terminal.Height);

			container.Redraw();
			container.SetCursorPosition();
			Curses.Refresh();

			bool done = false;
			while (!done && !Exit) {
				keyDispatcher.Dispatch(Timeout, delegate (int key) {
					if (QuitKey == key) {
						done = true;
					} else {
						container.ProcessKey(key);
						container.Redraw();
						container.SetCursorPosition();
						Curses.Refresh();
					}
				});
			}


			if (colors != null) {
				Curses.Terminal.SetColors(colors);
			}


			Curses.endwin();
			Running = false;
		}

	}
}

