using System;
using System.Collections.Generic;
using Mono.Unix.Native;
using Manos.IO;

namespace Mono.Terminal
{
	public class Application
	{
		public static Context Context { get; protected set; }

		public static int QuitKey { get; set; }
		public static int Timeout { get; set; }
		public static bool Exit { get; set; }

		public static bool Running { get; private set; }

		static Application()
		{
			Timeout = -1;
			QuitKey = 17;
		}

		private static IDictionary<ushort, Tuple<short, short, short>> colors;

		public static void SaveColors()
		{
			colors = Curses.Terminal.GetColors();
		}

		public static void RestoreColors()
		{
			if (colors != null) {
				Curses.Terminal.SetColors(colors);
			}
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

		static Action<int> keyaction;

		public static void Refresh()
		{
			if (keyaction != null) {
				keyaction(-2);
			}
		}

		static IStdin stdin;
		static SignalWatcher sw;

		public static void Run(Context context, Container container)
		{
			Context = context;

			if (container.CanFocus) {
				container.HasFocus = true;
			}

			// draw everything and refresh curses
			container.SetDim(0, 0, Curses.Terminal.Width, Curses.Terminal.Height);

			container.Redraw();
			container.SetCursorPosition();
			Curses.Refresh();

			keyaction = (key) => {
				if (key == QuitKey) {
					context.Stop();
					Window.End();
					System.Diagnostics.Process.GetCurrentProcess().Kill();
				} else if (key == -2) {
					container.Redraw();
					Curses.Refresh();
				} else {
					if (key == Curses.Key.Resize) {
						container.SetDim(0, 0, Curses.Terminal.Width, Curses.Terminal.Height);
					}
					container.ProcessKey(key);
					container.Redraw();
					container.SetCursorPosition();
					Curses.Refresh();
				}
			};

			stdin = context.OpenStdin();
			stdin.Ready(() => {
				keyaction(Curses.getch());
			});

			sw = new SignalWatcher(context, Signum.SIGWINCH , () => {
				Curses.resizeterm(Console.WindowHeight, Console.WindowWidth);
				keyaction(-2);
			});

			sw.Start();

			if (colors != null) {
				Curses.Terminal.SetColors(colors);
			}

			context.Start();

			Window.End();
			Running = false;

			Context = null;
		}
	}
}

