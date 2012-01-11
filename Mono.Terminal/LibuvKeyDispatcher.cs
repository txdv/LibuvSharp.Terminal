using System;
using Libuv;

namespace Mono.Terminal
{
	public class LibuvKeyDispatcher : IKeyDispatcher
	{
		public Loop Loop { get; protected set; }
		public TTY Stdin { get; protected set; }

		public LibuvKeyDispatcher()
			: this(Loop.Default)
		{
		}

		public LibuvKeyDispatcher(Loop loop)
		{
			Loop = loop;
			Stdin = new TTY(loop, new IntPtr(0), 1);
			var stream = Stdin.Stream;
			stream.ReadWatcher(() => {
				int ch = Curses.getch();
				if (KeyPress != null) {
					KeyPress(ch);
				}
			});
			stream.ResumeWatcher();
		}

		/*
		public void Dispatch(int timeout, Action<int> callback)
		{
			var stream = Stdin.Stream;
			//Stdin.Mode = TTYMode.Raw;
			stream.ReadWatcher(() => {
				callback(Curses.getch());
			});
			stream.ResumeWatcher();
		}*/

		public event Action<int> KeyPress;

		public void Run(int timeout)
		{
			Run();
		}

		public void Run()
		{
			Loop.Run();
		}

		public void Finish()
		{
			Stdin.Stream.Pause();
			Stdin.Close();
		}
	}
}
