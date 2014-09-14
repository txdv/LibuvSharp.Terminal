using System;
using System.Collections.Generic;

namespace LibuvSharp.Terminal
{
	public class Application
	{
		public static Loop Loop { get; protected set; }

		public static int QuitKey { get; set; }
		public static int Timeout { get; set; }
		public static bool Exit { get; set; }

		public static bool Running { get; private set; }
		private static bool Drawing { get; set; }

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
			Init(Loop.Default);
		}

		public static void Init(Loop loop)
		{
			Window.Init();

			Running = true;

			Curses.Raw = true;
			Curses.Echo = false;

			Curses.start_color();
			Curses.use_default_colors();

			Window.Standard.Keypad = true;

			Loop = loop;
		}

		static void OnEnd()
		{
			if (End != null) {
				End();
			}
		}

		public static Action End;

		static Action<int> keyaction;

		static Poll stdin;
		static SignalWatcher sw;


		public static void Run(Container container)
		{
			container = new ApplicationContainer(container);

			if (Loop == null) {
				throw new Exception("You have to initialize the application with a context");
			}

			if (container.CanFocus) {
				container.HasFocus = true;
			}

			// draw everything and refresh curses
			container.SetDim(0, 0, Curses.Terminal.Width, Curses.Terminal.Height);

			container.Redraw();
			container.SetCursorPosition();
			Curses.Refresh();

			sw = new SignalWatcher(Loop, Signum.SIGWINCH , () => {
				Curses.resizeterm(Console.WindowHeight, Console.WindowWidth);
				keyaction(Curses.Key.Resize);
			});

			keyaction = (key) => {
				if (key == QuitKey) {
					if (stdin != null) {
						stdin.Close();
						stdin = null;
					}

					if (sw != null) {
						sw.Stop();
						sw.Close();
					}

					Exit = true;
				} else if (key == -2) {
					container.Redraw();
					container.SetCursorPosition();
					Curses.Refresh();
				} else if (key == Curses.Key.Resize) {
					container.SetDim(0, 0, Curses.Terminal.Width, Curses.Terminal.Height);
					container.ProcessKey(Curses.Key.Resize);
					container.ForceRedraw();
					container.SetCursorPosition();
					Curses.Refresh();
				} else {
					container.ProcessKey(key);
				}
			};

			stdin = new Poll(Loop, 0);
			stdin.Event += (_) => {
				keyaction(Curses.getch());
			};
			stdin.Start(PollEvent.Read);

			sw.Start();

			if (colors != null) {
				Curses.Terminal.SetColors(colors);
			}

			while (!Exit) {
				if (container.Invalid) {
					keyaction(-2);
				}
				Loop.RunOnce();
			}
			OnEnd();

			Window.End();
			Running = false;

			Loop = null;
		}
	}
}

