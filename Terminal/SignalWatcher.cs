using System;
using System.Threading;
using Mono.Unix;
using Mono.Unix.Native;
using LibuvSharp;
using Signum = LibuvSharp.Signum;

namespace Terminal
{
	public class SignalWatcher
	{
		Thread thread;
		UnixSignal[] unixSignals;
		AsyncWatcher<Signum> watcher;

		public Signum[] Signals { get; protected set; }

		public SignalWatcher(Loop loop, Signum signum, Action callback)
			: this(loop, new Signum[] { signum }, (num) => {
				if (callback != null) {
					callback();
				}
			})
		{
		}

		public SignalWatcher(Loop loop, Signum[] signals, Action<Signum> callback)
		{
			Signals = signals;
			unixSignals = new UnixSignal[signals.Length];
			for (int i = 0; i < signals.Length; i++) {
				unixSignals[i] = new UnixSignal((Mono.Unix.Native.Signum)signals[i]);
			}

			watcher = new AsyncWatcher<Signum>(loop);
			watcher.Callback += (key) => {
				if (callback != null) {
					callback(key);
				}
			};

			thread = new Thread((o) => {
				while (true) {
					var index = UnixSignal.WaitAny(unixSignals);
					watcher.Send(Signals[index]);
				}
			});
		}

		public void Start()
		{
			thread.Start();
		}

		public void Stop()
		{
			thread.Abort();
		}

		public void Close()
		{
			watcher.Close();
		}
	}
}

