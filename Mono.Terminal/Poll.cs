using System;
using System.Collections.Generic;
using Mono.Unix.Native;

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
		Action<int> callback;

		public Loop Loop { get; set; }

		public PollKeyDispatcher(Loop loop)
		{
			Loop = loop;
			var watcher = new Watcher(0);
			watcher.ReadEvent += delegate {
				int ch = Curses.getch();
				if (callback != null) {
					callback(ch);
				}
			};
			Loop.Add(watcher);
		}

		public void Dispatch(int timeout, Action<int> callback)
		{
			this.callback = callback;
			Loop.Dispatch(timeout);
		}
	}}

