using System;
using System.Collections.Generic;
using LibuvSharp;

namespace Terminal
{
	public class Application
	{
		public static Loop Loop { get; protected set; }

		public static int QuitKey { get; set; }
		public static int Timeout { get; set; }

		public static void Exit()
		{
			if (stdin != null) {
				stdin.Close();
				stdin = null;
			}

			if (signalWatcher != null) {
				signalWatcher.Stop();
				signalWatcher.Close();
			}

			Loop.Stop();
		}

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
			Debug.Log("Application Init");

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

		public static event Action End;

		static Action<int> keyaction;

		static Poll stdin;
		static SignalWatcher signalWatcher;
		static Prepare prepare;


		public static void Run(Container container)
		{
			Debug.Log("Application Start");

			Debug.Log("Application ThreadId: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
			try {
				container = new ApplicationContainer(container);

				if (Loop == null) {
					throw new Exception("You have to initialize the application with a context");
				}

				if (container.CanFocus) {
					container.HasFocus = true;
				}

				// draw everything and refresh curses
				Debug.Log("Terminal width: {0} Terminal height: {1}", Curses.Terminal.Width, Curses.Terminal.Height);
				container.SetDim(0, 0, Curses.Terminal.Width, Curses.Terminal.Height);

				container.Redraw();
				container.SetCursorPosition();
				Curses.Refresh();

				keyaction = (key) => {
					if (key == QuitKey) {
						Exit();
					} else if (key == -2) {
						container.Redraw();
						container.SetCursorPosition();
						Curses.Refresh();
					} else if (key == Curses.Key.Resize) {
						container.SetDim(0, 0, Curses.Terminal.Width, Curses.Terminal.Height);
						container.ProcessKey(Curses.Key.Resize);
						container.ForceRedraw();
						container.SetCursorPosition();
						container.Invalid = true;
						Curses.Refresh();
					} else {
						container.ProcessKey(key);
					}
				};

				signalWatcher = new SignalWatcher(Loop, Signum.SIGWINCH , () => {
					Curses.resizeterm(Console.WindowHeight, Console.WindowWidth);
					keyaction(Curses.Key.Resize);
				});
				signalWatcher.Start();

				stdin = new Poll(Loop, 0);
				stdin.Event += (_) => {
					keyaction(Curses.getch());
				};
				stdin.Start(PollEvent.Read);

				prepare = new Prepare();
				prepare.Start(() => {
					if (container.Invalid) {
						keyaction(-2);
					}
				});

				if (colors != null) {
					Curses.Terminal.SetColors(colors);
				}

				Loop.Run();

				OnEnd();
			} finally {
				Window.End();
				Running = false;

				Loop = null;
				Debug.Log("Application End");
			}
		}
	}
}

