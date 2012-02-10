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

		public static void Init(Context context)
		{
			Window.Init();

			Running = true;

			Curses.Raw = true;
			Curses.Echo = false;

			Curses.start_color();
			Curses.use_default_colors();

			Window.Standard.Keypad = true;

			Context = context;
		}

		static void OnEnd()
		{
			if (End != null) {
				End();
			}
		}

		public static Action End;

		static Action<int> keyaction;

		static IStdin stdin;
		static SignalWatcher sw;

		public static void Run(Container container)
		{
			container = new ApplicationContainer(container);

			if (Context == null) {
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

			sw = new SignalWatcher(Context, Signum.SIGWINCH , () => {
				Curses.resizeterm(Console.WindowHeight, Console.WindowWidth);
				container.Redraw();
			});

			keyaction = (key) => {
				if (key == QuitKey) {
					sw.Stop();
					Context.Stop();
				} else if (key == -2) {
					container.Redraw();
					container.SetCursorPosition();
					Curses.Refresh();
				} else {
					if (key == Curses.Key.Resize) {
						container.SetDim(0, 0, Curses.Terminal.Width, Curses.Terminal.Height);
					}
					container.ProcessKey(key);
				}
			};

			stdin = Context.OpenStdin();
			stdin.Ready(() => {
				keyaction(Curses.getch());
			});

			sw.Start();

			Context.CreateIdleWatcher(() => {
				if (container.Invalid) {
					keyaction(-2);
				}
			}).Start();

			if (colors != null) {
				Curses.Terminal.SetColors(colors);
			}

			Context.Start();
			Context.Dispose();
			OnEnd();

			Window.End();
			Running = false;

			Context = null;
		}
	}
}

