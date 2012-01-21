using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mono.Unix.Native;
using Mono.Unix;

namespace Mono.Terminal.Poll
{
	public class Watcher
	{
		internal Pollfd pollfd;

		public int FileDescriptor { get; protected set; }

		public Watcher(int fd)
		{
			FileDescriptor = fd;
			pollfd = new Pollfd();
			pollfd.fd = fd;
			pollfd.events |= PollEvents.POLLIN;
		}

		public event Action ReadEvent;

		internal void Dispatch(PollEvents events)
		{
			if ((events & PollEvents.POLLIN) > 0) {
				if (ReadEvent != null) {
					ReadEvent();
				}
			}
		}
	}

	public class Pipe
	{
		int[] pipe = new int[2];

		public Pipe()
		{
			Syscall.pipe(pipe);
		}

		public int Out {
			get {
				return pipe[0];
			}
		}

		public int In {
			get {
				return pipe[1];
			}
		}

		public void Write(IntPtr buf, ulong count)
		{
			Syscall.write(In, buf, count);
		}

		public void Read(IntPtr buf, ulong count)
		{
			Syscall.read(Out, buf, count);
		}

		public void Close()
		{
			for (int i = 0; i < pipe.Length; i++) {
				Syscall.close(pipe[i]);
			}
		}
	}

	public class SignalWatcher
	{
		Thread thread;
		Pipe pipe;
		UnixSignal[] unixSignals;
		public Watcher Watcher { get; protected set; }
		public Signum[] Signals { get; protected set; }

		IntPtr readBuffer;
		IntPtr writeBuffer;

		public SignalWatcher(Signum[] signals)
		{
			Signals = signals;
			unixSignals = new UnixSignal[signals.Length];
			for (int i = 0; i < signals.Length; i++) {
				unixSignals[i] = new UnixSignal(signals[i]);
			}
			pipe = new Pipe();
			Watcher = new Watcher(pipe.Out);
			readBuffer = Marshal.AllocHGlobal(4);
			writeBuffer = Marshal.AllocHGlobal(4);
			thread = new Thread(() => {
				while (true) {
					var num = UnixSignal.WaitAny(unixSignals);
					Marshal.Copy(BitConverter.GetBytes((int)Signals[num]), 0, writeBuffer, 4);
					pipe.Write(writeBuffer, 4);
				}
			});
			Watcher.ReadEvent += () => {
				pipe.Read(readBuffer, 4);
				byte[] buff = new byte[4];
				Marshal.Copy(readBuffer, buff, 0, 4);
				Signum signum = (Signum)BitConverter.ToInt32(buff, 0);
				if (OnSignal != null) {
					OnSignal(signum);
				}
			};
		}

		public event Action<Signum> OnSignal;

		public void Start()
		{
			if (thread != null && !thread.IsAlive) {
				thread.Start();
			}
		}

		public void Stop()
		{
			if (thread != null && thread.IsAlive) {
				thread.Abort();
			}
		}

		public void Close()
		{
			Stop();
			Marshal.FreeHGlobal(readBuffer);
			Marshal.FreeHGlobal(writeBuffer);
			pipe.Close();
		}
	}

	public class Loop
	{
		List<Watcher> watchers = new List<Watcher>();

		public Loop()
		{
		}

		public void Add(Watcher watcher)
		{
			watchers.Add(watcher);
		}

		public void Dispatch(int timeout)
		{
			int count = watchers.Count;
			Pollfd[] pollmap = new Pollfd[count];
			for (int i = 0; i < count; i++) {
				pollmap[i] = watchers[i].pollfd;
			}

			Syscall.poll(pollmap, timeout);

			for (int i = 0; i < count; i++) {
				watchers[i].Dispatch(pollmap[i].revents);
			}

		}
	}

	public class PollKeyDispatcher : IKeyDispatcher
	{
		SignalWatcher sw = new SignalWatcher(new Signum[] { Signum.SIGWINCH });

		public bool Running { get; protected set; }

		public Loop Loop { get; set; }

		public PollKeyDispatcher()
			: this(new Loop())
		{
		}

		public PollKeyDispatcher(Loop loop)
		{
			Running = true;

			Loop = loop;
			var watcher = new Watcher(0);
			watcher.ReadEvent += delegate {
				int ch = Curses.getch();
				if (KeyPress != null) {
					KeyPress(ch);
				}
			};
			Loop.Add(watcher);

			sw.OnSignal += (obj) => {
				Curses.resizeterm(Console.WindowHeight, Console.WindowWidth);
				int ch = Curses.getch();
				if (KeyPress != null) {
					KeyPress(ch);
				}
			};
			sw.Start();
			Loop.Add(sw.Watcher);
		}

		public event Action<int> KeyPress;

		public void Run(int timeout)
		{
			while (Running) {
				Loop.Dispatch(timeout);
			}
		}

		public void Run()
		{
			Run(0);
		}

		public void Finish()
		{
			Running = false;
			sw.Close();
		}
	}
}

